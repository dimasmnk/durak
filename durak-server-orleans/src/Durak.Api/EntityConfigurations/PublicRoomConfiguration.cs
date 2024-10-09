using Durak.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Durak.Api.EntityConfigurations;

public class PublicRoomConfiguration : IEntityTypeConfiguration<PublicRoom>
{
    public void Configure(EntityTypeBuilder<PublicRoom> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Room)
            .WithOne(x => x.PublicRoom)
            .HasForeignKey<PublicRoom>(x => x.Id);
    }
}
