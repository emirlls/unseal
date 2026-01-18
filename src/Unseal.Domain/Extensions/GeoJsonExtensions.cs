using System.Linq;
using System.Text.Json;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;

namespace Unseal.Extensions;

public static class GeoJsonExtensions
{
    private static readonly JsonSerializerOptions _options;

    static GeoJsonExtensions()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new GeoJsonConverterFactory());
    }

    extension(string geoJson)
    {
        public FeatureCollection? ToFeatureCollection()
        {
            if (string.IsNullOrWhiteSpace(geoJson)) return null;
            var featureCollection = JsonSerializer.Deserialize<FeatureCollection>(geoJson, _options);
            return featureCollection;
        }

        public Geometry? ToGeomFromGeoJson()
        {
            if (string.IsNullOrWhiteSpace(geoJson)) return null;
            var geometry = JsonSerializer.Deserialize<FeatureCollection>(geoJson, _options)!
                .FirstOrDefault()
                ?.Geometry;
            return geometry;
        }
    }

    public static string? ToGeoJson(this Geometry? geometry)
    {
        if (geometry is null) return null;
        var geoJson = JsonSerializer.Serialize(geometry, _options);
        return geoJson;
    }
}