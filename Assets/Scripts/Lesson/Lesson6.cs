using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
public class Lesson6 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

        try
        {
            socket.Connect(ipPoint);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode==10061)
                print("服务器拒绝连接");
            else
                print("连接失败：" + e.ErrorCode);
            return;
        }

        byte[] receiveBytes = new byte[1024];
        int receiveNum = socket.Receive(receiveBytes);
        string printStr = string.Format("接收到服务端{0}的{1}字节", socket.RemoteEndPoint.ToString(),
            receiveNum);
        string receive = Encoding.UTF8.GetString(receiveBytes, 0, receiveNum);

        print(printStr+"接受内容："+receive);

        socket.Send(Encoding.UTF8.GetBytes("这里是客户端"));

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
