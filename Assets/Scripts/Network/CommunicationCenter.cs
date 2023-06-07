using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class CommunicationCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;

        public static Dictionary<int, Dictionary<CommunicationChildType, NetTaskInstance>> clientCommunications =
            new Dictionary<int, Dictionary<CommunicationChildType, NetTaskInstance>>();

        public CommunicationCenter() : base()
        {
            BuildCommunicationNTI();
            this.name = "CommunicationCenter";
            InstanceCount++;
        }

        public void BuildCommunicationNTI()
        {
            Debug.LogError("Build Comm");
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                SocketInstance tmp;
                while (true)
                {
                    manualResetEvent.WaitOne();
                    if (NetworkCenter.valSocketInstance.Count > 0)
                    {
                        tmp = NetworkCenter.valSocketInstance.Dequeue();
                        Dictionary<CommunicationChildType, NetTaskInstance> tmpDic =
                            new Dictionary<CommunicationChildType, NetTaskInstance>();
                        tmpDic.Add(CommunicationChildType.Recv,
                            new CommunicationChildCenter(tmp, CommunicationChildType.Recv));
                        tmpDic.Add(CommunicationChildType.Send,
                            new CommunicationChildCenter(tmp, CommunicationChildType.Send));

                        if (tmp.UID == 0)
                        {
                            throw new Exception("Maybe UID not assigned");
                        }
                        clientCommunications.Add(tmp.UID, tmpDic);
                    }
                    else
                    {
                        //Debug.LogError("NetworkCenter.valSocketInstance.Count < 0, Wait 5 Seconds");
                        Thread.Sleep(5000);
                    }
                }
            }));
            NetworkCenter.allNTI[NTI_type.Communication].Add(this);
        }
    }
}