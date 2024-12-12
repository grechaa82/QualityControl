using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;

namespace ControlApp.Services;

public class TcpService : ITcpService
{
    private TcpClient? _client;
    private NetworkStream? _stream;

    private readonly int maxAttempts = 5;
    private readonly string _ipAddress;
    private readonly int _port;

    private readonly ILogger<TcpService> _logger;

    public TcpService(string ipAddress, int port, ILogger<TcpService> logger)
    {
        _ipAddress = ipAddress;
        _port = port;
        _logger = logger;

        Connect();
    }

    public void Connect()
    {
        try
        {
            _client = new TcpClient(_ipAddress, _port);
            _stream = _client.GetStream();

            _logger.LogInformation("Connection successful");
        }
        catch (Exception ex)
        {
            _client = null;

            _logger.LogError($"Connection error: {ex.Message}");
        }
    }

    public void Send(string message)
    {
        try
        {
            if (_client == null || !_client.Connected)
            {
                _logger.LogInformation("There is no connection. Reconnecting...");
                Reconnect();
            }

            if (_client == null || !_client.Connected)
            {
                _logger.LogError("Failed to send data - connection not established");
                return;
            }

            byte[] data = Encoding.UTF8.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending data: {ex.Message}");
        }
    }

    public void Reconnect()
    {
        Disconnect();

        var attempt = 0;

        while (attempt < maxAttempts)
        {
            try
            {
                _logger.LogInformation($"Reconnection attempt #{++attempt}...");
                Connect();

                if (_client != null && _client.Connected)
                {
                    _logger.LogInformation("Connection successful");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Reconnection error: {ex.Message}");
                Thread.Sleep(100);
            }
        }

        _logger.LogInformation("Failed to reconnect after several attempts");
    }

    public void Disconnect()
    {
        if (_stream != null)
        {
            _stream.Close();
            _stream = null;
        }

        if (_client != null)
        {
            _client.Close();
            _client = null;
        }
    }
}
