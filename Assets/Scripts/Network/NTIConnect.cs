using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using RPG.Cmd;
using UnityEngine;

namespace PRG.Network
{
    public class NTIConnect : NetTaskInstance
    {
        public static int InstanceCount = 0;

        public NTIConnect(string name) : base(name)
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
                    NetworkManagement.Ins.EnqueueSI(socketInstance);

                    CMDHello.Ins.Send(this.socketInstance, "Hello Server");

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
            NetworkManagement.Ins.AddNTI(NTI_type.Connect, this);
        }
    }
}