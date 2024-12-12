using Microsoft.Extensions.Logging;
using QueueDisplayApp.Services;
using System.Windows;

namespace QueueDisplayApp;

public partial class MainWindow : Window
{
    private readonly IQueueService _queueService;
    private readonly ITcpService _tcpService;
    private readonly IRectangleDrawingService _rectangleDrawingService;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(
        IQueueService queueService,
        ITcpService tcpService,
        IRectangleDrawingService rectangleDrawingService,
        ILogger<MainWindow> logger)
    {
        InitializeComponent();

        _queueService = queueService;
        _tcpService = tcpService;
        _rectangleDrawingService = rectangleDrawingService;
        _logger = logger;

        _tcpService.MessageReceived += OnMessageReceived;
        StartServer();
    }

    private async void StartServer()
    {
        await _tcpService.StartServerAsync();
    }

    private void OnMessageReceived(object? sender, string message)
    {
        Dispatcher.Invoke(() => ProcessMessage(message));
    }

    private void ProcessMessage(string message)
    {
        var parts = message.Split(' ');

        if (parts.Length < 1)
        {
            return;
        }

        var command = parts[0];

        switch (command)
        {
            case "ADD" when parts.Length >= 2:
                _queueService.AddElement(parts[1] switch
                {
                    "green" => ElementStatus.Suitable,
                    "yellow" => ElementStatus.Marriage,
                    _ => throw new InvalidOperationException("Unknown color")
                });
                break;

            case "REMOVE":
                _queueService.RemoveElement();
                break;

            default:
                _logger.LogError("Unknown command");
                break;
        }

        UpdateQueueDisplay();
    }


    private void UpdateQueueDisplay()
    {
        _rectangleDrawingService.DrawRectangles(QueueCanvas, _queueService.GetElements());
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        _tcpService.StopServer();
        base.OnClosing(e);
    }
}
