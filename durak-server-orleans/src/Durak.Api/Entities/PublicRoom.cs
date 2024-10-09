namespace Durak.Api.Entities;

public class PublicRoom
{
    public Guid Id { get; set; }

    public Room Room { get; set; } = null!;
}
