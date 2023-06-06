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

        public CommunicationChildCenter(SocketInstance s, CommunicationChildType c) : base()
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

            this.name = "CommunicationChildCenter";
            InstanceCount++;

            s.recvList = new Queue<byte[]>();
            s.sendList = new Queue<byte[]>();
        }

        public void BuildRecvCommunicationChildNTI()
        {
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.Log("CommunicationChildNTI Start");
                while (true)
                {
                    this.manualResetEvent.WaitOne();

                    int length = this.socketInstance.socket.Receive(this.socketInstance.recvBuf, 0,
                        this.socketInstance.recvBuf.Length, SocketFlags.None);
                    if (length != 0)
                    {
                        socketInstance.recvList.Enqueue(this.socketInstance.recvBuf);
                        this.socketInstance.recvBuf = new byte[SocketInstance.length];
                    }
                }
            }));
            this.StartTask();
            NetworkCenter.allNTI[NTI_type.CommunicationChild].Add(this);
        }

        public void BuildSendCommunicationChildNTI()
        {
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                byte[] tmp;
                Debug.Log("CommunicationChildNTI Start");
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
                        Debug.Log("socketInstance.sendList.Count < 0, Wait 5 seconds");
                        Thread.Sleep(5000);
                    }
                }
            }));
            this.StartTask();
            NetworkCenter.allNTI[NTI_type.CommunicationChild].Add(this);
        }
    }
}