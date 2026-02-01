using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Unseal.Constants;
using Unseal.Models.ElasticSearch;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Users;

public class EsUserRepository : ElasticSearchRepository<UserElasticModel, Guid>, IEsUserRepository
{
    public EsUserRepository(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task AddBlockedUserAsync(Guid targetUserId, Guid blockedByUserId)
    {
        var response = await Client.UpdateAsync<UserElasticModel, object>(
            ElasticSearchConstants.User.UserSearchIndex,
            targetUserId.ToString(),
            u => u.Script(s => s
                .Source(ElasticSearchConstants.Queries.AddIdScript)
                .Params(p => p.Add(ElasticSearchConstants.Queries.NewId, blockedByUserId.ToString()))
            )
        );
    }
    
    public async Task UpdateProfilePictureAsync(Guid userId, string? newUrl)
    {
        var response = await Client.UpdateAsync<UserElasticModel, object>(
            ElasticSearchConstants.User.UserSearchIndex,
            userId.ToString(),
            u => u.Doc(new { ProfilePictureUrl = newUrl })
        );
    }

    public async Task<List<UserElasticModel>> SearchUsersAsync(
        List<Guid>? blockedUserIds,
        Guid currentUserId,
        string searchText,
        int size = 10,
        CancellationToken cancellationToken = default
    )
    {
        var blockedIds = blockedUserIds.IsNullOrEmpty()
            ? new List<Id>()
            : blockedUserIds.Select(id => (Id)id.ToString()).ToList();
        
        var response = await Client.SearchAsync<UserElasticModel>(s => s
                .Index(ElasticSearchConstants.User.UserSearchIndex)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Bool(mBool => mBool
                                .Should(
                                    sh => sh.Match(mt =>
                                        mt.Field(f => f.UserName).Query(searchText).Fuzziness(new Fuzziness(1))),
                                    sh => sh.Prefix(p => p.Field(f => f.UserName).Value(searchText.ToLower()))
                                )
                            )
                        )
                        .MustNot(
                            mn1 => mn1.Term(t => t.Field(f => f.BlockedByUserIds).Value(currentUserId.ToString())),
                            mn2 => mn2.Ids(ids => ids.Values(new Ids(blockedIds)))
                        )
                    )
                )
                .Size(size),
            cancellationToken
        );

        return response.Documents.ToList();
    }
}