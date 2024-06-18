using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NetMgr : MonoBehaviour
{
    private static NetMgr instance;
    public static NetMgr Instance { 
        get
        {
            if (instance==null)
            {
                GameObject obj = new GameObject();
                instance = obj.AddComponent<NetMgr>();
                obj.name = "NetMgr";
            }
            return instance;
        }
    }

    private Socket socket;

    private Queue<BaseMsg> sendMsgQueue = new Queue<BaseMsg>();
    private Queue<BaseMsg> receiveQueue = new Queue<BaseMsg>();


    /// <summary>
    /// 接收字节
    /// </summary>
    private byte[] receiveBytes = new byte[1024*1024];
    private int receiveNum;

    /// <summary>
    /// 缓存字节
    /// </summary>
    private byte[] cacheBytes = new byte[1024 * 1024];
    private int cacheNum;

    private bool isConnected;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (receiveQueue.Count > 0)
        {
            BaseMsg msg = receiveQueue.Dequeue();
            if (msg is PlayerMsg)
            {
                print((msg as PlayerMsg).playerID);
                print((msg as PlayerMsg).playerData.name);

            }

        }

    }
    public void Connect(string ip, int port)
    {
        if (isConnected)
            return;

        if (socket == null)
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        try
        {
            socket.Connect(ipPoint);
            isConnected = true;

            //接收线程
            ThreadPool.QueueUserWorkItem(SendMsg);
            //发送线程
            ThreadPool.QueueUserWorkItem(ReceiveMsg);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
                print("服务器拒绝连接");
            else
                print("连接失败：" + e.ErrorCode);
            return;
        }

    }


    public void Send(BaseMsg info)
    {
        sendMsgQueue.Enqueue(info);
    }

    private void SendMsg(object obj) 
    {
        while (isConnected)
        {
            if (sendMsgQueue.Count>0)
            {
                socket.Send(sendMsgQueue.Dequeue().Writing());
            }
        }
    }

    private void ReceiveMsg(object obj)
    {
        while (isConnected)
        {
            if (socket.Available > 0)
            {
                 receiveNum = socket.Receive(receiveBytes);
                //int msgID = BitConverter.ToInt32(receiveBytes, 0);
                //BaseMsg msg = null;
                //switch (msgID)
                //{
                //    case 1001:
                //        msg = new PlayerMsg();
                //        msg.Reading(receiveBytes, 4);
                //        break;
                   
                //}
                //if (msg == null)
                //    continue;


                //receiveQueue.Enqueue(msg);

            }

        }
    }
    /// <summary>
    /// 处理接受消息 分包、黏包问题的方法
    /// </summary>
    /// <param name="receiveBytes">接收的字节流</param>
    /// <param name="receiveNum">字节流长度</param>
    private void HandleReceiveMsg(byte[] receiveBytes,int receiveNum)
    {
        int msgID = 0;
        int msgLength = 0;
        int nowIndex = 0;

        //收到消息时 应该看看 之前有没有缓存的 如果有的话 我们直接拼接到后面
        receiveBytes.CopyTo(cacheBytes, cacheNum);
        cacheNum += receiveNum;

        while (true)
        {
            //每次将长度设置为-1 是避免上一次解析的数据 影响这一次的判断
            msgLength = -1;
            //处理解析一条消息
            if (cacheNum - nowIndex >= 8)
            {
                //解析ID
                msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
                //解析长度
                msgLength = BitConverter.ToInt32(cacheBytes, nowIndex);
                nowIndex += 4;
            }

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
                    receiveQueue.Enqueue(baseMsg);
                nowIndex += msgLength;
                if (nowIndex == cacheNum)
                {
                    cacheNum = 0;
                    break;
                }
            }
            else
            {
                //如果不满足 证明有分包 
                //那么我们需要把当前收到的内容 记录下来
                //有待下次接受到消息后 再做处理
                //receiveBytes.CopyTo(cacheBytes, 0);
                //cacheNum = receiveNum;
                //如果进行了 id和长度的解析 但是 没有成功解析消息体 那么我们需要减去nowIndex移动的位置
                if (msgLength != -1)
                    nowIndex -= 8;
                //就是把剩余没有解析的字节数组内容 移到前面来 用于缓存下次继续解析
                Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                cacheNum = cacheNum - nowIndex;
                break;
            }


        }


    }

    public void Close()
    {
        if (socket !=null)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            isConnected = false;

        }
    }
}
