using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Application.ElasticSearch;

internal interface IAddressLocationSearch
{
    Task IndexAsync(AddressLocation entity, CancellationToken ct);
    Task<IReadOnlyList<AddressLocationDocument>> SearchAsync(string term, int size, CancellationToken ct);
}