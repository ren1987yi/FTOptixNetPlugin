using FTOptixNetPlugin.NetServer;
using FTOptixNetPlugin.NetServer.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SampleCode.WebSocketTest
{

    





    internal class ChatSession_Server : WsSession
    {
        public ChatSession_Server(WsServer server) : base(server)
        {
        }

      

        protected override void OnReceivedRequestHeader(HttpRequest request)
        {

            
            //if (this.WebSocket.WsHandshaked)
            //    return;

            // Try to perform WebSocket upgrade
            //if (!WebSocket.PerformServerUpgrade(request, Response))
            //{
            //    base.OnReceivedRequestHeader(request);
            //    return;
            //}

            this.Close();

            //base.OnReceivedRequestHeader(request);
        }

        


        public override void OnWsConnected(HttpRequest request)
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} connected!");

            // Send invite message
            string message = "Hello from WebSocket chat! Please send a message or '!' to disconnect the client!";
            SendTextAsync(message);
        }

        public override void OnWsDisconnected()
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} disconnected!");
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            // Multicast message to all connected sessions
            ((WsServer)Server).MulticastText("loopback:" +message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
                Close(1000);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket session caught an error with code {error}");
        }

    }


    internal class ChatServer : WsServer
    {
        public ChatServer(IPAddress address, int port) : base(address, port)
        {
        }


        protected override TcpSession CreateSession()
        {
            //return base.CreateSession();
            return new ChatSession_Server(this);
        }



        protected override void OnError(SocketError error)
        {
            base.OnError(error);
        }
    }
}
