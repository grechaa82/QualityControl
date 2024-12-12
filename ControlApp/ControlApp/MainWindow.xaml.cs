using System.Windows;
using ControlApp.Services;
using Microsoft.Extensions.Logging;

namespace ControlApp;

public partial class MainWindow : Window
{
    private readonly ITcpService _tcpService;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(ITcpService tcpService, ILogger<MainWindow> logger)
    {
        InitializeComponent();

        _tcpService = tcpService;
        _logger = logger;

        ConnectToServer();
    }

    private void ConnectToServer()
    {
        _tcpService.Connect();
    }

    private void CameraButton_Click(object sender, RoutedEventArgs e)
    {
        string color = GreenToggle.IsChecked == true ? "green" : "yellow";
        _tcpService.Send($"ADD {color}");
    }

    private void PusherButton_Click(object sender, RoutedEventArgs e)
    {
        _tcpService.Send("REMOVE");
    }

    private void ReconnectButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _tcpService.Reconnect();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось переподключиться. Ошибка: {ex.Message}");
        }
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        _tcpService.Disconnect();
        base.OnClosing(e);
    }
}
