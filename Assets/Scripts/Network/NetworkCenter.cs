using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        //改用string作为UID，可直接用于Dic
        //public int UID;
        public string UID;
        public string name;
        public Socket socket;
        public byte[] sendBuf;
        public byte[] recvBuf;

        //可能会直接传Protobuf btye流，因此在SI中保留原始byte字节，但可以去0
        public Queue<byte[]> sendList;
        public Queue<byte[]> recvList;

        //new时必须提供socket和uid
        public SocketInstance(Socket s, string uid)
        {
            socket = s;
            //一并在new Comm时初始化buffer
            //sendBuf = new byte[length];
            //recvBuf = new byte[length];
            UID = uid;
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

    //网络方案总入口
    public class NetworkCenter : MonoBehaviour
    {
        public static bool isServer;

        //取消Valid
        //public static Queue<SocketInstance> tmpSocketInstance = new Queue<SocketInstance>();
        public static Queue<SocketInstance> valSocketInstance = new Queue<SocketInstance>();

        public static Dictionary<NTI_type, List<NetTaskInstance>>
            allNTI = new Dictionary<NTI_type, List<NetTaskInstance>>();


        private CommunicationCenter cc;

        private void Start()
        {
            allNTI.Add(NTI_type.Accept, new List<NetTaskInstance>());

            allNTI.Add(NTI_type.Valid, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.ValidChild, new List<NetTaskInstance>());

            allNTI.Add(NTI_type.Manager, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.Communication, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.CommunicationChild, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.Connect, new List<NetTaskInstance>());

            CommStart();
        }

        private string a;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.LogError(
                    // $"tmp:{tmpSocketInstance.Count},val:{valSocketInstance.Count},comm:{CommunicationCenter.clientCommunications.Count}");
                    $"val:{valSocketInstance.Count},comm:{cc.clientCommunications.Count}");
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
            cc = new CommunicationCenter();
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

        //统一接收入口
        public byte[] GetMessageBySocketUID(string uid)
        {
            if (!cc.clientCommunications.ContainsKey(uid))
            {
                //Debug.LogError("No This Key");
                return null;
            }

            if (cc.clientCommunications[uid][CommunicationChildType.Recv]
                    .socketInstance
                    .recvList.Count <= 0)
            {
                //Debug.LogError("Dic Count <= 0");

                return null;
            }

            return cc.clientCommunications[uid][CommunicationChildType.Recv].socketInstance
                .recvList
                .Dequeue();
        }

        //统一发送出口
        public void SendMessageBySocketUID(string uid, byte[] bs)
        {
            if (cc.clientCommunications.ContainsKey(uid))
            {
                cc.clientCommunications[uid][CommunicationChildType.Send].socketInstance.sendList
                    .Enqueue(bs);
            }
        }

        public void StartAsServer()
        {
            isServer = true;
            AcceptStart();
            //取消Valid，合并至Cmd和Comm
            //ValidStart();

            //启动NC时，自动启用
            //CommStart();
            ManagerStart();
            SceneManager.LoadScene(1);
        }

        public void StartAsClient()
        {
            isServer = false;
            //启动NC时，自动启用
            //CommStart();
            ConnectStart();
            SceneManager.LoadScene(2);
        }
    }
}