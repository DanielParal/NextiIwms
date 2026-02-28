using MediatR;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationsBySlug;

internal record GetAddressLocationsBySlugQuery(string Slug, int? Take = null) : IRequest<AddressLocation[]>;