using Microsoft.AspNetCore.SignalR.Client;

class WebClient
{
    public record TransData(string Id, byte[] buffer);
    const string url = @"http://127.0.0.1:5000/ws";
    static async Task Main(string[] args)
    {
        Console.WriteLine($"signalr server {url}");

        HubConnection hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
        hubConnection.On<string, string>("myMessage", (s1, s2) =>
          OnSend(s1, s2)
        );
        hubConnection.On<TransData>("ReceiveMessage", (td) =>
          OnSend(td)
        );
        hubConnection.Closed += HubConnection_Closed;
        await hubConnection.StartAsync();
        Console.WriteLine($"连接状态：{hubConnection.State}");

        await hubConnection.InvokeAsync("AddUser", "Test1");

        var command = Console.ReadLine();
        if (command == "exit")
        {
            await hubConnection.StopAsync();
        }
        Console.ReadKey();
    }

    private static void OnSend(TransData td)
    {
        Console.WriteLine($"td {td.Id}, {System.Text.Encoding.UTF8.GetString(td.buffer)}");
    }

    private static System.Threading.Tasks.Task HubConnection_Closed(Exception arg)
    {
        return Task.Run(() =>
        {
            Console.WriteLine($"连接断开");
            Console.WriteLine(arg.Message);
        });
    }

    private static void OnSend(string s1, string s2)
    {
        Console.WriteLine($"用户名称{s1},消息内容{s2}");
    }
}
