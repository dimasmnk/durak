using AutoFixture.Xunit2;
using Durak.Core.RoomModule.Entities;
using Durak.Core.RoomModule.Enums;
using Durak.Core.RoomModule.ValueObjects;
using Durak.Core.UserModule.Entities;

namespace Durak.Core.UnitTests.RoomModule.Entities;

public class PublicRoomTests
{
    [Theory]
    [AutoData]
    public void AddPlayer_EmptyRoom_ShouldAddPlayerAndUpdateStatus(User user)
    {
        // Arrange
        var settings = new PublicRoomSettings(20, 4);
        var room = PublicRoom.CreateRoom(settings);

        // Act
        room.AddPlayer(user);

        // Assert
        Assert.Single(room.Players);
        Assert.Equal(user.Id, room.Players.First().Id);
        Assert.Equal(RoomStatus.Gathering, room.Status);
    }

    [Theory]
    [AutoData]
    public void AddPlayer_EmptyRoomAndSeveralPlayers_ShouldAddPlayers(User user1, User user2, User user3, User user4)
    {
        // Arrange
        var settings = new PublicRoomSettings(20, 4);
        var room = PublicRoom.CreateRoom(settings);

        // Act
        room.AddPlayer(user1);
        room.AddPlayer(user2);
        room.AddPlayer(user3);
        room.AddPlayer(user4);

        // Assert
        Assert.Equal(4, room.Players.Count);
        Assert.Equal(RoomStatus.Gathering, room.Status);
    }

    [Theory]
    [AutoData]
    public void RemovePlayer_HaveOnePlayer_ShouldRemovePlayerAndUpdateStatus(User user)
    {
        // Arrange
        var settings = new PublicRoomSettings(20, 4);
        var room = PublicRoom.CreateRoom(settings);
        room.AddPlayer(user);

        // Act
        room.RemovePlayer(user);

        // Assert
        Assert.Empty(room.Players);
        Assert.Equal(RoomStatus.Idle, room.Status);
    }

    [Theory]
    [AutoData]
    public void AddPlayer_HaveAvailableSpaceBetweenPlayers_ShouldAddPlayerToAvailablePlace(
        User user1,
        User user2,
        User user3,
        User user4,
        User user5)
    {
        // Arrange
        var settings = new PublicRoomSettings(20, 4);
        var room = PublicRoom.CreateRoom(settings);
        room.AddPlayer(user1);
        room.AddPlayer(user2);
        room.AddPlayer(user3);
        room.AddPlayer(user4);
        room.RemovePlayer(user3);

        // Act
        room.AddPlayer(user5);

        // Assert
        Assert.Equal(user5.Id, room.Players.Last().Id);
    }

    [Theory]
    [AutoData]
    public void AddPlayer_AllPlayersAreReady_ShouldReturnThatAllPLayersAreReady(
        User user1,
        User user2,
        User user3,
        User user4)
    {
        // Arrange
        var settings = new PublicRoomSettings(20, 4);
        var room = PublicRoom.CreateRoom(settings);
        room.AddPlayer(user1);
        room.AddPlayer(user2);
        room.AddPlayer(user3);
        room.AddPlayer(user4);
        room.SetReadyForPlayer(user1.Id);
        room.SetReadyForPlayer(user2.Id);
        room.SetReadyForPlayer(user3.Id);
        room.SetReadyForPlayer(user4.Id);

        // Act
        var areAllPlayersAreReady = room.AreAllPlayersReady();

        // Assert
        Assert.True(areAllPlayersAreReady);
    }
}