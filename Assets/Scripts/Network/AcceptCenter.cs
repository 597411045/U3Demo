using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class AcceptCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;

        public AcceptCenter() : base()
        {
            BuildAcceptNTI(7000);
            this.name = "AcceptCenter";
            InstanceCount++;

        }

        public void BuildAcceptNTI(int port)
        {
            //NetTaskInstance AcceptNTI = new NetTaskInstance();
            this.socketInstance =
                new SocketInstance(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            EndPoint ep = new IPEndPoint(ip, port);
            this.socketInstance.socket.Bind(ep);
            this.socketInstance.socket.Listen(5);
            Debug.LogError("Listen Ready");
            this.name = "AcceptNTI";

            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.LogError("AcceptNTI Start");
                while (true)
                {
                    this.manualResetEvent.WaitOne();
                    Socket tmp = this.socketInstance.socket.Accept();
                    Debug.LogError("A New Client In");
                    NetworkCenter.tmpSocketInstance.Enqueue(new SocketInstance(tmp));
                }
            }));
            NetworkCenter.allNTI[NTI_type.Accept].Add(this);
        }
    }
}