using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Nexticz.Module.Cuzk.Application.ElasticSearch;
using Nexticz.Module.Cuzk.Domain.AddressLocationAggregate;

namespace Nexticz.Module.Cuzk.Infrastructure.ElasticSearch;

internal sealed class AddressLocationSearch(ElasticsearchClient client) : IAddressLocationSearch
{
    public async Task IndexAsync(AddressLocation entity, CancellationToken ct)
    {
        var doc = new AddressLocationDocument(
            AdmCode: entity.AdmCode,
            Slug: entity.Slug
        );

        // var response = await client.IndexAsync(doc, d => d
        //     .Index(ElasticsearchSetup.IndexName)
        //     .Id(entity.AdmCode), ct);

        var response = await client.UpdateAsync<AddressLocationDocument, AddressLocationDocument>(
            index: ElasticsearchSetup.IndexName,
            id: entity.AdmCode,
            u => u
                .Doc(doc)
                .DocAsUpsert(true),
                ct
            );
        
        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException($"Elasticsearch indexing failed for AdmCode={entity.AdmCode}: {response.ElasticsearchServerError?.Error?.Reason}");
        }
    }
    
    public async Task<IReadOnlyList<AddressLocationDocument>> SearchAsync(string term, int size, CancellationToken ct)
    {
        var response = await client.SearchAsync<AddressLocationDocument>(s => s
            .Index(ElasticsearchSetup.IndexName)
            .Size(size)
            .Query(q => q
                .Bool(b => b
                    .Should(
                        mq => 
                            mq.Match(
                                m => 
                                    m.Field(f => f.Slug)
                                        .Query(term).Operator(Operator.And))
                    )
                    .MinimumShouldMatch(1)
                )
            ), ct);

        if (!response.IsValidResponse) 
            return [];
        
        return response.Hits.Select(h => h.Source!).ToList();
    }
}