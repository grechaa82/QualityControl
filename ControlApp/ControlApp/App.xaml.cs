using ControlApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;

namespace ControlApp;

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
                    .AddTransient<ITcpService, TcpService>(serviceProvider =>
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<TcpService>>();
                        return new TcpService(ipAddres, port, logger);
                    })
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
