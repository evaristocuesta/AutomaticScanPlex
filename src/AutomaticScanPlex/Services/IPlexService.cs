using AutomaticScanPlex.Models;

namespace AutomaticScanPlex.Services;

public interface IPlexService
{
    Task<List<Section>> GetSectionsAsync(CancellationToken ct);
}