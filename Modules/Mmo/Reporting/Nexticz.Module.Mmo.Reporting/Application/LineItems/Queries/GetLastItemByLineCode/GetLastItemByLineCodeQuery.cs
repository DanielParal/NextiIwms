using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Queries.GetLastItemByLineCode;

internal record GetLastItemByLineCodeQuery(string LineCode) : IRequest<LastItemPerLineView?>;