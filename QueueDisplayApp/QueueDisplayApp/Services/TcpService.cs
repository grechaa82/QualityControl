using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;

namespace QueueDisplayApp.Services;

public class TcpService : ITcpService
{
    public event EventHandler<string>? MessageReceived;
    
    private TcpListener? _tcpServer;

    private readonly IPAddress _address;
    private readonly int _port;

    private readonly ILogger<TcpService> _logger;

    public TcpService(string address, int port, ILogger<TcpService> logger)
    {
        // TODO: Check address
        _address = IPAddress.Parse(address);
        _port = port;
        _logger = logger;
    }

    public async Task StartServerAsync()
    {
        try
        {
            _tcpServer = new TcpListener(_address, _port);
            _tcpServer.Start();

            while (true)
            {
                TcpClient client = await _tcpServer.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting TCP server");
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using var stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                MessageReceived?.Invoke(this, message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling client connection");
        }
        finally
        {
            client.Close();
        }
    }

    public void StopServer()
    {
        if (_tcpServer != null)
        {
            _tcpServer.Stop();
        }
    }
}
