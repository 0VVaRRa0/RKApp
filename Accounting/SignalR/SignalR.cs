using Microsoft.AspNetCore.SignalR.Client;

public class SignalRService
{
    private readonly HubConnection _connection;

    public SignalRService(string hubUrl, Action onRefreshServices, Action onRefreshInvoices, Action onRefreshClients)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On("RefreshServices", () =>
        {
            onRefreshServices?.Invoke();
        });
        _connection.On("RefreshClients", () =>
        {
            onRefreshClients?.Invoke();
        });
        _connection.On("RefreshInvoices", () =>
        {
            onRefreshInvoices?.Invoke();
        });
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
    }
}
