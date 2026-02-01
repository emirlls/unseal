using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Unseal.Constants;

namespace Unseal.Repositories.Base;

public interface IElasticSearchRepository<T, TKey> where T : class
{
    ElasticsearchClient Client { get; }

    Task<bool> CreateIndexAsync(
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    );
    /// <summary>
    /// Creates a document in the specified index.
    /// </summary>
    /// <param name="document">The document object to be indexed.</param>
    /// <param name="indexName">Name of the Elasticsearch index.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>True if the document is successfully indexed; otherwise, false.</returns>
    Task<CreateResponse> CreateDocumentAsync(
        T document,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///  Bulk create documents in the specified index.
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="indexName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BulkResponse> BulkCreateDocumentsAsync(
        IEnumerable<T> documents,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    );
    
    /// <summary>
    ///  Bulk delete documents in the specified index.
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="indexName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BulkResponse> BulkDeleteDocumentsAsync(
        IEnumerable<T> documents,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    );
    
    /// <summary>
    /// Delete a document from the specified index.
    /// </summary>
    /// <param name="id">ID of the document to delete.</param>
    /// <param name="indexName">Name of the Elasticsearch index.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>True if the document is successfully deleted; otherwise, false.</returns>
    Task<DeleteResponse> DeleteDocumentAsync(
        TKey id,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    );
    
    /// <summary>
    /// Counts the number of documents in the specified index.
    /// </summary>
    /// <param name="indexName">Name of the Elasticsearch index.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>The count of documents in the index.</returns>
    Task<long> CountDocumentsAsync(
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves all documents from the specified index.
    /// </summary>
    /// <param name="indexName">Name of the Elasticsearch index.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>A collection of documents from the index.</returns>
    Task<IReadOnlyCollection<T>> GetDocumentsAsync(
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a single document from the specified index by its ID.
    /// </summary>
    /// <param name="id">ID of the document to retrieve.</param>
    /// <param name="indexName">Name of the Elasticsearch index.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>The document object or null if not found.</returns>
    Task<T?> GetDocumentAsync(
        TKey id,
        string indexName = ElasticSearchConstants.DefaultIndex,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get all data in the index.
    /// </summary>
    /// <param name="indexName"></param>
    /// <param name="selector"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyCollection<T>> SearchAsync(
        string indexName = ElasticSearchConstants.DefaultIndex,
        Action<SearchRequestDescriptor<T>>? selector = null,
        CancellationToken cancellationToken = default
    );
}