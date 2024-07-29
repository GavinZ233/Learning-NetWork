using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetWorkData;
using System.Net.Sockets;
using System.Net;

namespace TCPServerAsyncExercises
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

            try
            {
                serverSocket.Bind(ipPoint);
                serverSocket.Listen(num);

                serverSocket.BeginAccept(AcceptCallBack, null);
            }
            catch (SocketException e)
            {
                Console.WriteLine("服务器创建出错" + e.Message);
            }
        }
        private void AcceptCallBack(IAsyncResult asyncResult)
        {

                try
                {
                    Socket clientSocket = serverSocket.EndAccept(asyncResult);
                    ClientSocket client = new ClientSocket(clientSocket);
                        clientDic.Add(client.clientID, client);

                    serverSocket.BeginAccept(AcceptCallBack, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine("客户端接入报错：" + e.Message);
                }
            
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

