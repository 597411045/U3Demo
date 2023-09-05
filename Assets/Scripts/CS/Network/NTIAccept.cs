using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CS.Log;

namespace CS.Network
{
    public class NTIAccept : NetThreadInstance
    {
        string strIp;
        int intPort;

        public NTIAccept(string _ip, int _port)
        {
            LogManagement.Log("NTIAccept Init");
            strIp = _ip;
            intPort = _port;
            BuildAcceptNTI();
        }

        public void BuildAcceptNTI()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(strIp);
            EndPoint ep = new IPEndPoint(ip, intPort);
            socket.Bind(ep);
            socket.Listen(5);

            thread = new Thread(() =>
            {
                while (true)
                {
                    this.manualResetEvent.WaitOne();
                    Socket tmpS = socket.Accept();
                    NetworkManagement.SingleTon.AddClient(tmpS, "UnknownClient");
                    manualResetEvent.Reset();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}