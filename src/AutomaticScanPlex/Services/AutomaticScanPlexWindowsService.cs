namespace AutomaticScanPlex.Services;

public class AutomaticScanPlexWindowsService : BackgroundService
{
    private readonly ILogger<AutomaticScanPlexWindowsService> _logger;
    private readonly IPlexService _plexService;
    private readonly List<FileSystemWatcher> _watchers;

    public AutomaticScanPlexWindowsService(
        ILogger<AutomaticScanPlexWindowsService> logger,
        IPlexService plexService)
    {
        _logger = logger;
        _plexService = plexService;
        _watchers = new List<FileSystemWatcher>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(60_000, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker starts at {time}", DateTimeOffset.Now);
        var sections = _plexService.GetSectionsAsync(cancellationToken).Result;

        foreach (var path in sections.Select(s => s.Path))
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
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File {file} deleted at {time}", e.Name, DateTimeOffset.Now);
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File {file} created at {time}", e.Name, DateTimeOffset.Now);
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File {file} changed {changeType} at {time}", e.Name, e.ChangeType, DateTimeOffset.Now);
    }

    private void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            _logger.LogError("Worker error at {time}: {message}", DateTimeOffset.Now, ex.Message);
            PrintException(ex.InnerException);
        }
    }
}
