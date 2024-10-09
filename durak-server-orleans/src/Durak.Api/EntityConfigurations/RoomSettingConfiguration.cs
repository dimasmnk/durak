using Durak.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Durak.Api.EntityConfigurations;

public class RoomSettingConfiguration : IEntityTypeConfiguration<RoomSetting>
{
    public void Configure(EntityTypeBuilder<RoomSetting> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
