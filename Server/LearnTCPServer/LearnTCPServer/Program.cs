using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace LearnTCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);


            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

                socket.Bind(ipPoint);


            }
            catch (Exception e)
            {

                Console.WriteLine("绑定报错:" + e.Message);
            }

            socket.Listen(23);

            Console.WriteLine("服务的绑定监听结束，等待客户端");

            Socket socketClient = socket.Accept();

            Console.WriteLine("客户端连入");

            socketClient.Send(Encoding.UTF8.GetBytes("成功连入客户端"));

            byte[] result = new byte[1024];

            int receiveNum = socketClient.Receive(result);
            Console.WriteLine("接收{0}发的{1}字节{2}",
                socketClient.RemoteEndPoint.ToString(),
                receiveNum,
                Encoding.UTF8.GetString(result,0,receiveNum));

            Console.ReadKey();

            socketClient.Shutdown(SocketShutdown.Both);

            socketClient.Close();

        }





    }
}
