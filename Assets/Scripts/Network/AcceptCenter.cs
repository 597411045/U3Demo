using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class AcceptCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;

        //临时记录tmpSocketUid
        public static int tmpId = 1;

        public AcceptCenter(string name) : base(name)
        {
            BuildAcceptNTI(7000);
            InstanceCount++;
        }

        public void BuildAcceptNTI(int port)
        {
            //NetTaskInstance AcceptNTI = new NetTaskInstance();
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
                Debug.LogError("AcceptNTI Start");
                while (true)
                {
                    this.manualResetEvent.WaitOne();
                    Socket tmpS = this.socketInstance.socket.Accept();
                    Debug.LogError("A New Client In");
                    //取消Valid，无需传输到临时Socket列表，直接转入Val列表
                    //NetworkCenter.tmpSocketInstance.Enqueue(new SocketInstance(tmp));
                    SocketInstance tmpSI = new SocketInstance(tmpS, "tmpSocket" + tmpId++.ToString());
                    NetworkCenter.valSocketInstance.Enqueue(tmpSI);
                }
            }),"BuildAcceptNTI");
            StartTask();
            NetworkCenter.allNTI[NTI_type.Accept].Add(this);
        }
    }
}