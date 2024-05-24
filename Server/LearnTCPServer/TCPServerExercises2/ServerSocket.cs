using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServerExercises2
{
    class ServerSocket
    {
        public Socket serverSocket;
        public Dictionary<int, ClientSocket> clientDic = new Dictionary<int, ClientSocket>();

        bool isClose;
        public void Start(string ip,int port,int num)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            serverSocket.Bind(ipPoint);
            serverSocket.Listen(num);
            ThreadPool.QueueUserWorkItem(AcceptClient);
            ThreadPool.QueueUserWorkItem(ReceiveClient);

        }

        public void Close()
        {
            isClose = true;

            foreach (ClientSocket item in clientDic.Values)
            {
                item.Close();
            }

            clientDic.Clear();

            serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket.Close();
            serverSocket = null;    
        }

        public void Broadcast(string str)
        {
            foreach (ClientSocket item in clientDic.Values)
            {
                item.Send(str);
            }
        }

        private void AcceptClient(object obj)
        {
            while (!isClose)
            {
                try
                {
                    Socket clientSocket = serverSocket.Accept();
                    ClientSocket client = new ClientSocket(clientSocket);
                    client.Send("接入到服务器^v^");
                    clientDic.Add(client.clientID, client);
                }
                catch (Exception e)
                {
                    Console.WriteLine("客户端接入报错："+e.Message);
                }
            }
        }

        private void ReceiveClient(object obj)
        {
            while (!isClose)
            {
                if (clientDic.Count>0)
                {
                    foreach (ClientSocket item in clientDic.Values)
                    {
                        item.Receive();
                    }
                }
            }
        }

    }
}
