using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vizus.Domain.Entities;

namespace Vizus.Infrastructure.Persistence.Configurations;

public class DoctorTimeOffConfiguration : IEntityTypeConfiguration<DoctorTimeOff>
{
    public void Configure(EntityTypeBuilder<DoctorTimeOff> builder)
    {
        builder.ToTable("DoctorTimeOffs");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Reason).HasMaxLength(300);

        builder.HasOne(t => t.Doctor)
            .WithMany(d => d.TimeOffs)
            .HasForeignKey(t => t.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}