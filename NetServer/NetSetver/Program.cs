using CS.Cmd;
using CS.Network;
using CS.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS
{
    class Program
    {
        static int count = 0;
        static DateTime curr;
        static DateTime lastFrame;
        static NetworkManagement nm;
        static CmdManagement cm;
        static LogManagement lm;

        static void Main(string[] args)
        {
            curr = DateTime.Now;
            lastFrame = DateTime.Now;
            string input = Console.ReadLine();
            if (int.Parse(input) == 0)
            {
                lm = new LogManagement("Server");
                nm = new NetworkManagement(NTI_type.Server);
            }

            if (int.Parse(input) == 1)
            {
                lm = new LogManagement("Client");
                nm = new NetworkManagement(NTI_type.Client);
            }

            cm = new CmdManagement();

            LogManagement.Log("Program Start");

            while (true)
            {
                if (curr.Subtract(lastFrame).Seconds > 2)
                {
                    lastFrame = curr;
                    nm.AutoProcess();
                }

                curr = DateTime.Now;
                Thread.Sleep(1);
            }
        }
    }
}