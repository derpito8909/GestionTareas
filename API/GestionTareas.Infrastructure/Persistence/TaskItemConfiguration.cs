using GestionTareas.Domain.Entities;
using GestionTareas.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTareas.Infrastructure.Persistence;

public class TaskItemConfiguration: IEntityTypeConfiguration<TaskItem>
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
            .IsRequired();

        b.HasOne(x => x.AssignedUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETDATE()")
            .ValueGeneratedOnAdd();

        b.Property(x => x.AdditionalInfoJson)
            .HasColumnName("AdditionalInfoJson")
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
                "CK_Tasks_AdditionalInfoJson_IsJson",
                "[AdditionalInfoJson] IS NULL OR ISJSON([AdditionalInfoJson]) = 1"
            );
            
            t.HasCheckConstraint(
                "CK_Tasks_Title_NotBlank",
                "LEN(LTRIM(RTRIM([Title]))) > 0"
            );
        });
    }
}