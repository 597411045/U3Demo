using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PRG.Network
{
    public class NTIAccept : NetTaskInstance
    {
        public static int InstanceCount = 0;


        public NTIAccept(string name) : base(name)
        {
            BuildAcceptNTI(7000);
            InstanceCount++;
        }

        public void BuildAcceptNTI(int port)
        {
            this.socketInstance =
                new SocketInstance(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
                    "ServerMainSocket");
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            EndPoint ep = new IPEndPoint(ip, port);
            this.socketInstance.socket.Bind(ep);
            this.socketInstance.socket.Listen(5);
            Debug.LogError("Listen Ready");
            this.name = "AcceptNTI";

            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.LogError("BuildAcceptNTI Start");
                while (true)
                {
                    this.manualResetEvent.WaitOne();
                    Socket tmpS = this.socketInstance.socket.Accept();
                    Debug.LogError("A New Client In");
                    //取消Valid，无需传输到临时Socket列表，直接转入Val列表
                    //NetworkCenter.tmpSocketInstance.Enqueue(new SocketInstance(tmp));
                    SocketInstance tmpSI = new SocketInstance(tmpS, System.Guid.NewGuid().ToString());
                    tmpSI.sendList.Enqueue(Encoding.UTF8.GetBytes("Hello Client"));
                    NetworkManagement.Ins.EnqueueSI(tmpSI);
                }
            }), "BuildAcceptNTI");
            StartTask();
            NetworkManagement.Ins.AddNTI(NTI_type.Accept, this);
        }
    }
}