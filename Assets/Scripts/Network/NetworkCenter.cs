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
    public class NetworkCenter : MonoBehaviour
    {
        public static bool isServer;
        public static NetworkCenter ins;


        //取消Valid
        //public static Queue<SocketInstance> tmpSocketInstance = new Queue<SocketInstance>();
        public static Queue<SocketInstance> valSocketInstance = new Queue<SocketInstance>();

        public static Dictionary<NTI_type, List<NetTaskInstance>>
            allNTI = new Dictionary<NTI_type, List<NetTaskInstance>>();


        private CommunicationCenter cc;

        private void Start()
        {
            ins = this;
            allNTI.Add(NTI_type.Accept, new List<NetTaskInstance>());

            allNTI.Add(NTI_type.Communication, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.CommunicationChild, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.Connect, new List<NetTaskInstance>());

            CommStart();
        }

        private string a;

        private byte[] b;


        private void Awake()
        {
            UpdateManager.ServerAsyn.Add(new CAction(UpdateMethod, this.GetInstanceID(), this.gameObject));
        }
        
        public void UpdateMethod()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                DestroyAll();
            }

            if (isServer)
            {
                foreach (var c in cc.clientCommunications)
                {
                    b = GetMessageBySocketUID(c.Key);
                    if (b != null)
                    {
                        CommandExecuter.CommandExec(c.Key, Encoding.UTF8.GetString(b));
                    }
                }
            }
            else
            {
                b = GetMessageBySocketUID("ClientMainSocket");
                if (b != null)
                {
                    CommandExecuter.CommandExec("ClientMainSocket", Encoding.UTF8.GetString(b));
                }
            }
        }

        public void LateUpdate()
        {
            DestroyFinished();
        }

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

        private void OnDestroy()
        {
            DestroyAll();
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

        public void AcceptStart()
        {
            if (AcceptCenter.InstanceCount > 0) return;
            AcceptCenter ac = new AcceptCenter("AcceptCenter");
        }

        public void AcceptStop()
        {
            if (AcceptCenter.InstanceCount <= 0) return;
        }


        public void CommStart()
        {
            if (CommunicationCenter.InstanceCount > 0) return;
            cc = new CommunicationCenter("CommunicationCenter");
        }

        public void CommStop()
        {
            if (CommunicationCenter.InstanceCount <= 0) return;
        }


        public void ConnectStart()
        {
            if (ConnectCenter.InstanceCount > 0) return;
            ConnectCenter cc = new ConnectCenter("ConnectCenter");
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
            else
            {
                Debug.LogError("SendMessageBySocketUID Has No Key: " + uid);
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
            SceneManager.LoadScene(1);
        }

        public void StartAsClient()
        {
            isServer = false;
            //启动NC时，自动启用
            //CommStart();
            ConnectStart();
        }
    }
}