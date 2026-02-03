using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Volo.Abp.DependencyInjection;

namespace Unseal.Repositories.Base;

public class ElasticSearchRepository<T, TKey> : IElasticSearchRepository<T, TKey>, ITransientDependency
    where T : class
{
    private readonly Lazy<Task<ElasticsearchClient>> _clientLazy;
    public ElasticsearchClient Client => _clientLazy.Value.GetAwaiter().GetResult();

    public ElasticSearchRepository(IServiceProvider serviceProvider)
    {
        _clientLazy = new Lazy<Task<ElasticsearchClient>>(async () =>
        {
            var client = await GetClientAsync(serviceProvider);
            return client;
        });
    }
    private async Task<ElasticsearchClient> GetClientAsync(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<ElasticsearchClient>();
    }
    public async Task<bool> CreateIndexAsync(
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    )
    {
        var existsResponse = await Client.Indices.ExistsAsync(indexName, cancellationToken);

        if (!existsResponse.Exists)
        {
            var createResponse = await Client.Indices.CreateAsync(indexName, cancellationToken);
            return createResponse.IsValidResponse;
        }

        return existsResponse.Exists;
    }

    public async Task<IndexResponse> CreateDocumentAsync(
        T document,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    )
    {
        var idProperty = document.GetType().GetProperty(ElasticSearchConstants.IdPropertyName);
        var idValue = idProperty?.GetValue(document)?.ToString();

        var response = await Client.IndexAsync(document, indexName, idValue, cancellationToken);
    
        return response;
    }

    public async Task<BulkResponse> BulkCreateDocumentsAsync(
        IEnumerable<T> documents,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    )
    {
        var response = await Client.BulkAsync(b =>
            b.Index(indexName)
                .IndexMany(documents), cancellationToken);
        return response;
    }

    public async Task<BulkResponse> BulkDeleteDocumentsAsync(
        IEnumerable<T> documents,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    )
    {
        var response = await Client.BulkAsync(b => b
                .Index(indexName)
                .DeleteMany(documents),
            cancellationToken);

        return response;
    }

    public async Task<DeleteResponse> DeleteDocumentAsync(
        TKey id,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    )
    {
        var response = await Client.DeleteAsync<T>(id!.ToString(), d => d
            .Index(indexName), cancellationToken);
        return response;
    }

    public async Task<long> CountDocumentsAsync(
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    )
    {
        var countResponse = await Client.CountAsync<T>(c => c
                .Indices(indexName),
            cancellationToken
        );

        return countResponse.Count;
    }

    public async Task<IReadOnlyCollection<T>> GetDocumentsAsync(string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default)
    {
        var response = await Client.SearchAsync<T>(s => s
            .Index(indexName)
            .Query(q => q.MatchAll()), cancellationToken);

        return response.Documents;
    }

    public async Task<T?> GetDocumentAsync(TKey id, string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default)
    {
        var response = await Client.GetAsync<T>(id!.ToString(), g => g
            .Index(indexName), cancellationToken);

        return response.Found ? response.Source : default;
    }

    public async Task<IReadOnlyCollection<T>> SearchAsync(
        string indexName = ElasticSearchConstants.DefaultIndex,
        Action<SearchRequestDescriptor<T>>? selector = null,
        CancellationToken cancellationToken = default
    )
    {
        var response = await Client.SearchAsync<T>(s => 
        {
            s.Index(indexName).Size(ElasticSearchConstants.ElasticPageSize);
        
            if (selector != null)
            {
                selector(s);
            }
            else
            {
                s.Query(q => q.MatchAll());
            }
        }, cancellationToken);

        return response.Documents;
    }
}