using DBA.Application.Common.Exceptions;
using DBA.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBA.Application.DeviceBookings.Commands;

public record ReturnDeviceBookingCommand(Guid Id) : IRequest<Guid>;

public class ReturnDeviceBookingCommandHandler : IRequestHandler<ReturnDeviceBookingCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public ReturnDeviceBookingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ReturnDeviceBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.DeviceBookings
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(request.Id);

        booking.ToReturn();
        await _context.SaveChangesAsync(cancellationToken);
        return booking.Id;
    }
}
