using Google.Protobuf;
using NetSetver.NetCore.Cmd;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetSetver.NetCore.Network
{
    public enum NTI_type
    {
        Server,
        Client
    }

    public class Client
    {
        public string name;
        public bool isPendingDestroy = false;
        public NTICommRecv recv;
        public NTICommSend send;

        public Client(string _name, Socket s)
        {
            Console.WriteLine("Client In");
            name = _name;
            recv = new NTICommRecv(s, this);
            send = new NTICommSend(s, this);
        }

        public void Destroy()
        {
            Console.WriteLine("Something Error, Destroy Client");
            recv.Destroy();
            send.Destroy();
        }
    }

    public class NetThreadInstance
    {
        //Thread
        public Thread thread;
        public ManualResetEvent manualResetEvent;
        //Socket
        public Socket socket;
        public static int bufLength = 256;
        public Queue<byte> recvBuf;
        public Queue<byte[]> sendList;
        public Queue<byte[]> recvList;


        public NetThreadInstance()
        {
            manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.Reset();
        }

        public void Destroy()
        {
            manualResetEvent.Reset();
            if (thread != null)
            {
                thread.Abort();
            }

            if (socket != null)
            {
                if (socket.Connected)
                {
                    socket.Disconnect(false);
                    socket.Dispose();
                }
            }
        }
    }

    public class NetworkManagement
    {
        public static NetworkManagement SingleTon;
        NTI_type type;

        private NTIAccept nTIAccept;
        private NTIConnect nTIConnect;
        public List<Client> clients;
        public Queue<Client> preClients;

        TransformSyncRequest request;

        public void AddClient(Socket s, string name)
        {
            Client tmpClient = new Client(name, s);
            //clients.Add(tmpClient);
            preClients.Enqueue(tmpClient);
        }

        public NetworkManagement(NTI_type _type)
        {
            SingleTon = this;
            type = _type;
            if (type == NTI_type.Server)
            {
                nTIAccept = new NTIAccept("127.0.0.1", 7000);

            }
            if (type == NTI_type.Client)
            {
                nTIConnect = new NTIConnect("127.0.0.1", 7000);
            }
            clients = new List<Client>();
            preClients = new Queue<Client>();
        }

        public void Process()
        {
            if (type == NTI_type.Client)
            {
                ClientProcess();
            }
            if (type == NTI_type.Server)
            {
                ServerProcess();
            }
            CheckClientDestroy();

        }

        private void CheckClientDestroy()
        {
            for (int i = clients.Count - 1; i >= 0; i--)
            {
                if (clients[i].isPendingDestroy)
                {
                    clients[i].Destroy();
                    clients.RemoveAt(i);
                }
            }
        }

        private void ServerProcess()
        {

            nTIAccept.manualResetEvent.Set();

            int count = preClients.Count();
            for (int i = count; i > 0; i--)
            {
                clients.Add(preClients.Dequeue());
            }
            foreach (var i in clients)
            {

                if (i.name == "")
                {
                    //验证
                    Cmd_WhoAreYou cmd = new Cmd_WhoAreYou(i, new WhoAreYouRequest() { ServerName = "MainServer" });
                    i.name = "WaitForReply";
                }
                if (i.name == "WaitForReply" || i.name.Contains("Valid"))
                {

                    //接收并处理
                    ReceiveAndExecuteCommand(i);
                    //发送
                    i.send.manualResetEvent.Set();
                }
            }
        }

        private void ClientProcess()
        {

            if (nTIConnect.socket != null && !nTIConnect.socket.Connected)
            {
                nTIConnect.manualResetEvent.Set();
            }
            else
            {
                int count = preClients.Count();
                for (int i = count; i > 0; i--)
                {
                    clients.Add(preClients.Dequeue());
                }
                foreach (var i in clients)
                {
                    if (i.name.Contains("Server"))
                    {
                        if (request == null)
                        {
                            TransformSyncRequest request = new TransformSyncRequest()
                            {
                                PositionX = 100,
                                PositionY = 100,
                                PositionZ = 100,
                                RotationX = 0,
                                RotationY = 0,
                                RotationZ = 0,
                                GameObjectName = i.send.socket.LocalEndPoint.ToString()
                            };
                            //int j = request.CalculateSize();
                            //byte[] tmp = new byte[j];
                            //request.WriteTo(tmp);
                            string msg = request.GetType().Name + "|" + request.ToString();
                            Cmd_BroadCast cmd = new Cmd_BroadCast(i, new BroadCastRequest() { Msg = msg });
                        }
                        else
                        {
                            string msg = request.GetType().Name + "|" + request.ToString();
                            Cmd_BroadCast cmd = new Cmd_BroadCast(i, new BroadCastRequest() { Msg = msg });
                        }

                        ReceiveAndExecuteCommand(i);
                        i.send.manualResetEvent.Set();

                    }
                }
            }
        }

        private void ReceiveAndExecuteCommand(Client i)
        {
            i.recv.manualResetEvent.Set();
            int count = i.recv.recvList.Count;
            for (int j = count; j > 0; j--)
            {
                CmdManagement.SingleTon.CommandExec(Encoding.ASCII.GetString(i.recv.recvList.Dequeue()), i);
            }
            CmdManagement.SingleTon.Process();
        }

    }
}
