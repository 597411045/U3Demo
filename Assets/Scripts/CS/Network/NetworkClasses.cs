using Google.Protobuf;
using CS.Cmd;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CS.Log;

namespace CS.Network
{
    public enum NTI_type
    {
        Server,
        Client
    }

    public enum ClientState
    {
        Unknown,
        PendingValid,
        Valid,
        PendingDestroy
    }

    public class Client
    {
        public string name;
        public ClientState state;
        public NTICommRecv recv;
        public NTICommSend send;

        public Client(string _name, Socket s)
        {
            LogManagement.Log("Client In");
            name = _name;
            recv = new NTICommRecv(s, this);
            send = new NTICommSend(s, this);
            state = ClientState.Unknown;
        }

        public void Destroy()
        {
            LogManagement.Log("Something Error, Destroy Client");
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
        public List<Client> ClientList;
        public Queue<Client> ClientBuffer;
        public Queue<CmdBase> CmdBuffer;
        public List<Action> customActions;

        public void AddClient(Socket s, string name)
        {
            Client tmpClient = new Client(name, s);
            ClientBuffer.Enqueue(tmpClient);
        }

        public void AddCmd(CmdBase cmd)
        {
            CmdBuffer.Enqueue(cmd);
        }

        public NetworkManagement(NTI_type _type, string ip = "127.0.0.1", int port = 7000)
        {
            SingleTon = this;
            type = _type;

            customActions = new List<Action>();

            if (type == NTI_type.Server)
            {
                nTIAccept = new NTIAccept(ip, port);
                customActions.Add(ServerAction);
            }

            if (type == NTI_type.Client)
            {
                nTIConnect = new NTIConnect(ip, port);
            }

            ClientList = new List<Client>();
            ClientBuffer = new Queue<Client>();
            CmdBuffer = new Queue<CmdBase>();
        }

        public void AutoProcess()
        {
            if (type == NTI_type.Client)
            {
                ClientAutoProcess();
            }

            if (type == NTI_type.Server)
            {
                ServerAutoProcess();
            }
        }

        public void CheckInvalidClients()
        {
            for (int i = ClientList.Count - 1; i >= 0; i--)
            {
                if (ClientList[i].state == ClientState.PendingDestroy)
                {
                    ClientList[i].Destroy();
                    ClientList.RemoveAt(i);
                }
            }
        }

        private void ServerAutoProcess()
        {
            nTIAccept.manualResetEvent.Set();

            FlushClients();
            CheckInvalidClients();
            TryReceiveExceptPD();
            TryExecuteRecv();
            DoCustomActions();
            TryExecuteSend();
            TrySendExceptPD();
        }

        private void ClientAutoProcess()
        {
            if (TryConnect())
            {
            }
            else
            {
                FlushClients();
                CheckInvalidClients();
                TryReceiveExceptPD();
                TryExecuteRecv();
                DoCustomActions();
                TryExecuteSend();
                TrySendExceptPD();
            }
        }

        public bool TryConnect()
        {
            if (nTIConnect.socket != null && !nTIConnect.socket.Connected)
            {
                nTIConnect.manualResetEvent.Set();
                return true;
            }

            return false;
        }

        public void FlushClients()
        {
            int count = ClientBuffer.Count();
            for (int i = count; i > 0; i--)
            {
                ClientList.Add(ClientBuffer.Dequeue());
                LogManagement.Log("Flush a Client");
            }
        }

        public void TryReceiveExceptPD()
        {
            foreach (var i in ClientList)
            {
                if (i.state != ClientState.PendingDestroy)
                {
                    i.recv.manualResetEvent.Set();
                }
            }
        }

        public void TryExecuteRecv()
        {
            //对接收对cmd做处理
            foreach (var i in ClientList)
            {
                if (i.state != ClientState.PendingDestroy)
                {
                    int c1 = i.recv.recvList.Count;
                    for (int j = c1; j > 0; j--)
                    {
                        CmdManagement.SingleTon.CommandExec(Encoding.ASCII.GetString(i.recv.recvList.Dequeue()), i);
                    }
                }
            }

            //Cmd综合处理
            CmdManagement.SingleTon.Process();
        }

        public void TryExecuteSend()
        {
            //对预备发送的cmd做处理
            int c2 = CmdBuffer.Count;
            for (int i = c2; i > 0; i--)
            {
                CmdBuffer.Dequeue().JoinCmdExecDic();
            }

            //Cmd综合处理
            CmdManagement.SingleTon.Process();
        }

        public void TrySendExceptPD()
        {
            foreach (var i in ClientList)
            {
                if (i.state != ClientState.PendingDestroy)
                {
                    i.send.manualResetEvent.Set();
                }
            }
        }

        public void DoCustomActions()
        {
            for (int i = customActions.Count - 1; i >= 0; i--)
            {
                customActions[i].Invoke();
            }
        }

        private void ServerAction()
        {
            foreach (var i in ClientList)
            {
                if (i.state == ClientState.Unknown)
                {
                    Cmd_WhoAreYou cmd = new Cmd_WhoAreYou(i, new WhoAreYouRequest() { ServerName = "MainServer" });
                    AddCmd(cmd);
                    LogManagement.Log("Add Cmd_WhoAreYou");
                }
            }
        }

        public void ClientAction()
        {
            foreach (var i in ClientList)
            {
                if (i.state == ClientState.Valid)
                {
                    TransformSyncRequest request = new TransformSyncRequest()
                    {
                        PositionX = 100,
                        PositionY = 100,
                        PositionZ = 100,
                        RotationX = 0,
                        RotationY = 0,
                        RotationZ = 0,
                        GameObjectName = ClientList[0].send.socket.LocalEndPoint.ToString()
                    };
                    string msg = request.GetType().Name + "|" + request.ToString();
                    Cmd_BroadCast cmd = new Cmd_BroadCast(ClientList[0], new BroadCastRequest() { Msg = msg });
                    AddCmd(cmd);
                }
            }
        }

        public void CloseAll()
        {
            LogManagement.Log("NetworkManager.CloseAll");
            foreach (var i in ClientList)
            {
                i.state = ClientState.PendingDestroy;
            }

            CheckInvalidClients();
        }
    }
}