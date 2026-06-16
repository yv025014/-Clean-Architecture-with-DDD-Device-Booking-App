using DBA.Application.Common.Interfaces;
using DBA.Domain.Entities;
using FluentValidation;
using MediatR;

namespace DBA.Application.DeviceBookings.Commands;

public record CreateDeviceBookingCommand(string DeviceName, string Applicant, DateTimeOffset ExpectedReturn) : IRequest<Guid>;

public class CreateDeviceBookingCommandValidator : AbstractValidator<CreateDeviceBookingCommand>
{
    public CreateDeviceBookingCommandValidator()
    {
        RuleFor(x => x.DeviceName).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Applicant).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ExpectedReturn).NotEmpty();
    }
}

public class CreateDeviceBookingCommandHandler : IRequestHandler<CreateDeviceBookingCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateDeviceBookingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDeviceBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = DeviceBooking.Create(request.DeviceName, request.Applicant, request.ExpectedReturn);
        _context.DeviceBookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);
        return booking.Id;
    }
}
