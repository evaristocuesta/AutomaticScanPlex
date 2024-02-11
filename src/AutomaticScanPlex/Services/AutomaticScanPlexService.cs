using AutomaticScanPlex.Models;
using System.Collections.Concurrent;

namespace AutomaticScanPlex.Services;

public class AutomaticScanPlexService : BackgroundService
{
    private readonly ILogger<AutomaticScanPlexService> _logger;
    private readonly IPlexService _plexService;
    private readonly List<FileSystemWatcher> _watchers;
    private readonly ConcurrentDictionary<long, Section> _sectionsToUpdate;
    private List<Section> _sections;

    public AutomaticScanPlexService(
        ILogger<AutomaticScanPlexService> logger,
        IPlexService plexService)
    {
        _logger = logger;
        _plexService = plexService;
        _watchers = new();
        _sectionsToUpdate = new();
        _sections = new();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var section in _sectionsToUpdate.Values)
            {
                _logger.LogInformation("Refreshing section {name} at: {time}", section.Name, DateTimeOffset.Now);
                var refreshed = await _plexService.RefreshSection(section, stoppingToken);

                if (!refreshed)
                {
                    _logger.LogError("Error Refreshing section {name} at: {time}", section.Name, DateTimeOffset.Now);
                }
            }

            _sectionsToUpdate.Clear();
            await Task.Delay(60_000, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker starts at {time}", DateTimeOffset.Now);
        _sections = _plexService.GetSectionsAsync(cancellationToken).Result;

        foreach (var path in _sections.Select(s => s.Path))
        {
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            _logger.LogInformation("Worker starts watching {path} at {time}", path, DateTimeOffset.Now);

            var watcher = new FileSystemWatcher(path);
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            _watchers.Add(watcher);
        }

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stops at {time}", DateTimeOffset.Now);

        foreach (var watcher in _watchers)
        {
            _logger.LogInformation("Worker stops watching {path} at {time}", watcher.Path, DateTimeOffset.Now);
            watcher.Dispose();
        }

        return base.StopAsync(cancellationToken);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        PrintException(e.GetException());
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        _logger.LogInformation("File {oldFile} renamed to {file} at {time}", e.OldName, e.Name, DateTimeOffset.Now);
        AddSectionToUpdate(e.FullPath);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File {file} deleted at {time}", e.Name, DateTimeOffset.Now);
        AddSectionToUpdate(e.FullPath);
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File {file} created at {time}", e.Name, DateTimeOffset.Now);
        AddSectionToUpdate(e.FullPath);
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File {file} changed {changeType} at {time}", e.Name, e.ChangeType, DateTimeOffset.Now);
        AddSectionToUpdate(e.FullPath);
    }

    private void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            _logger.LogError("Worker error at {time}: {message}", DateTimeOffset.Now, ex.Message);
            PrintException(ex.InnerException);
        }
    }

    private void AddSectionToUpdate(string path)
    {
        var sectionsToUpdate = _sections.Where(s => s.Path is not null && path.StartsWith(s.Path));

        foreach (var section in sectionsToUpdate)
        {
            _sectionsToUpdate.TryAdd(section.Id, section);
        }
    }
}
