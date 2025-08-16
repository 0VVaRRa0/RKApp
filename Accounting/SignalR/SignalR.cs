using Microsoft.AspNetCore.SignalR.Client;

public class SignalRService
{
    private readonly HubConnection _connection;

    public SignalRService(string hubUrl, Action onRefreshServices)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On("RefreshServices", () =>
        {
            onRefreshServices?.Invoke();
        });
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
    }
}
