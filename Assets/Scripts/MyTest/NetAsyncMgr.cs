using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetAsyncMgr : MonoBehaviour
{
    private static NetAsyncMgr instance;
    public static NetAsyncMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                instance = obj.AddComponent<NetAsyncMgr>();
                obj.name = "NetAsyncMgr";
            }
            return instance;
        }
    }

    private Socket socket;
    private byte[] cacheBytes = new byte[1024];
    private int cacheNum;


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

    }

    public void Connect(string ip,int port)
    {
        if (socket!=null&&socket.Connected)
        {
            return;
        }
        socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.RemoteEndPoint = ipPoint;
        args.Completed += (socket, args) =>
        {
            if (args.SocketError==SocketError.Success)
            {
                print("连接成功");
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(cacheBytes, 0, cacheBytes.Length);
                receiveArgs.Completed += ReceiveCallBack;
                this.socket.ReceiveAsync(receiveArgs);
            }
            else
            {
                print("连接失败");
            }
        };
        socket.ConnectAsync(args);
    }
    private void ReceiveCallBack(object obj,SocketAsyncEventArgs arg)
    {
        if (arg.SocketError==SocketError.Success)
        {
            print(Encoding.UTF8.GetString(arg.Buffer, 0, arg.BytesTransferred));
            arg.SetBuffer(0, arg.Buffer.Length);
            socket.ReceiveAsync(arg);
        }
        else
        {
            Close();
            print("接收u消息出错" + arg.SocketError);
        }
    }

    public void Send(string str)
    {
        if (this.socket!=null&&this.socket.Connected)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(bytes, 0, bytes.Length);

            this.socket.SendAsync(args);
        }
    }
    public void Close()
    {
        if (socket != null)
        {

            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);
            socket.Close();
            socket = null;

        }
    }
}
