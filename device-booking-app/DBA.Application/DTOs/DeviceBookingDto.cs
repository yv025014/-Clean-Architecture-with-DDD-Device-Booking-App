namespace DBA.Application.DTOs;

public record DeviceBookingDto(
    Guid Id,
    string DeviceName,
    string Applicant,
    DateTimeOffset ExpectedReturn,
    DateTimeOffset? ActualReturn,
    string? Approver,
    int Status,
    string? Reason
);
