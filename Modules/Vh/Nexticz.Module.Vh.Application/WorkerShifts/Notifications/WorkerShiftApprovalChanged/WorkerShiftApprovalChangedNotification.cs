using MediatR;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Notifications.WorkerShiftApprovalChanged;

public record WorkerShiftApprovalChangedNotification(Guid Id, bool Approval) : INotification;