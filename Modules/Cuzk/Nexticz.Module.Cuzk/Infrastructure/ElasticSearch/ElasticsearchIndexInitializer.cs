using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Hosting;

namespace Nexticz.Module.Cuzk.Infrastructure.ElasticSearch;

internal class ElasticsearchIndexInitializer(ElasticsearchClient client) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ElasticsearchSetup.EnsureIndexAsync(client);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}