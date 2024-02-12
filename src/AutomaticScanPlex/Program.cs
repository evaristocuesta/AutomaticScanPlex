using AutomaticScanPlex.Configurations;
using AutomaticScanPlex.Services;
using Polly.Extensions.Http;
using Polly;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Automatic Scan Plex";
});

builder.Services.AddHostedService<AutomaticScanPlexService>();
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
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(GetRetryPolicy());

var host = builder.Build();
host.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt));
}
