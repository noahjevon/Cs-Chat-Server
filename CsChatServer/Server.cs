using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CsChatServer
{
    class Server
    {
        // add array to store current users
        private partial class UserList
        {
            public static List<string> Usernames = new List<string>();
        }

        private class Connect : WebSocketBehavior
        {
            private string _name;
            
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data + " connected");
                _name = e.Data;
                Sessions.Broadcast("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + "Welcome " + e.Data + " to the server!");
            }
            
            protected override void OnClose(CloseEventArgs e)
            {
                Sessions.Broadcast("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + _name + " has left.");
            }
        }

        private class Usernames : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                UserList.Usernames.Add(e.Data);
                Sessions.Broadcast(string.Join("\n", UserList.Usernames));
            }
            
            protected override void OnClose(CloseEventArgs e)
            {
                Sessions.Broadcast(string.Join("\n", UserList.Usernames));
            }
        }

        private class Disconnect : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data + " disconnected");
                Sessions.Broadcast("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data + " has left.");
                
                UserList.Usernames.Remove(e.Data);
            }
        }

        private class SendMessage : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data);
                Sessions.Broadcast("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e.Data);
            }
        }

        private static void RunServer()
        {
            var server = new WebSocketServer("ws://127.0.0.1:80");
            server.AddWebSocketService<Connect>("/Connect");
            server.AddWebSocketService<SendMessage>("/Message");
            server.AddWebSocketService<Usernames>("/Usernames");
            server.AddWebSocketService<Disconnect>("/Disconnect");
            
            server.Start();
            Console.WriteLine("Server started...");
            Console.WriteLine("Waiting for connections...");
            
            
            Console.ReadKey(true);
        }
        
        public static void Main(string[] args)
        {
            RunServer();
        }
    }
}