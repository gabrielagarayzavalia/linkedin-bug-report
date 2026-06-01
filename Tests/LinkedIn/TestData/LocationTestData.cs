using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cabaVsPBA.Tests.LinkedIn.TestData;

public sealed class LocationTestDataEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("tcIds")]
    public List<string> TcIds { get; set; } = [];

    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("queryRepeatChar")]
    public string? QueryRepeatChar { get; set; }

    [JsonPropertyName("queryRepeatCount")]
    public int? QueryRepeatCount { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("expectedJurisdiction")]
    public string? ExpectedJurisdiction { get; set; }

    [JsonPropertyName("expectedLabelEn")]
    public string? ExpectedLabelEn { get; set; }

    [JsonPropertyName("expectedLabelEs")]
    public string? ExpectedLabelEs { get; set; }

    [JsonPropertyName("assertionType")]
    public string AssertionType { get; set; } = "typeahead";

    [JsonPropertyName("placeName")]
    public string? PlaceName { get; set; }

    [JsonPropertyName("alsoExpectPba")]
    public bool AlsoExpectPba { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    public string ResolveQuery()
    {
        if (!string.IsNullOrEmpty(QueryRepeatChar) && QueryRepeatCount is > 0)
        {
            return new string(QueryRepeatChar[0], QueryRepeatCount.Value);
        }

        return Query;
    }
}

public static class LocationTestData
{
    private static readonly Lazy<IReadOnlyList<LocationTestDataEntry>> Cache = new(LoadInternal);

    public static string JsonPath { get; } =
        Path.Combine(LinkedInPaths.ProjectRoot, "docs", "test-data", "location-test-data.json");

    public static IReadOnlyList<LocationTestDataEntry> Load() => Cache.Value;

    public static LocationTestDataEntry GetByTdId(string tdId) =>
        Load().FirstOrDefault(e => e.Id.Equals(tdId, StringComparison.OrdinalIgnoreCase))
        ?? throw new InvalidOperationException($"TD no encontrado: {tdId}");

    public static LocationTestDataEntry GetByTcId(string tcId) =>
        Load().FirstOrDefault(e => e.TcIds.Any(t =>
            t.Equals(tcId, StringComparison.OrdinalIgnoreCase)))
        ?? throw new InvalidOperationException($"TC no encontrado en test data: {tcId}");

    private static IReadOnlyList<LocationTestDataEntry> LoadInternal()
    {
        if (!File.Exists(JsonPath))
        {
            throw new FileNotFoundException($"No se encontró test data: {JsonPath}");
        }

        var json = File.ReadAllText(JsonPath);
        return JsonSerializer.Deserialize<List<LocationTestDataEntry>>(json)
               ?? throw new InvalidOperationException("Test data vacío o inválido.");
    }
}
