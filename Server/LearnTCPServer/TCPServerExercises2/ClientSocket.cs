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

        public void Send(BaseMsg msg)
        {
            if (socket!=null)
            {
                try
                {
                    socket.Send(msg.Writing());

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

                        int msgID = BitConverter.ToInt32(result, 0);
                        BaseMsg msg = null;
                        switch (msgID)
                        {
                            case 1001:
                                msg = new PlayerMsg();
                                msg.Reading(result, 4);
                            break;
                        }

                        if (msg == null)
                            return;

                        ThreadPool.QueueUserWorkItem(HandleMsg,msg);

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
            BaseMsg msg = obj as BaseMsg;
            if (msg is PlayerMsg)
            {
                PlayerMsg pm = msg as PlayerMsg;
                Console.WriteLine("收到消息:" + pm.playerID);
                Console.WriteLine("收到消息:" + pm.playerData.name);


            }
        }
    }
}
