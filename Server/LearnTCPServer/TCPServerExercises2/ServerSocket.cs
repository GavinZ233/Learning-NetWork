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
        /// <summary>
        /// 客户端移除列表
        /// </summary>
        private List<ClientSocket> delList = new List<ClientSocket>();

        bool isClose;
        public void Start(string ip, int port, int num)
        {
            isClose = false;
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

        public void Broadcast(BaseMsg msg)
        {
            lock (clientDic)
            {

                foreach (ClientSocket item in clientDic.Values)
                {
                    item.Send(msg);
                }
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
                        lock (clientDic)
                             clientDic.Add(client.clientID, client);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("客户端接入报错：" + e.Message);
                    }
            }
        }

        private void ReceiveClient(object obj)
        {
            while (!isClose)
            {
                if (clientDic.Count > 0)
                {
                    lock (clientDic)
                    {
                        foreach (ClientSocket item in clientDic.Values)
                        {
                            item.Receive();
                        }
                        //移除无用的客户端
                        CloseDelListSocket();
                    }
                }
            }
        }

        public void CloseDelListSocket()
        {
            for (int i = 0; i < delList.Count; i++)
            {
                CloseClientSocket(delList[i]);
            }
            delList.Clear();
        }

        public void AddDelSocket(ClientSocket socket)
        {
            if (!delList.Contains(socket))
            {
                delList.Add(socket);
            }
        }

        public void CloseClientSocket(ClientSocket socket)
        {
            lock (clientDic)
            {

                socket.Close();
                if (clientDic.ContainsKey(socket.clientID))
                {
                    clientDic.Remove(socket.clientID);
                }
            }
        }

    }
}
