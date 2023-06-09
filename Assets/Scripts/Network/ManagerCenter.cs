using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    //用于管理清除无效的Thread
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
                Debug.LogError("ManagerNTI Start");
                while (true)
                {
                    //暂时取消SocketValid功能，合并值Command和Communication系统
                    
                    // if (NetworkCenter.allNTI[NTI_type.ValidChild].Count > 0)
                    // {
                    //     for (int i = NetworkCenter.allNTI[NTI_type.ValidChild].Count - 1; i >= 0; i--)
                    //     {
                    //         if (NetworkCenter.allNTI[NTI_type.ValidChild][i].threadInstance.GetRunningTime()
                    //                 .TotalSeconds > 10 ||
                    //             NetworkCenter.allNTI[NTI_type.ValidChild][i].markForDone)
                    //         {
                    //             NetworkCenter.allNTI[NTI_type.ValidChild][i].DestroyTask();
                    //             NetworkCenter.allNTI[NTI_type.ValidChild].RemoveAt(i);
                    //             Debug.LogError("Remove A Timeout ValidChildNTI");
                    //         }
                    //     }
                    // }else 
                    if (NetworkCenter.allNTI[NTI_type.Connect].Count > 0)
                    {
                        for (int i = NetworkCenter.allNTI[NTI_type.Connect].Count - 1; i >= 0; i--)
                        {
                            if (NetworkCenter.allNTI[NTI_type.Connect][i].markForDone)
                            {
                                NetworkCenter.allNTI[NTI_type.Connect][i].DestroyTask();
                                NetworkCenter.allNTI[NTI_type.Connect].RemoveAt(i);
                                Debug.LogError("Remove A MarkForDone ConnectNTI");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("ManagerNTI Wait 5 Second");
                        Thread.Sleep(5000);
                    }
                }
            }));
            NetworkCenter.allNTI[NTI_type.Manager].Add(this);
        }
    }
}