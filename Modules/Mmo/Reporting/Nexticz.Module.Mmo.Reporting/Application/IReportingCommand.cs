using MediatR;

namespace Nexticz.Module.Mmo.Reporting.Application;

internal interface IReportingCommand<out TResponse> : IRequest<TResponse>;