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

        static void Main(string[] args)
        {
            try
            {
                curr = DateTime.Now;
                lastFrame = DateTime.Now;
                string input = Console.ReadLine();
                if (int.Parse(input) == 0)
                {
                    LogManagement.SingleTon.Initial("Server");
                    NetworkManagement.SingleTon.Initial(NTI_type.Server);
                }

                if (int.Parse(input) == 2)
                {
                    LogManagement.SingleTon.Initial("Server");
                    NetworkManagement.SingleTon.Initial(NTI_type.Server, "10.0.4.13");
                }

                if (int.Parse(input) == 1)
                {
                    LogManagement.SingleTon.Initial("Client");
                    NetworkManagement.SingleTon.Initial(NTI_type.Client);
                }


                LogManagement.SingleTon.Log("Program", "Main");


                while (true)
                {
                    if (curr.Subtract(lastFrame).Seconds > 1)
                    {
                        lastFrame = curr;
                        NetworkManagement.SingleTon.AutoProcess();
                    }

                    curr = DateTime.Now;
                    Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                LogManagement.SingleTon.Log("Program", "Main", "\n" + e.StackTrace);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}