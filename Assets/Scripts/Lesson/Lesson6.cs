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
                print("�������ܾ�����");
            else
                print("����ʧ�ܣ�" + e.ErrorCode);
            return;
        }

        byte[] receiveBytes = new byte[1024];
        int receiveNum = socket.Receive(receiveBytes);
        string printStr = string.Format("���յ������{0}��{1}�ֽ�", socket.RemoteEndPoint.ToString(),
            receiveNum);
        string receive = Encoding.UTF8.GetString(receiveBytes, 0, receiveNum);

        print(printStr+"�������ݣ�"+receive);

        socket.Send(Encoding.UTF8.GetBytes("�����ǿͻ���"));

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
