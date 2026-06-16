using DBA.Domain.Entities;
using DBA.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBA.Infrastructure.Persistence.Configurations;

public class DeviceBookingConfiguration : IEntityTypeConfiguration<DeviceBooking>
{
    public void Configure(EntityTypeBuilder<DeviceBooking> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Applicant)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ExpectedReturn)
            .IsRequired();

        builder.Property(x => x.ActualReturn);

        builder.Property(x => x.Approver)
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(100);
    }
}
