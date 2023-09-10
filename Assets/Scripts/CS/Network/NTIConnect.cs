using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CS.Log;

namespace CS.Network
{
    public class NTIConnect : NetThreadBase
    {
        string strIp;
        int intPort;

        public NTIConnect(string _ip, int _port)
        {
            LogManagement.SingleTon.Log(this.GetType().Name, "NTIConnect");
            strIp = _ip;
            intPort = _port;
            BuildConnectNTI();
        }


        public void BuildConnectNTI()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(strIp);
            EndPoint ep = new IPEndPoint(ip, intPort);
            thread = new Thread(() =>
            {
                while (true)
                {
                    manualResetEvent.WaitOne();
                    try
                    {
                        socket.Connect(ep);
                    }
                    catch (SocketException e)
                    {
                        LogManagement.SingleTon.LogNetContent(this.GetType().Name, "Thread",
                            ep.ToString(), "BuiltIn", e.Message);
                        manualResetEvent.Reset();
                        continue;
                    }
                    catch (InvalidOperationException e)
                    {
                        LogManagement.SingleTon.LogNetContent(this.GetType().Name, "Thread",
                            ep.ToString(), "BuiltIn", e.Message);
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        manualResetEvent.Reset();
                        continue;
                    }

                    NetworkManagement.SingleTon.AddClientInBuffer(socket, "UnknownServer");
                    manualResetEvent.Reset();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}