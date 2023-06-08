using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Network
{
    public enum NTI_type
    {
        Accept,
        Valid,
        ValidChild,
        Manager,
        Communication,
        CommunicationChild,
        Connect
    }

    public class ThreadInstance
    {
        public string name;
        public Thread thread;
        public DateTime startTime;

        public ThreadInstance(Thread t)
        {
            startTime = DateTime.Now;
            thread = t;
            thread.IsBackground = true;
        }

        public TimeSpan GetRunningTime()
        {
            return DateTime.Now.Subtract(startTime);
        }
    }

    public class SocketInstance
    {
        public static int length = 256;

        public int UID;
        public string name;
        public Socket socket;
        public byte[] sendBuf;
        public byte[] recvBuf;
        public Queue<byte[]> sendList;
        public Queue<byte[]> recvList;

        public SocketInstance(Socket s)
        {
            socket = s;
            sendBuf = new byte[length];
            recvBuf = new byte[length];
        }
    }

    public class NetTaskInstance
    {
        public string name;
        public ManualResetEvent manualResetEvent;
        public ThreadInstance threadInstance;
        public SocketInstance socketInstance;

        public NetTaskInstance()
        {
            manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.Reset();
        }

        public bool markForDone;

        public void StartTask()
        {
            manualResetEvent.Set();
            threadInstance.thread.Start();
        }

        ~NetTaskInstance()
        {
            Debug.LogError($"{name} destroy by desstration");
            this.DestroyTask();
        }

        public void DestroyTask()
        {
            Debug.LogError($"{name} destroy by manual");
            manualResetEvent.Reset();
            if (socketInstance != null && socketInstance.socket != null)
            {
                socketInstance.socket.Disconnect(false);
            }

            if (threadInstance != null && threadInstance.thread != null)
            {
                threadInstance.thread.Abort();
            }
        }
    }

    public class NetworkCenter : MonoBehaviour
    {
        public static bool isServerForS1 = true;

        public static Queue<SocketInstance> tmpSocketInstance = new Queue<SocketInstance>();
        public static Queue<SocketInstance> valSocketInstance = new Queue<SocketInstance>();

        public static Dictionary<NTI_type, List<NetTaskInstance>>
            allNTI = new Dictionary<NTI_type, List<NetTaskInstance>>();


        private void Start()
        {
            allNTI.Add(NTI_type.Accept, new List<NetTaskInstance>());

            allNTI.Add(NTI_type.Valid, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.ValidChild, new List<NetTaskInstance>());

            allNTI.Add(NTI_type.Manager, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.Communication, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.CommunicationChild, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.Connect, new List<NetTaskInstance>());

        }

        private string a;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.LogError(
                    $"tmp:{tmpSocketInstance.Count},val:{valSocketInstance.Count},comm:{CommunicationCenter.clientCommunications.Count}");
                foreach (var c in allNTI)
                {
                    a += c.Key.ToString() + ":" + c.Value.Count + "|";
                }

                Debug.LogError(a);
                a = "";
            }
        }

        public void AcceptStart()
        {
            if (AcceptCenter.InstanceCount > 0) return;
            AcceptCenter ac = new AcceptCenter();
            ac.StartTask();
        }

        public void AcceptStop()
        {
            if (AcceptCenter.InstanceCount <= 0) return;
            allNTI[NTI_type.Accept][0].DestroyTask();
        }

        public void ValidStart()
        {
            if (ValidCenter.InstanceCount > 0) return;
            ValidCenter vc = new ValidCenter();
            vc.StartTask();
        }

        public void ValidStop()
        {
            if (ValidCenter.InstanceCount <= 0) return;
            allNTI[NTI_type.Valid][0].DestroyTask();
        }

        public void CommStart()
        {
            if (CommunicationCenter.InstanceCount > 0) return;
            CommunicationCenter cc = new CommunicationCenter();
            cc.StartTask();
        }

        public void CommStop()
        {
            if (CommunicationCenter.InstanceCount <= 0) return;
            allNTI[NTI_type.Communication][0].DestroyTask();
        }


        public void ManagerStart()
        {
            if (ManagerCenter.InstanceCount > 0) return;
            ManagerCenter mc = new ManagerCenter();
            mc.StartTask();
        }

        public void ManagerStop()
        {
            if (ManagerCenter.InstanceCount <= 0) return;
            allNTI[NTI_type.Manager][0].DestroyTask();
        }


        private void OnDestroy()
        {
            Debug.LogError("OnDestroy");
            foreach (var c in allNTI)
            {
                foreach (var d in c.Value)
                {
                    d.DestroyTask();
                }
            }
        }

        public void ConnectStart()
        {
            if (ConnectCenter.InstanceCount > 0) return;
            ConnectCenter cc = new ConnectCenter();
            cc.StartTask();
        }

        public void ClientSendValid()
        {
            // if (CommunicationCenter.clientCommunications.ContainsKey(-1))
            // {
            //     CommunicationCenter.clientCommunications[-1][CommunicationChildType.Send].socketInstance.sendList
            //         .Enqueue(
            //             new byte[123]);
            // }
        }

        public void ClientSendSome()
        {
            // if (CommunicationCenter.clientCommunications.ContainsKey(-1))
            // {
            //     CommunicationCenter.clientCommunications[-1][CommunicationChildType.Send].socketInstance.sendList
            //         .Enqueue(
            //             new byte[] { 89, 90, 91 });
            // }
        }

        public static void ClientSendText(string toString)
        {
            if (CommunicationCenter.clientCommunications.ContainsKey("Server"))
            {
                CommunicationCenter.clientCommunications["Server"][CommunicationChildType.Send].socketInstance.sendList
                    .Enqueue(
                        Encoding.UTF8.GetBytes(toString));
            }
        }

        public void StartAsServer()
        {
            isServerForS1 = true;
            AcceptStart();
            ValidStart();
            CommStart();
            ManagerStart();
        }
        
        public void StartAsClient()
        {
            isServerForS1 = false;
            CommStart();
            ConnectStart();
        }
    }
}