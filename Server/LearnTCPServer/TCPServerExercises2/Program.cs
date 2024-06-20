using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerExercises2
{
    class Program
    {
        public static ServerSocket socket;

        static void Main(string[] args)
        {
            socket = new ServerSocket();
            socket.Start("127.0.0.1",8080,55);

            while (true)
            {
                string input = Console.ReadLine();
                if (input=="Quit")
                {
                    socket.Close();
                }
                else if (input.Substring(0,2)=="B:")
                {
                    if (input.Substring(2)=="1001")
                    {
                        PlayerMsg msg = new PlayerMsg();
                        msg.playerID = 666;
                        msg.playerData = new PlayerData();
                        msg.playerData.name = "服务端发送的";
                        socket.Broadcast(msg);

                    }
                }
            }


        }
    }
}
