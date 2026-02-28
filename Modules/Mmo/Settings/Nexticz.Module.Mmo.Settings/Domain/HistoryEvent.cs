using JasperFx.Events;
using Nexticz.Lib.Shared.DataAccess.Marten.Models;

namespace Nexticz.Module.Mmo.Settings.Domain;

internal class HistoryEvent(IEvent rawEvent) : EventResponse(rawEvent);