using Durak.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Durak.Api.EntityConfigurations;

public class RoomUserConfiguration : IEntityTypeConfiguration<RoomUser>
{
    public void Configure(EntityTypeBuilder<RoomUser> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.RoomUsers)
            .HasForeignKey(x => x.RoomId);
    }
}
