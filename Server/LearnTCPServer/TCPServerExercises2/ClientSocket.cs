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


        /// <summary>
        /// 缓存字节
        /// </summary>
        private byte[] cacheBytes = new byte[1024 * 1024];
        private int cacheNum;


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
                        HandleReceiveMsg(result,receiveNum);
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

        /// <summary>
        /// 处理接受消息 分包、黏包问题的方法
        /// </summary>
        /// <param name="receiveBytes">接收的字节流</param>
        /// <param name="receiveNum">字节流长度</param>
        private void HandleReceiveMsg(byte[] receiveBytes, int receiveNum)
        {
            int msgID = 0;
            int msgLength = 0;
            int nowIndex = 0;//当前访问索引

            //收到消息时将消息记入缓存区
            receiveBytes.CopyTo(cacheBytes, cacheNum);
            cacheNum += receiveNum;

            while (true)
            {
                //每次将长度设置为-1 是避免上一次解析的数据 影响这一次的判断
                msgLength = -1;
                //是否满足一条消息的完整长度
                if (cacheNum - nowIndex >= 8)
                {
                    //解析ID
                    msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                    nowIndex += 4;
                    //解析长度
                    msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                    nowIndex += 4;
                }
                //缓存区的数据足够读取完整的消息体
                if (cacheNum - nowIndex >= msgLength && msgLength != -1)
                {
                    //解析消息体
                    BaseMsg baseMsg = null;
                    switch (msgID)
                    {
                        case 1001:
                            PlayerMsg msg = new PlayerMsg();
                            msg.Reading(cacheBytes, nowIndex);
                            baseMsg = msg;
                            break;
                    }
                    if (baseMsg != null)
                        ThreadPool.QueueUserWorkItem(HandleMsg, baseMsg);
                    nowIndex += msgLength;
                    if (nowIndex == cacheNum) //当缓存区读完时，索引回到头部，相当于清空缓存
                    {
                        cacheNum = 0;
                        break;
                    }
                }
                else //不满足,证明有分包
                {
                    //如果进行了 id和长度的解析 但是 没有成功解析消息体 那么需要减去nowIndex移动的位置
                    if (msgLength != -1)
                        nowIndex -= 8;
                    //把剩余没有解析的字节数组内容 移到前面来 用于缓存下次继续解析
                    Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                    cacheNum = cacheNum - nowIndex;
                    break;
                }
            }


        }

    }
}
