using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LearnTCPServerAsync
{
    class ClientSocket
    {
        public Socket socket;
        public int clientID;
        private static int CLIENT_BEGIN_ID = 1;

        private byte[] cacheBytes = new byte[1024];
        private int cacheNum = 0;
        public ClientSocket(Socket socket)
        {
            this.socket = socket;
            clientID = CLIENT_BEGIN_ID++;

            this.socket.BeginReceive(cacheBytes, cacheNum, cacheBytes.Length, SocketFlags.None, ReceiveCallBack, socket);
        }

        private void ReceiveCallBack(IAsyncResult asyncResult)
        {
            try
            {
                cacheNum = socket.EndReceive(asyncResult);
                Console.WriteLine(Encoding.UTF8.GetString(cacheBytes,0,cacheNum));
                cacheNum = 0;
                if (socket.Connected)
                {
                    socket.BeginReceive(cacheBytes, cacheNum, cacheBytes.Length, SocketFlags.None, ReceiveCallBack, socket);
                }
                else
                {
                    Console.WriteLine("没有连接");

                }
            }
            catch (SocketException e)
            {

                Console.WriteLine("接收出错" + e.Message);
            }
        }

        public void Send(string str)
        {
            if (socket.Connected)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendCallBack, null);
            }
        }

        private void SendCallBack(IAsyncResult result)
        {
            try
            {
                socket.EndSend(result);
            }
            catch (SocketException e)
            {
                Console.WriteLine("发送失败"+e.Message);


            }
        }
    }
}
