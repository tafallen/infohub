using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using uk.me.timallen.infohub;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IRestClientFactory, RestClientFactory>();
        services.AddSingleton<INewsClientWrapper, NewsClientWrapper>();

        services.AddSingleton<INewsService, NewsService>();
        services.AddSingleton<IAccuWeatherService, AccuWeatherService>();
        services.AddSingleton<IHiveHeatingService, HiveHeatingService>();
        services.AddSingleton<ISunriseSunsetService, SunriseSunsetService>();
        services.AddSingleton<IOpenWeatherService, OpenWeatherService>();
    })
    .Build();

host.Run();
