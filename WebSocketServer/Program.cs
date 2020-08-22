using System;
using Alchemy;
using Alchemy.Classes;
using System.Collections.Concurrent;

namespace WebSocketServerDemo
{
    class Program
    {
        private static ConcurrentDictionary<string, Connection> OnlineConnections = 
            new ConcurrentDictionary<string, Connection>();

        static void Main(string[] args)
        {
            var aServer = new WebSocketServer(8100, System.Net.IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };

            aServer.Start();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Title = "Websocket Server";

            Console.WriteLine("Running Inox WebSocket Server ...");
            Console.WriteLine("[Type \"exit\" and hit enter to stop the server]");

            var command = string.Empty;
            while (command != "exit")
            {
                command = Console.ReadLine();
            }

            aServer.Stop();

        }

        private static void OnDisconnect(UserContext context)
        {
            Console.WriteLine("Client Disconnected: " + context.ClientAddress);
            Connection conn;
            OnlineConnections.TryRemove(context.ClientAddress.ToString(), out conn);
        }

        private static void OnConnect(UserContext context)
        {
            Console.WriteLine("Client Connected from: " + context.ClientAddress);
            var conn = new Connection { Context = context };
            OnlineConnections.TryAdd(context.ClientAddress.ToString(), conn);
        }

        private static void OnSend(UserContext context)
        {
            Console.WriteLine("Data Sent To: " + context.ClientAddress);
        }

        private static void OnReceive(UserContext context)
        {
            try
            {
                var message = context.DataFrame.ToString();
                var userKey = context.ClientAddress.ToString();

                Console.WriteLine("Data Received [{0}] - {1}", userKey, message);

                if (message.StartsWith("uc:"))
                {
                    string username = message.Split(':')[1] == "" ? "Guest" : message.Split(':')[1];
                    message = " has joined the chat...";
                    OnlineConnections[userKey].Username = username;
                }

                foreach (var connKey in OnlineConnections.Keys)
                {
                    Connection conn = OnlineConnections[connKey];
                    conn.Context.Send(OnlineConnections[userKey].Username + ": " + message);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
