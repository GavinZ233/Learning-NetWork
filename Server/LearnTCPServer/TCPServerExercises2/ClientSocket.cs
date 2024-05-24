using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServerExercises2
{
    class ClientSocket
    {
        private static int CLIENT_BEGIN_ID = 1;
        public int clientID;
        public Socket socket;

        public bool Connected => socket.Connected;
        public ClientSocket(Socket s)
        {
            this.clientID=CLIENT_BEGIN_ID;
            this.socket = s;
            ++CLIENT_BEGIN_ID;
        }


        public void Close()
        {
            if (socket!=null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }

        public void Send(string info)
        {
            if (socket!=null)
            {
                try
                {
                    socket.Send(Encoding.UTF8.GetBytes(info));

                }
                catch (Exception e)
                {
                    Console.WriteLine("发送出错："+e.Message);

                }
            }
        }

        public void Receive()
        {
            if (socket !=null)
            {

                try
                {
                    if (socket.Available>0)
                    {
                        byte[] result = new byte[1024];

                        int receiveNum = socket.Receive(result);
                        ThreadPool.QueueUserWorkItem(HandleMsg,
                            Encoding.UTF8.GetString(result, 0, receiveNum));

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("接收出错："+e.Message);
                }

            }
        }

        private void HandleMsg(object obj)
        {
            string str = obj as string;
            Console.WriteLine("收到消息:"+str);
        }
    }
}
