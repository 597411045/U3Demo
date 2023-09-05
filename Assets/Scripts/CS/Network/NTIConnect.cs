using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CS.Log;

namespace CS.Network
{
    public class NTIConnect : NetThreadInstance
    {

        string strIp;
        int intPort;
        public NTIConnect(string _ip, int _port)
        {
            LogManagement.Log("NTIConnect Init");
            strIp = _ip;
            intPort = _port;
            BuildConnectNTI();
        }

        public void BuildConnectNTI()
        {
            if (thread != null)
            {
                thread.Abort();
            }

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
                        LogManagement.Log(e.Message);
                        manualResetEvent.Reset();
                        continue;
                    }catch(InvalidOperationException e)
                    {
                        LogManagement.Log(e.Message);
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        manualResetEvent.Reset();
                        continue;
                    }
                    NetworkManagement.SingleTon.AddClient(socket, "UnknownServer");
                    manualResetEvent.Reset();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}