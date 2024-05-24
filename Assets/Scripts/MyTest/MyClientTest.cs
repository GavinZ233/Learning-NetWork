using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
public class MyClientTest : MonoBehaviour
{
    Socket socket;

    Queue<string> sendStrs;
    Queue<string> receiveStrs;

    public InputField input;

    private void Start()
    {
        input.onValueChanged.AddListener(OnInputValueChange);


        //开始客户端，连接服务端
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

        sendStrs = new Queue<string>();
        receiveStrs = new Queue<string>();

        try
        {
            socket.Connect(ip);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
                print("服务器拒绝连接");
            else
                print("连接失败：" + e.ErrorCode);
            return;
        }

        //接收线程
        ThreadPool.QueueUserWorkItem(Receive);
        //发送线程
        ThreadPool.QueueUserWorkItem(Send);

    }
    private void OnInputValueChange(string s)
    {
        sendStrs.Enqueue(s);
    }
    private void Receive(object obj)
    {
        while (true)
        {
            if (socket.Available>0)
            {
                byte[] bytes = new byte[1024];
                int receiveNum =socket.Receive(bytes);
                receiveStrs.Enqueue(Encoding.UTF8.GetString(bytes,0,receiveNum));

            }

        }
    }
    private void Send(object obj)
    {
        while (true)
        {
            if (sendStrs.Count>0)
            {
                socket.Send(Encoding.UTF8.GetBytes(sendStrs.Dequeue()));

            }
        }
    }
    private void Update()
    {
        if (receiveStrs.Count>0)
        {
            print("接收到消息:"+receiveStrs.Dequeue());
        }
    }

}
