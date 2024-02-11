using AutomaticScanPlex.Models;

namespace AutomaticScanPlex.Services;

public interface IPlexService
{
    Task<List<Section>> GetSectionsAsync(CancellationToken ct);
    Task<bool> RefreshSection(Section section, CancellationToken ct);
}