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

            ThreadPool.QueueUserWorkItem(CheckTimeOut);
        }


        /// <summary>
        /// 缓存字节
        /// </summary>
        private byte[] cacheBytes = new byte[1024 * 1024];
        private int cacheNum;

        /// <summary>
        /// 上一次收到消息的时间
        /// </summary>
        private long frontTime=-1;
        private static int TIME_OUT_TIME = 7;
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
            if (Connected)
            {

                try
                {
                    socket.Send(msg.Writing());

                }
                catch (Exception e)
                {
                    Console.WriteLine("发送出错：" + e.Message);

                }

            }
            else
                Program.socket.AddDelSocket(this);
        }

        public void Receive()
        {
            if (!Connected)
            {
                Program.socket.AddDelSocket(this);

                return;
            }

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
                    Program.socket.AddDelSocket(this);

                }

            }
        }
        /// <summary>
        /// 超时检测
        /// </summary>
        private void CheckTimeOut(object obj)
        {
            while (Connected)
            {
                if (frontTime != -1 && DateTime.Now.Ticks / TimeSpan.TicksPerSecond - frontTime >= TIME_OUT_TIME)
                {
                    Program.socket.AddDelSocket(this);
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        private void HandleMsg(object obj)
        {
            BaseMsg msg = obj as BaseMsg;
            Console.WriteLine(msg.GetType().Name);
            switch (msg.GetType().Name)
            {
                case "PlayerMsg":
                    PlayerMsg pm = msg as PlayerMsg;
                    Console.WriteLine("收到消息:" + pm.playerID);
                    Console.WriteLine("收到消息:" + pm.playerData.name);

                    break;
                case "QuitMsg":
                    Program.socket.AddDelSocket(this);

                    break;
                case "HeartMsg":
                    //收到心跳消息，记录时间
                    //DateTime.Now.Ticks系统Ticks时间除以一千万得到系统的秒
                    frontTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                    break;
                default:
                    break;
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
                    BaseMsg msg = null;
                    switch (msgID)
                    {
                        case 1001:
                            msg = new PlayerMsg();
                            msg.Reading(cacheBytes, nowIndex);
                            break;
                        case 9999:
                            msg = new QuitMsg();
                            break;
                        case 8888:
                            msg = new HeartMsg();
                            Console.WriteLine("心跳");
                            break;
                    }
                    if (msg != null)
                        ThreadPool.QueueUserWorkItem(HandleMsg, msg);
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
