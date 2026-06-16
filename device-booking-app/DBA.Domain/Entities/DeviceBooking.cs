using DBA.Domain.Enums;
using DBA.Domain.Exceptions;

namespace DBA.Domain.Entities;

public class DeviceBooking
{
    public Guid Id { get; private set; }
    public string DeviceName { get; private set; } = default!;
    public string Applicant { get; private set; } = default!;
    public DateTimeOffset ExpectedReturn { get; private set; }
    public DateTimeOffset? ActualReturn { get; private set; }
    public string? Approver { get; private set; }
    public BookingStatus Status { get; private set; }
    public string? Reason { get; private set; }

    private DeviceBooking() { }

    public static DeviceBooking Create(string name, string user, DateTimeOffset date)
    {
        LendDayChecker(DateTimeOffset.UtcNow, date);

        return new DeviceBooking
        {
            Id = Guid.NewGuid(),
            DeviceName = name,
            Applicant = user,
            ExpectedReturn = date,
            ActualReturn = null,
            Approver = null,
            Status = BookingStatus.Pending,
            Reason = null
        };
    }

    public void ToApprove(string approver)
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException("非審核中申請單無法過審");

        Approver = approver;
        Status = BookingStatus.Approved;
    }

    public void ToReject(string approver, string reason)
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException("非審核中申請單無法拒絕");

        Approver = approver;
        Reason = reason;
        Status = BookingStatus.Rejected;
        ActualReturn = DateTimeOffset.UtcNow;
    }

    public void ToReturn()
    {
        if (Status != BookingStatus.Approved)
            throw new DomainException("非借出單據無法歸還");

        Status = BookingStatus.Returned;
    }

    private static void LendDayChecker(DateTimeOffset today, DateTimeOffset expectedReturn)
    {
        if (expectedReturn <= today)
            throw new DomainException("預計歸還時間不可早於今日。");
        if ((expectedReturn - today).TotalDays > 14)
            throw new DomainException("借用時間最多不能超過 14 天。");
    }
}
