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
builder.Services.Configure<PlexOptions>(plexOptions);
string? defaultSectionUrlBase = plexOptions.GetSection("SectionsUrlBase").Value;

builder.Services.AddHttpClient(
    "PlexSections",
    client =>
    {
        client.BaseAddress = new Uri(defaultSectionUrlBase);
    });

var host = builder.Build();
host.Run();
