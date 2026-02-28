using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder.Models;

internal record ShouldHandleNextToLastShiftResult(bool ShouldHandle, Shift? NextToLastShift);