using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await ConnectServer();
    }
    public class TransData
    {
        public string Id;
        public byte[] buffer;
    }
    const string url = @"http://127.0.0.1:5000/ws";
    static async Task ConnectServer()
    {
        Debug.Log($"signalr server {url}");

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
        Debug.Log($"连接状态：{hubConnection.State}");

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
        Debug.Log($"td {td.Id}, {System.Text.Encoding.UTF8.GetString(td.buffer)}");
    }

    private static System.Threading.Tasks.Task HubConnection_Closed(Exception arg)
    {
        return Task.Run(() =>
        {
            Debug.Log($"连接断开");
            Debug.Log(arg.Message);
        });
    }

    private static void OnSend(string s1, string s2)
    {
        Debug.Log($"用户名称{s1},消息内容{s2}");
    }
}
