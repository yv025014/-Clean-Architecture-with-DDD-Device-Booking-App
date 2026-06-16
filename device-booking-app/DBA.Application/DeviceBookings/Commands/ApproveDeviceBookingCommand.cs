using DBA.Application.Common.Exceptions;
using DBA.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBA.Application.DeviceBookings.Commands;

public record ApproveDeviceBookingCommand(Guid Id, string Approver) : IRequest<Guid>;

public class ApproveDeviceBookingCommandValidator : AbstractValidator<ApproveDeviceBookingCommand>
{
    public ApproveDeviceBookingCommandValidator()
    {
        RuleFor(x => x.Approver).NotEmpty().MaximumLength(20);
    }
}

public class ApproveDeviceBookingCommandHandler : IRequestHandler<ApproveDeviceBookingCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public ApproveDeviceBookingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ApproveDeviceBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.DeviceBookings
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(request.Id);

        booking.ToApprove(request.Approver);
        await _context.SaveChangesAsync(cancellationToken);
        return booking.Id;
    }
}
