using AutomaticScanPlex.Configurations;
using AutomaticScanPlex.Converters;
using AutomaticScanPlex.Dtos;
using AutomaticScanPlex.Models;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;

namespace AutomaticScanPlex.Services;

public class PlexService : IPlexService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PlexOptions _plexOptions;

    public PlexService(
        IHttpClientFactory httpClientFactory,
        IOptions<PlexOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _plexOptions = options.Value;
    }

    public async Task<List<Section>> GetSectionsAsync(CancellationToken ct)
    {
        var httpClient = _httpClientFactory.CreateClient("PlexSections");
        var response = await httpClient.GetAsync($"?X-Plex-Token={_plexOptions.Token}", ct);

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<Section>().ToList();
        }

        var xml = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(xml))
        {
            return Enumerable.Empty<Section>().ToList();
        }

        var serializer = new XmlSerializer(typeof(MediaContainerDto));
        using var reader = new StringReader(xml);
        var result = (MediaContainerDto?)serializer.Deserialize(reader);
        var converter = new SectionsConverter();
        return converter.Convert(result);
    }
}
