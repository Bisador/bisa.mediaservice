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
            .HasMaxLength(100);
  
        builder.HasIndex(x =>
            new
            {
                x.OwnerType,
                x.OwnerId
            });

        builder.HasOne(x => x.Media)
            .WithMany(x => x.Links)
            .HasForeignKey(x => x.MediaId);
    }
}