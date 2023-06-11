using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PRG.Cmd;
using RGP.Cmd;
using RPG.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PRG.Network
{
    public enum NTI_type
    {
        Accept,
        Communication,
        CommunicationChild,
        Connect
    }

    public class ThreadInstance
    {
        public string name;
        public Thread thread;
        public DateTime startTime;

        public ThreadInstance(Thread t, string _name)
        {
            startTime = DateTime.Now;
            thread = t;
            thread.IsBackground = true;
            name = _name;
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
        public Socket socket;
        public byte[] recvBuf;

        //可能会直接传Protobuf btye流，因此在SI中保留原始byte字节，但可以去0
        public Queue<byte[]> sendList;
        public Queue<byte[]> recvList;

        //new时必须提供socket和uid
        public SocketInstance(Socket s, string uid)
        {
            socket = s;
            UID = uid;
            recvList = new Queue<byte[]>();
            sendList = new Queue<byte[]>();
            recvBuf = new byte[SocketInstance.length];
        }
    }

    public class NetTaskInstance
    {
        public bool isFinished;
        public string name;
        public ManualResetEvent manualResetEvent;
        public ThreadInstance threadInstance;
        public SocketInstance socketInstance;

        public NetTaskInstance(string _name)
        {
            manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.Reset();
            name = _name;
        }

        //public bool markForDone;

        public void StartTask()
        {
            manualResetEvent.Set();
            threadInstance.thread.Start();
        }

        public void StopTask()
        {
            manualResetEvent.Reset();
            if (threadInstance != null)
            {
                Debug.LogError(threadInstance.name + ":" + threadInstance.thread.ThreadState);
                threadInstance.thread.Abort();
                Debug.LogError(threadInstance.name + ":" + threadInstance.thread.ThreadState);
            }
            else
            {
                Debug.LogError("threadInstance null, no need des");
            }

            if (socketInstance != null)
            {
                if (socketInstance.socket != null)
                {
                    Debug.LogError(socketInstance.UID + ":" + socketInstance.socket.Connected);
                    if (socketInstance.socket.Connected)
                    {
                        socketInstance.socket.Disconnect(false);
                        Debug.LogError(socketInstance.UID + ":" + socketInstance.socket.Connected);
                    }
                }
            }
            else
            {
                Debug.LogError("socketInstance or socket null, no need des");
            }
        }
    }

    //网络方案总入口
    public class NetworkManagement : TaskPipelineBase, IRecvCmd, ISendSyncObject, ISyncData, ISyncStats
    {
        public static NetworkManagement Ins;
        public static bool isServer;

        private Queue<SocketInstance> valSocketInstance;
        private Dictionary<NTI_type, List<NetTaskInstance>> allNTI;
        private NTICommCenter cc;

        public Queue<byte[]> cmdTunnel;

        // public NetworkCenter()
        // {
        //     Debug.LogError("NetworkCenter Construction");
        //     if (ins != null)
        //     {
        //         Debug.LogError("For Now, Only One NetworkCenter Allowed");
        //         return;
        //     }
        //
        //     ins = this;
        // }

        private void Awake()
        {
            #region 单例

            if (Ins == null)
            {
                Debug.LogError(this.ToString() + " Awake");
                Ins = this;
            }
            else
            {
                Debug.LogError("For Now, Only One " + this.ToString() + " Allowed");
                Destroy(this);
            }

            #endregion

            valSocketInstance = new Queue<SocketInstance>();
            allNTI = new Dictionary<NTI_type, List<NetTaskInstance>>();

            //初始化线程任务集合
            allNTI.Add(NTI_type.Accept, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.Communication, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.CommunicationChild, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.Connect, new List<NetTaskInstance>());
            //初始化通讯中心
            cc = new NTICommCenter("CommunicationCenter");

            #region 初始化控制台

            cc.clientCommunications.Add("cmd", new Dictionary<CommunicationChildType, NetTaskInstance>());
            NetTaskInstance cmdNTI = new NetTaskInstance("cmd");
            cc.clientCommunications["cmd"].Add(CommunicationChildType.Recv, cmdNTI);
            cc.clientCommunications["cmd"].Add(CommunicationChildType.Send, cmdNTI);
            cc.clientCommunications["cmd"][CommunicationChildType.Recv].socketInstance =
                new SocketInstance(null, "cmd");
            cmdTunnel = cc.clientCommunications["cmd"][CommunicationChildType.Recv].socketInstance.recvList;

            #endregion

            //通过地图名，决定功能，直接进入地图时适用
            if (SceneManager.GetActiveScene().name.Contains("Server"))
            {
                isServer = true;
                NTIAccept ac = new NTIAccept("AcceptCenter");
            }

            if (SceneManager.GetActiveScene().name.Contains("Client"))
            {
                isServer = false;
                NTIConnect cc = new NTIConnect("ConnectCenter");
            }
        }


        protected override void OnDestroy()
        {
            DestroyAll();
            base.OnDestroy();
        }

        public void RecvCmd()
        {
            foreach (var c in cc.clientCommunications)
            {
                byte[] b = GetMessageBySocketUID(c.Key);
                if (b != null)
                {
                    CommandExecuter.Ins.CommandExec(c.Key, Encoding.UTF8.GetString(b));
                }
            }
        }

        public void SendSyncObject()
        {
            foreach (var c in FindObjectsOfType<SyncObjectComponent>())
            {
                if (c.enabled)
                {
                    foreach (var d in GetComponents<ISyncObject>())
                    {
                        CMDSyncObject.Send("ClientMainSocket", d.BuildSyncObject());
                    }
                }
            }
        }

        public void SyncStats()
        {
        }

        public void SyncData()
        {
        }


        #region LateUpdate 线程任务清理

        //每帧后检查已完成的线程任务
        public void LateUpdate()
        {
            DestroyFinished();
        }


        //退出时停止所有线程任务
        private void DestroyAll()
        {
            Debug.LogError("DestroyAll");
            foreach (var c in allNTI)
            {
                for (int i = c.Value.Count - 1; i >= 0; i--)
                {
                    c.Value[i].StopTask();
                    c.Value.RemoveAt(i);
                }
            }
        }

        private void DestroyFinished()
        {
            foreach (var c in allNTI)
            {
                for (int i = c.Value.Count - 1; i >= 0; i--)
                {
                    if (c.Value[i].isFinished)
                    {
                        c.Value[i].StopTask();
                        c.Value.RemoveAt(i);
                    }
                }
            }
        }

        #endregion

        #region 普通函数

        //统一套接字实例入口
        public void EnqueueSI(SocketInstance si)
        {
            valSocketInstance.Enqueue(si);
        }

        public SocketInstance DequeueSI()
        {
            if (valSocketInstance.Count > 0)
            {
                return valSocketInstance.Dequeue();
            }

            return null;
        }

        //统一线程任务入口
        public void AddNTI(NTI_type nt, NetTaskInstance nti)
        {
            allNTI[nt].Add(nti);
        }

        //统一接收入口
        public byte[] GetMessageBySocketUID(string uid)
        {
            if (!cc.clientCommunications.ContainsKey(uid))
            {
                //Debug.LogError("GetMessageBySocketUID No Such Key");
                return null;
            }

            if (cc.clientCommunications[uid][CommunicationChildType.Recv]
                    .socketInstance
                    .recvList.Count <= 0)
            {
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
            else
            {
                Debug.LogError("SendMessageBySocketUID No Such Key");
            }
        }


        //通过入口界面选择功能
        public void StartAsServer()
        {
            isServer = true;
            NTIAccept ac = new NTIAccept("AcceptCenter");
            SceneManager.LoadScene("Scenes/Sanebox 1 Server/Sanebox 1 Server");
        }

        public void StartAsClient()
        {
            isServer = false;
            NTIConnect cc = new NTIConnect("ConnectCenter");
            //客户端通过网络消息进行场景切换
        }

        #endregion
    }
}