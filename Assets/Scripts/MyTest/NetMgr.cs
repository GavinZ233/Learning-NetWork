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

    private Queue<string> sendMsgQueue = new Queue<string>();
    private Queue<string> receiveQueue = new Queue<string>();



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
            print("接收到消息:" + receiveQueue.Dequeue());
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


    public void Send(string info)
    {
        sendMsgQueue.Enqueue(info);
    }

    private void SendMsg(object obj) 
    {
        while (isConnected)
        {
            if (sendMsgQueue.Count>0)
            {
                socket.Send(Encoding.UTF8.GetBytes(sendMsgQueue.Dequeue()));
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
                receiveQueue.Enqueue(Encoding.UTF8.GetString(receiveBytes, 0, receiveNum));

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
