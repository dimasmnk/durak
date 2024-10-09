using Durak.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Durak.Api.EntityConfigurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.RoomSetting)
            .WithOne(x => x.Room)
            .HasForeignKey<RoomSetting>(x => x.Id);
    }
}
