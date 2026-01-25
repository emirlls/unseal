using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Unseal.Constants;

namespace Unseal.Extensions;

public static class CacheExtension
{
    public static string GenerateCacheKey(this string name)
    {
        return
            $"{AppConstants.AppName}_{name}";
    }
    public static string GenerateCacheKeyToCount(
        CultureInfo cultureInfo,
        Guid? currentUserId,
        string? listName
    )
    {
        return
            $"{AppConstants.AppName}:{cultureInfo}:{currentUserId}:{listName}_Count";
    }

    public static async Task ClearCacheAsync(
        this IServiceProvider serviceProvider,
        string cacheKey
    )
    {
        var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
        await distributedCache.RemoveAsync(cacheKey);
    }

    public static byte[] CompressData<T>(T model)
    {
        var settings = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            MaxDepth = 10
        };

        var data = JsonSerializer.SerializeToUtf8Bytes(model, settings);
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
            gzip.Write(data, 0, data.Length);
        return output.ToArray();
    }

    public static string DecompressData(byte[] data)
    {
        using var input = new MemoryStream(data);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return Encoding.UTF8.GetString(output.ToArray());
    }

    public static async Task<RedisKey[]> GetMembersAsync(
        this IConnectionMultiplexer redis, 
        string indexKey
    )
    {
        var db = redis.GetDatabase();
        var keys = (await db.SetMembersAsync(indexKey))
            .Select(x => (RedisKey)(string)x!)
            .ToArray();
        return keys;
    }
}