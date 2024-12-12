namespace QueueDisplayApp.Services;

public interface ITcpService
{
    Task StartServerAsync();

    event EventHandler<string> MessageReceived;

    void StopServer();
}
