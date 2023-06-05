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
        Manager
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

        public string name;
        public int verseId;
        public Socket socket;
        public byte[] sendBuf;
        public byte[] recvBuf;

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
            this.DestroyTask();
        }

        public void DestroyTask()
        {
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

    public class SocketTest : MonoBehaviour
    {
        public static Queue<SocketInstance> tmpSocketInstance = new Queue<SocketInstance>();
        public static List<SocketInstance> valSocketInstance = new List<SocketInstance>();

        public static Dictionary<NTI_type, List<NetTaskInstance>>
            allNTI = new Dictionary<NTI_type, List<NetTaskInstance>>();


        private void Start()
        {
            allNTI.Add(NTI_type.Accept, new List<NetTaskInstance>());
            BuildAcceptNTI(7000);

            allNTI.Add(NTI_type.Valid, new List<NetTaskInstance>());
            allNTI.Add(NTI_type.ValidChild, new List<NetTaskInstance>());
            BuildValidNTI();

            allNTI.Add(NTI_type.Manager, new List<NetTaskInstance>());
            BuildManagerNTI();
        }

        private void BuildManagerNTI()
        {
            NetTaskInstance ManagerNTI = new NetTaskInstance();


            ManagerNTI.name = "ManagerNTI";

            ManagerNTI.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.Log("ManagerNTI Start");
                while (true)
                {
                    if (allNTI[NTI_type.ValidChild].Count > 0)
                    {
                        for (int i = allNTI[NTI_type.ValidChild].Count - 1; i >= 0; i--)
                        {
                            if (allNTI[NTI_type.ValidChild][i].threadInstance.GetRunningTime().TotalSeconds > 5 ||
                                allNTI[NTI_type.ValidChild][i].markForDone)
                            {
                                allNTI[NTI_type.ValidChild][i].DestroyTask();
                                allNTI[NTI_type.ValidChild].RemoveAt(i);
                                Debug.Log("Remove A Timeout ValidChildNTI");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("ManagerNTI Wait 5 Second");
                        Debug.Log(valSocketInstance.Count);
                        Thread.Sleep(5000);
                    }
                }
            }));
            allNTI[NTI_type.Manager].Add(ManagerNTI);
        }


        public void BuildAcceptNTI(int port)
        {
            NetTaskInstance AcceptNTI = new NetTaskInstance();
            AcceptNTI.socketInstance =
                new SocketInstance(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            EndPoint ep = new IPEndPoint(ip, port);
            AcceptNTI.socketInstance.socket.Bind(ep);
            AcceptNTI.socketInstance.socket.Listen(5);
            Debug.Log("Listen Ready");
            AcceptNTI.name = "AcceptNTI";

            AcceptNTI.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.Log("AcceptNTI Start");
                while (true)
                {
                    AcceptNTI.manualResetEvent.WaitOne();
                    Socket tmp = AcceptNTI.socketInstance.socket.Accept();
                    Debug.Log("A New Client In");
                    tmpSocketInstance.Enqueue(new SocketInstance(tmp));
                }
            }));
            allNTI[NTI_type.Accept].Add(AcceptNTI);
        }

        public void BuildValidNTI()
        {
            NetTaskInstance ValidNTI = new NetTaskInstance();
            ValidNTI.name = "ValidNTI";
            ValidNTI.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.Log("ValidNTI Start");
                while (true)
                {
                    ValidNTI.manualResetEvent.WaitOne();
                    if (tmpSocketInstance.Count > 0)
                    {
                        SocketInstance tmp = tmpSocketInstance.Dequeue();
                        BuildValidChildNTI(tmp);
                        Debug.Log("Build A ValidChildNTI");
                    }
                    else
                    {
                        Debug.Log("ValidNTI Wait 1 Second");
                        Thread.Sleep(1000);
                    }
                }
            }));

            allNTI[NTI_type.Valid].Add(ValidNTI);
        }

        public void BuildValidChildNTI(SocketInstance s)
        {
            NetTaskInstance ValidChildNTI = new NetTaskInstance();
            ValidChildNTI.socketInstance =
                s;

            ValidChildNTI.name = "ValidChildNTI";

            ValidChildNTI.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.Log("ValidChildNTI Start");
                int length = ValidChildNTI.socketInstance.socket.Receive(ValidChildNTI.socketInstance.recvBuf, 0,
                    ValidChildNTI.socketInstance.recvBuf.Length, SocketFlags.None);
                Debug.Log($"Reveived {length} bytes");
                if (length != 123)
                {
                    Debug.Log($"InValid Client");
                }
                else
                {
                    ValidChildNTI.manualResetEvent.WaitOne();
                    valSocketInstance.Add(ValidChildNTI.socketInstance);
                    ValidChildNTI.socketInstance = null;
                    Debug.Log($"Valid Client");
                }

                ValidChildNTI.markForDone = true;
            }));
            ValidChildNTI.StartTask();
            allNTI[NTI_type.ValidChild].Add(ValidChildNTI);
        }


        private int clientCount = 0;

        public void ValidStart()
        {
            allNTI[NTI_type.Valid][0].StartTask();
        }

        public void AcceptStart()
        {
            allNTI[NTI_type.Accept][0].StartTask();
        }

        public void ManagerStart()
        {
            allNTI[NTI_type.Manager][0].StartTask();
        }


        private void OnDestroy()
        {
        }

        public void ClientConnect()
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Thread clientThread = new Thread(() =>
            {
                client.Connect(IPAddress.Parse("127.0.0.1"), 7000);
                Debug.Log("connect success");
                client.Send(Encoding.UTF8.GetBytes("Test"));
                byte[] bt = new byte[27];
                client.Receive(bt, 0, bt.Length, SocketFlags.None);
            });
            clientThread.IsBackground = true;
            clientThread.Start();
        }
    }
}