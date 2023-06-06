using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class ValidChildCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;
        public static int EasyID = 1;

        public ValidChildCenter(SocketInstance s) : base()
        {
            this.socketInstance = s;
            BuildValidChildNTI();
            this.name = "ValidChildCenter";
            InstanceCount++;
        }

        public void BuildValidChildNTI()
        {
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.Log("ValidChildNTI Start");
                int length = this.socketInstance.socket.Receive(this.socketInstance.recvBuf, 0,
                    this.socketInstance.recvBuf.Length, SocketFlags.None);
                Debug.Log($"Reveived {length} bytes");
                if (length != 123)
                {
                    Debug.Log($"InValid Client");
                }
                else
                {
                    this.manualResetEvent.WaitOne();
                    this.socketInstance.UID = EasyID++;
                    NetworkCenter.valSocketInstance.Enqueue(this.socketInstance);
                    this.socketInstance = null;
                    Debug.Log($"Valid Client");
                }

                this.socketInstance.recvBuf = new byte[SocketInstance.length];

                this.markForDone = true;
            }));
            this.StartTask();
            NetworkCenter.allNTI[NTI_type.ValidChild].Add(this);
        }
    }
}