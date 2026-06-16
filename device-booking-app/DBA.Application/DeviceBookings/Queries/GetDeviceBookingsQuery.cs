using DBA.Application.Common.Interfaces;
using DBA.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBA.Application.DeviceBookings.Queries;

public record GetDeviceBookingsQuery : IRequest<List<DeviceBookingDto>>;

public class GetDeviceBookingsQueryHandler : IRequestHandler<GetDeviceBookingsQuery, List<DeviceBookingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDeviceBookingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeviceBookingDto>> Handle(GetDeviceBookingsQuery request, CancellationToken cancellationToken)
    {
        return await _context.DeviceBookings
            .Select(x => new DeviceBookingDto(
                x.Id,
                x.DeviceName,
                x.Applicant,
                x.ExpectedReturn,
                x.ActualReturn,
                x.Approver,
                (int)x.Status,
                x.Reason))
            .ToListAsync(cancellationToken);
    }
}
