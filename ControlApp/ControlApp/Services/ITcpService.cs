namespace ControlApp.Services;

public interface ITcpService
{
    void Connect();

    void Send(string message);

    void Reconnect();

    void Disconnect();
}
