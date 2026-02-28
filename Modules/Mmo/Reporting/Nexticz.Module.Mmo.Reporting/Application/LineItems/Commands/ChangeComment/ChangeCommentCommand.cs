using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Commands.ChangeComment;

public record ChangeCommentCommand(Guid Id, string Comment) : IReportingCommand<ErrorOr<LineItemView>>;