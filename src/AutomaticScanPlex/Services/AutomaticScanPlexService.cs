using AutomaticScanPlex.Models;
using System.Collections.Concurrent;

namespace AutomaticScanPlex.Services;

public class AutomaticScanPlexService : BackgroundService
{
    private readonly ILogger<AutomaticScanPlexService> _logger;
    private readonly IPlexService _plexService;
    private readonly Dictionary<long, FileSystemWatcher> _watchers;
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
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CreateWatchersSectionsAsync(stoppingToken);
                await Task.Delay(60_000, stoppingToken);
                await RefreshSectionsToUpdate(stoppingToken);
            }
        }
        catch ( OperationCanceledException)
        {
            // When the stopping token is cancelled
        }
        catch ( Exception ex ) 
        {
            _logger.LogError(ex, "Error {message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.

            Environment.Exit(1);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker starts at {time}", DateTimeOffset.Now);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stops at {time}", DateTimeOffset.Now);
        RemoveWatchers();
        return base.StopAsync(cancellationToken);
    }

    private async Task CreateWatchersSectionsAsync(CancellationToken ct)
    {
        RemoveWatchers();
        _sections = await _plexService.GetSectionsAsync(ct);

        if (_sections.Count == 0)
        {
            _logger.LogWarning("Cannot get sections from Plex at {time}", DateTimeOffset.Now);
            return;
        }

        _watchers.Clear();

        foreach (var section in _sections)
        {
            if (string.IsNullOrEmpty(section.Path))
            {
                continue;
            }

            _logger.LogInformation("Worker starts watching {path} at {time}", section.Path, DateTimeOffset.Now);

            var watcher = new FileSystemWatcher(section.Path);
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            _watchers.Add(section.Id, watcher);
        }
    }

    private async Task RefreshSectionsToUpdate(CancellationToken stoppingToken)
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
    }

    private void RemoveWatchers()
    {
        foreach (var watcher in _watchers.Values)
        {
            _logger.LogInformation("Worker stops watching {path} at {time}", watcher.Path, DateTimeOffset.Now);
            watcher.Dispose();
        }
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
