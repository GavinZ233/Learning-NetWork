﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServerExercises2
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerSocket socket = new ServerSocket();
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
                    socket.Broadcast(input.Substring(2));
                }
            }


        }
    }
}
