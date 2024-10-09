namespace Durak.Api.Contracts.Dtos;

public class RoomInListDto
{
    public Guid Id { get; set; }
    public bool IsGameStarted { get; set; }
    public int PlayerCount { get; set; }

    public RoomSettingDto RoomSetting { get; set; } = null!;
}
