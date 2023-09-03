using NetSetver.NetCore.Cmd;
using NetSetver.NetCore.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetSetver
{
    class Program
    {

        static int count = 0;
        static DateTime curr;
        static DateTime lastFrame;
        static NetworkManagement nm;
        static CmdManagement cm;

        static void Main(string[] args)
        {
            curr = DateTime.Now;
            lastFrame = DateTime.Now;
            string input = Console.ReadLine();
            if (int.Parse(input) == 0)
            {
                nm = new NetworkManagement(NTI_type.Server);
            }
            if (int.Parse(input) == 1)
            {
                nm = new NetworkManagement(NTI_type.Client);
            }
            cm = new CmdManagement();


            Console.WriteLine("Pragram Start");
            while (true)
            {
                if (curr.Subtract(lastFrame).Seconds > 2)
                {
                    lastFrame = curr;
                    nm.Process();
                }
                curr = DateTime.Now;
                Thread.Sleep(1);
            }
        }
    }
}
