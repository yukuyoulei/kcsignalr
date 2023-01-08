using Microsoft.AspNetCore.SignalR;

namespace webserver
{
    public record TransData(string Id, byte[] buffer);
    public interface IHubClient
    {
        Task ReceiveMessage(TransData data);
        Task myMessage(string u, string m);
    }

    public class SignalREntry : Hub<IHubClient>
    {
        private readonly static Dictionary<string, string> _connections = new();
        public void AddUser(string name)
        {
            string cid = GetConnectionId();
            Console.WriteLine($"AddUser {cid}, name {name}");
            if (!_connections.ContainsKey(cid))
            {
                _connections.Add(cid, name);
            }
            Clients.All.ReceiveMessage(new TransData(cid, System.Text.Encoding.UTF8.GetBytes("hello")));
        }
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
        public override Task OnConnectedAsync()
        {
            var session = GetConnectionId();
            Console.WriteLine($"OnConnectedAsync {session}");
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var session = GetConnectionId();
            Console.WriteLine($"OnDisconnectedAsync {session}");
            if (_connections.ContainsKey(session))
                _connections.Remove(session);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
