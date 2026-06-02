using MediaService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaService.Infrastructure.Persistence.Configurations;

public sealed class MediaLinkConfiguration : IEntityTypeConfiguration<MediaLink>
{
    public void Configure(EntityTypeBuilder<MediaLink> builder)
    {
        builder.ToTable("MediaLinks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.OwnerId)
            .HasMaxLength(200);
 
        builder.HasIndex(x =>
            new
            {
                x.OwnerType,
                x.OwnerId
            });

        builder.HasIndex(x =>
                new
                {
                    x.MediaId,
                    x.OwnerType,
                    x.OwnerId
                })
            .IsUnique();
    }
}