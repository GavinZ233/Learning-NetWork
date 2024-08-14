using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

    
public class Lesson14_同步信息基础 : MonoBehaviour
{
    private void Start()
    {
        //1.创建套接字
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //2.绑定本机地址
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        socket.Bind(ipPoint);

        //3.发送到指定目标
        IPEndPoint remoteIpPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
        //指定要发送的字节数 和 远程计算机的 IP和端口
        socket.SendTo(Encoding.UTF8.GetBytes("客户端发送消息"), remoteIpPoint);

        //4.接受消息
        byte[] bytes = new byte[512];
        //这个变量主要是用来记录 谁发的信息给你 传入函数后 在内部 它会帮助我们进行赋值
        EndPoint remoteIpPoint2 = new IPEndPoint(IPAddress.Any, 0);
        int length = socket.ReceiveFrom(bytes, ref remoteIpPoint2);
        print("IP:" + (remoteIpPoint2 as IPEndPoint).Address.ToString() +
            "port:" + (remoteIpPoint2 as IPEndPoint).Port +
            "发来了" +
            Encoding.UTF8.GetString(bytes, 0, length));

        //5.释放关闭
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

}

