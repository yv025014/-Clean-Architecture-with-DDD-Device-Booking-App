using DBA.Application.Common.Exceptions;
using DBA.Application.Common.Interfaces;
using DBA.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBA.Application.DeviceBookings.Queries;

public record GetDeviceBookingByIdQuery(Guid Id) : IRequest<DeviceBookingDto>;

public class GetDeviceBookingByIdQueryHandler : IRequestHandler<GetDeviceBookingByIdQuery, DeviceBookingDto>
{
    private readonly IApplicationDbContext _context;

    public GetDeviceBookingByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeviceBookingDto> Handle(GetDeviceBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var dto = await _context.DeviceBookings
            .Where(x => x.Id == request.Id)
            .Select(x => new DeviceBookingDto(
                x.Id,
                x.DeviceName,
                x.Applicant,
                x.ExpectedReturn,
                x.ActualReturn,
                x.Approver,
                (int)x.Status,
                x.Reason))
            .FirstOrDefaultAsync(cancellationToken);

        if (dto is null)
            throw new NotFoundException(request.Id);

        return dto;
    }
}
