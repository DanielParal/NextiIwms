using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application;

internal interface ISettingsCommand<out TResponse> : IRequest<TResponse>;