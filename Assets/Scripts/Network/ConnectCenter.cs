using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PRG.Network
{
    public class ConnectCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;

        public ConnectCenter(string name) : base(name)
        {
            BuildConnectNTI(7000);
            InstanceCount++;
        }

        public void BuildConnectNTI(int port)
        {
            this.socketInstance =
                new SocketInstance(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                    "ClientMainSocket");
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            EndPoint ep = new IPEndPoint(ip, port);

            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.LogError("BuildConnectNTI Start");
                try
                {
                    this.socketInstance.socket.Connect(ep);
                    Debug.LogError("Connected");
                    NetworkCenter.Ins.EnqueueSI(socketInstance);
                    this.socketInstance.sendList.Enqueue(Encoding.UTF8.GetBytes("Hello Server"));
                    this.socketInstance = null;
                }
                catch (Exception e)
                {
                    Debug.LogError("CATCHED:" + e);
                }
                finally
                {
                    isFinished = true;
                }
            }), "BuildConnectNTI");
            StartTask();
            NetworkCenter.Ins.AddNTI(NTI_type.Connect, this);
        }
    }
}