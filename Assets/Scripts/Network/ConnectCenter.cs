using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class ConnectCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;

        public ConnectCenter() : base()
        {
            BuildConnectNTI(7000);
            this.name = "ConnectCenter";
            InstanceCount++;
        }

        public void BuildConnectNTI(int port)
        {
            //NetTaskInstance AcceptNTI = new NetTaskInstance();
            this.socketInstance =
                new SocketInstance(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            EndPoint ep = new IPEndPoint(ip, port);

            this.socketInstance.UID = -1;
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                this.socketInstance.socket.Connect(ep);
                Debug.LogError("Connected");
                NetworkCenter.valSocketInstance.Enqueue(socketInstance);
                this.socketInstance.socket.Send(new byte[123], 0, 123, SocketFlags.None);
                this.socketInstance = null;
                markForDone = true;
            }));
            NetworkCenter.allNTI[NTI_type.Connect].Add(this);
        }
    }
}