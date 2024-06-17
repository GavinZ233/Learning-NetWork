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



    private byte[] receiveBytes = new byte[1024*1024];
    private int receiveNum;

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
                int msgID = BitConverter.ToInt32(receiveBytes, 0);
                BaseMsg msg = null;
                switch (msgID)
                {
                    case 1001:
                        msg = new PlayerMsg();
                        msg.Reading(receiveBytes, 4);
                        break;
                   
                }
                if (msg == null)
                    continue;


                receiveQueue.Enqueue(msg);

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
