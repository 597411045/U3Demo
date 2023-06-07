using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class ValidCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;
        
        public ValidCenter() : base()
        {
            BuildValidNTI();
            this.name = "ValidCenter";
            InstanceCount++;
        }

        public void BuildValidNTI()
        {
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.LogError("ValidNTI Start");
                while (true)
                {
                    this.manualResetEvent.WaitOne();
                    if (NetworkCenter.tmpSocketInstance.Count > 0)
                    {
                        SocketInstance tmp = NetworkCenter.tmpSocketInstance.Dequeue();
                        ValidChildCenter vcc = new ValidChildCenter(tmp);
                        Debug.LogError("Build A ValidChildNTI");
                    }
                    else
                    {
                        //Debug.LogError("ValidNTI Wait 5 Second");
                        Thread.Sleep(5000);
                    }
                }
            }));

            NetworkCenter.allNTI[NTI_type.Valid].Add(this);
        }
    }
}