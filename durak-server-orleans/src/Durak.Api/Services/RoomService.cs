using Durak.Api.Contracts.Dtos;
using Durak.Api.Contracts.Requests;
using Durak.Api.Entities;
using Durak.Api.Hubs.RoomListEvents;
using Durak.Api.Mappers;
using Durak.Api.Persistence;
using Durak.Api.Repositories;

namespace Durak.Api.Services;

public class RoomService(IRoomRepository roomRepository,
    IDurakDbContext context,
    ILogger<RoomService> logger,
    IUserService userService,
    IRoomUserRepository roomUserRepository,
    IRoomListEventService roomListEventService,
    IPublicRoomRepository publicRoomRepository) : IRoomService
{
    private readonly IDurakDbContext _context = context;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly IUserService _userService = userService;
    private readonly IRoomUserRepository _roomUserRepository = roomUserRepository;
    private readonly IRoomListEventService _roomListEventService = roomListEventService;
    private readonly IPublicRoomRepository _publicRoomRepository = publicRoomRepository;
    private readonly ILogger<RoomService> _logger = logger;

    public async Task<Guid> CreateRoomAsync(CreateRoomRequest createRoomRequest, long userId, CancellationToken cancellationToken)
    {
        createRoomRequest.MaxPlayerCount = 2;

        if (createRoomRequest.MaxPlayerCount < 2 || createRoomRequest.MaxPlayerCount > 4)
            throw new ArgumentException("Max player count must be between 2 and 4");

        if (createRoomRequest.Bet < 1 && createRoomRequest.Bet > 500)
            throw new ArgumentException("Bet must be greater than 0 and lower than 500");

        var roomId = Guid.NewGuid();
        var roomSetting = new RoomSetting
        {
            Id = roomId,
            Bet = createRoomRequest.Bet,
            IsPublic = createRoomRequest.IsPublic,
            MaxPlayerCount = createRoomRequest.MaxPlayerCount
        };
        var room = new Room
        {
            Id = roomId,
            RoomSetting = roomSetting
        };
        await _roomRepository.AddRoomAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Room {RoomId} has been created by user {UserId}", roomId, userId);

        return roomId;
    }

    public async Task<RoomDto> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetRoomNoTrackingAsync(roomId, cancellationToken);
        return room.ToRoomDto();
    }

    public async Task<List<RoomInListDto>> GetPublicRoomsAsync(CancellationToken cancellationToken)
    {
        var publicRooms = await _roomRepository.GetPublicRoomsNoTrackingAsync(cancellationToken);
        var publicRoomDtos = publicRooms.Select(r => r.ToRoomInListDto()).ToList();
        return publicRoomDtos;
    }

    public async Task<bool> TryJoinRoomAsync(long userId, Guid roomId, CancellationToken cancellationToken)
    {
        var isRoomExists = await _roomRepository.IsRoomExistsAsync(roomId, cancellationToken);
        if (!isRoomExists)
        {
            _logger.LogError("Room {RoomId} does not exist", roomId);
            return false;
        }

        var isRoomUserExists = await _roomUserRepository.IsRoomUserExistsAsync(userId, cancellationToken);
        if (isRoomUserExists)
        {
            _logger.LogError("User {UserId} is already in a room", userId);
            return false;
        }
        var room = await _roomRepository.GetRoomNoTrackingAsync(roomId, cancellationToken);
        var userBalance = await _userService.GetUserBalanceAsync(userId, cancellationToken);
        if (userBalance < room.RoomSetting.Bet)
        {
            _logger.LogError("User {UserId} does not have enough coins to join the room", userId);
            return false;
        }
        if (room.PlayerCount >= room.RoomSetting.MaxPlayerCount)
        {
            _logger.LogError("Room {RoomId} is full", roomId);
            return false;
        }

        var isRoomInitialized = room.PlayerCount == 0;

        var roomUser = new RoomUser
        {
            RoomId = roomId,
            UserId = userId
        };

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        await _userService.WithdrawDepositAsync(userId, room.RoomSetting.Bet, cancellationToken);
        await _roomUserRepository.AddRoomUser(roomUser, cancellationToken);
        await _roomRepository.UpdateRoomPlayerCountAsync(roomId, room.PlayerCount + 1, cancellationToken);

        if (isRoomInitialized && room.RoomSetting.IsPublic)
            await _publicRoomRepository.AddPublicRoomAsync(new PublicRoom() { Id = room.Id }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        transaction.Commit();
        _logger.LogInformation("User {UserId} has joined room {RoomId}", userId, roomId);

        if (isRoomInitialized)
            await _roomListEventService.SendAsync(new RoomAdditionEventData(room.RoomSetting, room.PlayerCount + 1), cancellationToken);
        else
            await _roomListEventService.SendAsync(new RoomPlayerCountUpdateEventData(room.Id, room.PlayerCount + 1), cancellationToken);

        return true;
    }
}
