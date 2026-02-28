using MediatR;

namespace Nexticz.Module.Sign.Settings.Application;

internal interface ISettingsCommand<out TResponse> : IRequest<TResponse>;