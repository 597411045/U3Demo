using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class ManagerCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;

        public ManagerCenter() : base()
        {
            BuildManagerNTI();
            this.name = "ManagerCenter";
            InstanceCount++;
        }

        private void BuildManagerNTI()
        {
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.Log("ManagerNTI Start");
                while (true)
                {
                    if (NetworkCenter.allNTI[NTI_type.ValidChild].Count > 0)
                    {
                        for (int i = NetworkCenter.allNTI[NTI_type.ValidChild].Count - 1; i >= 0; i--)
                        {
                            if (NetworkCenter.allNTI[NTI_type.ValidChild][i].threadInstance.GetRunningTime()
                                    .TotalSeconds > 5 ||
                                NetworkCenter.allNTI[NTI_type.ValidChild][i].markForDone)
                            {
                                NetworkCenter.allNTI[NTI_type.ValidChild][i].DestroyTask();
                                NetworkCenter.allNTI[NTI_type.ValidChild].RemoveAt(i);
                                Debug.Log("Remove A Timeout ValidChildNTI");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("ManagerNTI Wait 5 Second");
                        Debug.Log(NetworkCenter.valSocketInstance.Count);
                        Thread.Sleep(5000);
                    }
                }
            }));
            NetworkCenter.allNTI[NTI_type.Manager].Add(this);
        }
    }
}