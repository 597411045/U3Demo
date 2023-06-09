using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Network
{
    public enum CommunicationChildType
    {
        Send,
        Recv
    }


    public class CommunicationChildCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;

        public CommunicationChildCenter(SocketInstance s, CommunicationChildType c,string name) : base(name)
        {
            this.socketInstance = s;

            switch (c)
            {
                case CommunicationChildType.Recv:
                {
                    BuildRecvCommunicationChildNTI();
                    break;
                }
                case CommunicationChildType.Send:
                {
                    BuildSendCommunicationChildNTI();
                    break;
                }
            }

            InstanceCount++;
        }

        public void BuildRecvCommunicationChildNTI()
        {
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.LogError("Recv Start");
                while (true)
                {
                    this.manualResetEvent.WaitOne();

                    int length = this.socketInstance.socket.Receive(this.socketInstance.recvBuf, 0,
                        this.socketInstance.recvBuf.Length, SocketFlags.None);
                    if (length != 0)
                    {
                        //去0
                        //测试长度
                        int i = 0;
                        for (; i < this.socketInstance.recvBuf.Length;)
                        {
                            if (this.socketInstance.recvBuf[i] == 0) break;
                            i++;
                        }

                        //new btye[]
                        byte[] b = new byte[i];
                        for (int j = 0; j < i; j++)
                        {
                            b[j] = this.socketInstance.recvBuf[j];
                        }

                        socketInstance.recvList.Enqueue(b);
                        this.socketInstance.recvBuf = new byte[SocketInstance.length];
                    }
                }
            }),"BuildRecvCommunicationChildNTI");
            this.StartTask();
            NetworkCenter.allNTI[NTI_type.CommunicationChild].Add(this);
        }

        public void BuildSendCommunicationChildNTI()
        {
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                byte[] tmp;
                Debug.LogError("Send Start");
                while (true)
                {
                    this.manualResetEvent.WaitOne();
                    if (socketInstance.sendList.Count > 0)
                    {
                        tmp = socketInstance.sendList.Dequeue();
                        socketInstance.socket.Send(tmp, 0, tmp.Length, SocketFlags.None);
                    }
                    else
                    {
                        //Debug.LogError("socketInstance.sendList.Count < 0, Wait 5 seconds");
                        //Thread.Sleep(5000);
                    }
                }
            }),"BuildSendCommunicationChildNTI");
            this.StartTask();
            NetworkCenter.allNTI[NTI_type.CommunicationChild].Add(this);
        }
    }
}