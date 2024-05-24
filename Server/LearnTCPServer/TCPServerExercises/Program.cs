using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TCPServerExercises
{
    class Program
    {
        static Socket serverSocket;
        static List<Socket> clientSockets=new List<Socket>();
        static bool isClose = false;
        static void Main(string[] args)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

            serverSocket.Bind(ipEndPoint);

            serverSocket.Listen(22);

            Thread acceptThread = new Thread(AcceptClientConnet);
            Thread receiveThread = new Thread(ReceiveClientMsg);
            acceptThread.Start();
            receiveThread.Start();

            while (true)
            {
                string input = Console.ReadLine();
                if (input=="Quit")
                {
                    isClose = true;
                    for (int i = 0; i < clientSockets.Count; i++)
                    {
                        clientSockets[i].Shutdown(SocketShutdown.Both);
                        clientSockets[i].Close();
                    }
                    clientSockets.Clear();
                    break;
                }
                else if (input.Substring(0,2)=="Q:")
                {
                    for (int i = 0; i < clientSockets.Count; i++)
                    {
                        clientSockets[i].Send(Encoding.UTF8.GetBytes(input.Substring(2)));
                    }
                }
            }

        }

        static void AcceptClientConnet()
        {
            while (!isClose)
            {
                Socket socket = serverSocket.Accept();
                clientSockets.Add(socket);
                socket.Send(Encoding.UTF8.GetBytes("成功接入服务端"));
            }
        }

        static void ReceiveClientMsg()
        {
            Socket client;
            byte[] bytes = new byte[1024];
            int receiveNum;
            while (!isClose)
            {
                for (int i = 0; i <clientSockets.Count; i++)
                {
                    client = clientSockets[i];
                    if (client.Available>0)
                    {
                        receiveNum = client.Receive(bytes);

                        ThreadPool.QueueUserWorkItem(HandleMsg,
                            (client,Encoding.UTF8.GetString(bytes,0,receiveNum)));
                    }
                }
            }
        }

        static void HandleMsg(object obj)
        {
            (Socket socket,string str)info = ((Socket socket, string str)) obj;
            Console.WriteLine("收到客户端{0}发来的消息:{1}", info.socket.RemoteEndPoint.ToString(), info.str);
        }

    }
}
