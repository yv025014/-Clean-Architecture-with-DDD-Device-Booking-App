using DBA.Application.Common.Exceptions;
using DBA.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBA.Application.DeviceBookings.Commands;

public record RejectDeviceBookingCommand(Guid Id, string Approver, string Reason) : IRequest<Guid>;

public class RejectDeviceBookingCommandValidator : AbstractValidator<RejectDeviceBookingCommand>
{
    public RejectDeviceBookingCommandValidator()
    {
        RuleFor(x => x.Approver).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(100);
    }
}

public class RejectDeviceBookingCommandHandler : IRequestHandler<RejectDeviceBookingCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RejectDeviceBookingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RejectDeviceBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.DeviceBookings
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(request.Id);

        booking.ToReject(request.Approver, request.Reason);
        await _context.SaveChangesAsync(cancellationToken);
        return booking.Id;
    }
}
