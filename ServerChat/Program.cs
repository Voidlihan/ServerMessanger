using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerChat
{
    class Program
    {
        static void Main(string[] args)
        {
            string ipaddress = "192.168.1.19";
            int port = 3231;
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ipaddress), port);
            var messages = new List<Message>();
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(ipPoint);
            listenSocket.Listen(10);
            while (true)
            {
                Socket handler = listenSocket.Accept();
                StringBuilder builder = new StringBuilder();
                int bytes = 0; 
                byte[] data = new byte[8192];
                bytes = handler.Receive(data);
                builder.Append(System.Text.Encoding.Unicode.GetString(data, 0, bytes));
                while (handler.Available > 0);
                var messageJson = builder.ToString().Substring(4);
                var message = JsonConvert.DeserializeObject<Message>(messageJson);
                messages.Add(message);
                Console.WriteLine($"{message.Text}");
                var messagesJson = JsonConvert.SerializeObject(messages);
                var sendData = System.Text.Encoding.Unicode.GetBytes(messagesJson);
                Socket sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sendSocket.Connect(ipPoint);
                sendSocket.Send(sendData);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}
