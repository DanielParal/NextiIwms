using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexticz.Module.Cuzk.Infrastructure.ElasticSearch;

public static class ElasticsearchSetup
{
    public const string IndexName = "cuzk_addresslocations";

    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var uriFromSettings = configuration["Elasticsearch:Uri"];
        
        if (string.IsNullOrWhiteSpace(uriFromSettings)) 
            throw new InvalidOperationException("Elasticsearch Uri not found in configuration.");
        
        var uri = new Uri(uriFromSettings);

        var settings = new ElasticsearchClientSettings(uri)
            .DefaultIndex(IndexName);

        var client = new ElasticsearchClient(settings);
        services.AddSingleton(client);
        services.AddHostedService<ElasticsearchIndexInitializer>();
    }

    internal static async Task EnsureIndexAsync(ElasticsearchClient client)
    { 
        var exists = await client.Indices.ExistsAsync(IndexName);
        if (exists.Exists) 
            return;

        var create = await client.Indices.CreateAsync(IndexName, c => c
            .Settings(s => s
                .MaxNgramDiff(19)
                .Analysis(a => a
                    .TokenFilters(tf => tf
                        .NGram("infix_ngrams", ng => ng.MinGram(1).MaxGram(20))
                    )
                    .Analyzers(an => an
                        .Custom("infix_analyzer", ca => ca
                            .Tokenizer("standard")
                            .Filter("lowercase", "asciifolding", "infix_ngrams")
                        )
                        .Custom("infix_search_analyzer", ca => ca
                            .Tokenizer("standard")
                            .Filter("lowercase", "asciifolding")
                        )
                    )
                )
            )
            .Mappings(m => m
                .Properties(p =>  p
                    .Keyword("admCode")
                    .Text("slug", t => t
                        .Analyzer("infix_analyzer")
                        .SearchAnalyzer("infix_search_analyzer")
                    )
                )
            )
        );

        if (!create.IsValidResponse)
        {
            throw new InvalidOperationException("Elasticsearch index creation failed");
        }
    }
}