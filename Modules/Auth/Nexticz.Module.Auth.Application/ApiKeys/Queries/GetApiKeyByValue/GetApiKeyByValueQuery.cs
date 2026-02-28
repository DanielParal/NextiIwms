using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Domain.AppUserApiKeys;

namespace Nexticz.Module.Auth.Application.ApiKeys.Queries.GetApiKeyByValue;

public record GetApiKeyByValueQuery(string Value) : IRequest<ErrorOr<AppUserApiKey?>>;