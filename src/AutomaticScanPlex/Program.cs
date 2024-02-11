using AutomaticScanPlex.Configurations;
using AutomaticScanPlex.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Automatic Scan Plex";
});

builder.Services.AddHostedService<AutomaticScanPlexWindowsService>();
builder.Services.AddSingleton<IPlexService, PlexService>();

var plexOptions = builder.Configuration.GetSection(PlexOptions.Plex);

builder.Services.AddOptions<PlexOptions>()
    .Bind(plexOptions)
    .ValidateDataAnnotations()
    .ValidateOnStart();

string? sectionUrlBase = plexOptions.GetSection("SectionsUrlBase").Value;

if (string.IsNullOrWhiteSpace(sectionUrlBase))
{
    Console.WriteLine("Set an section url base in app config");
    return;
}

builder.Services.AddHttpClient(
    "PlexSections",
    client =>
    {
        client.BaseAddress = new Uri(sectionUrlBase);
    });

var host = builder.Build();
host.Run();
