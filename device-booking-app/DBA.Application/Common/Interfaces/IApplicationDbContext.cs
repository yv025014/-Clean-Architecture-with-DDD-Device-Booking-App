using DBA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DBA.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<DeviceBooking> DeviceBookings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
