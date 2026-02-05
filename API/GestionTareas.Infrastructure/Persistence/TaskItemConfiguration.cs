using GestionTareas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTareas.Infrastructure.Persistence;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> b)
    {
        b.ToTable("Tasks");

        b.HasKey(x => x.Id);

        b.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.Description)
            .HasMaxLength(1000);

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        b.Property(x => x.AssignedUserId)
            .IsRequired()
            .HasColumnName("UserId");

        b.HasOne(x => x.AssignedUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        // ✅ La columna real es AdditionalInfo
        b.Property(x => x.AdditionalInfoJson)
            .HasColumnName("AdditionalInfo")
            .HasColumnType("nvarchar(max)");

        b.HasIndex(x => new { x.AssignedUserId, x.Status });
        b.HasIndex(x => x.CreatedAt);

        b.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Tasks_Status",
                "[Status] IN ('Pending','InProgress','Done')"
            );
            
            t.HasCheckConstraint(
                "CK_Tasks_AdditionalInfo_IsJson",
                "[AdditionalInfo] IS NULL OR ISJSON([AdditionalInfo]) = 1"
            );

            t.HasCheckConstraint(
                "CK_Tasks_Title_NotBlank",
                "LEN(LTRIM(RTRIM([Title]))) > 0"
            );
        });
    }
}