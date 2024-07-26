using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LearnTCPServerAsync
{
    class ServerSocket
    {
        private Socket socket;
        private Dictionary<int, ClientSocket> clientDic = new Dictionary<int, ClientSocket>();

        public void Start(string ip,int port,int num)
        {
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                socket.Bind(ipPoint);
                socket.Listen(num);

                socket.BeginAccept(AcceptCallBack,null);
            }
            catch (SocketException e)
            {
                Console.WriteLine("服务器创建出错"+e.Message);
            }
        }


        private void AcceptCallBack(IAsyncResult result)
        {
            try
            {
                Socket clientSocket = socket.EndAccept(result);
                ClientSocket client = new ClientSocket(clientSocket);

                Console.WriteLine("客户端连入成功" + client.clientID);


                clientDic.Add(client.clientID, client);

                socket.BeginAccept(AcceptCallBack, null);

            }
            catch (SocketException e)
            {

                Console.WriteLine("客户端连入失败" + e.Message);
            }
        }

        public void Broadcast(string str)
        {
            foreach (ClientSocket client in clientDic.Values)
            {
                client.Send(str);
            }
        }
    }
}
