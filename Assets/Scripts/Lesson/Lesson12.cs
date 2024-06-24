using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Lesson12 : MonoBehaviour
{
    private byte[] bytes = new byte[1024];
    // Start is called before the first frame update
    void Start()
    {
        //服务端接收客户端连入
        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socketTcp.BeginAccept(AcceptCallBack, socketTcp);

        //客户端连接服务端
        Socket clientTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),8080);
        clientTcp.BeginConnect(ipPoint, (result) =>
        {
            try
            {
                clientTcp.EndConnect(result);
            }
            catch (SocketException e)
            {
                print(e.SocketErrorCode);
            }
        },clientTcp);


        //收发消息

        //
        socketTcp.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, ReceiveCallBack,socketTcp);
        socketTcp.BeginSend(bytes,0,bytes.Length,SocketFlags.None,(result)=>
        {
            try
            {
                socketTcp.EndReceive(result);


            }
            catch (SocketException e)
            {
                print("发送错误:" + e.SocketErrorCode);
            }
        },socketTcp);


        SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
        saea.Completed += (socket,args)=> { };

        socketTcp.AcceptAsync();


    }

    private void ReceiveCallBack(IAsyncResult result)
    {
        try
        {
            Socket s = result.AsyncState as Socket;

            int num = s.EndReceive(result);

            Encoding.UTF8.GetString(bytes, 0, num);

            s.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, ReceiveCallBack, s);
        }
        catch (SocketException e)
        {
            print("接收消息出错：" + e.SocketErrorCode);

        }
    }

    private void AcceptCallBack(IAsyncResult result)
    {
        try
        {
            Socket s = result.AsyncState as Socket;
            Socket clientSocket = s.EndAccept(result);

            s.BeginAccept(AcceptCallBack, s);
        }
        catch (SocketException e)
        {
            print(e.SocketErrorCode);

        }
    }

}
