using MediaService.Domain.Entities;
using MediaService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaService.Infrastructure.Persistence.Configurations;

public class MediaItemConfiguration : IEntityTypeConfiguration<MediaItem>
{
    public void Configure(EntityTypeBuilder<MediaItem> builder)
    {
        builder.ToTable("MediaItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.TenantId)
            .IsRequired();
        

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.BucketName)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.Property(x => x.Category)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ObjectKey)
            .HasMaxLength(1024)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.StorageProvider)
            .HasMaxLength(64)
            .IsRequired();
        
        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();
        
        builder.Property(x => x.Purpose)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.OwnerId)
            .HasMaxLength(128);

        builder.Property(x => x.Sha256)
            .HasMaxLength(128);
 
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x =>
                new
                {
                    x.BucketName,
                    x.ObjectKey
                })
            .IsUnique();
        
        builder.HasMany(x => x.Links)
            .WithOne(x => x.Media)
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.OwnerId);
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasQueryFilter(x => x.Status != MediaStatus.Deleted);
    }
}