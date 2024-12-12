using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueDisplayApp.Services;
using System.IO;
using System.Windows;

namespace QueueDisplayApp;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = LoadConfiguration();

                var ipAddres = configuration["IpAddress"];
                int port = configuration.GetValue<int>("Port");

                services
                    .AddTransient<IQueueService, QueueService>()
                    .AddTransient<ITcpService, TcpService>(serviceProvider =>
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<TcpService>>();
                        return new TcpService(ipAddres, port, logger);
                    })
                    .AddTransient<IRectangleDrawingService, RectangleDrawingService>()
                    .AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }

    private IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false, reloadOnChange: true)
            .Build();
    }
}
