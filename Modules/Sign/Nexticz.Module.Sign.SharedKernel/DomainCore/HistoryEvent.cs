using JasperFx.Events;
using Nexticz.Lib.Shared.DataAccess.Marten.Models;

namespace Nexticz.Module.Sign.SharedKernel.DomainCore;

public class HistoryEvent(IEvent rawEvent) : EventResponse(rawEvent);