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

        //改为实例，由Network调用
        //public static Dictionary<string, Dictionary<CommunicationChildType, NetTaskInstance>> clientCommunications =
        //    new Dictionary<string, Dictionary<CommunicationChildType, NetTaskInstance>>();
        public Dictionary<string, Dictionary<CommunicationChildType, NetTaskInstance>> clientCommunications;

        public CommunicationCenter() : base()
        {
            BuildCommunicationNTI();
            this.name = "CommunicationCenter";
            InstanceCount++;

            clientCommunications =
                new Dictionary<string, Dictionary<CommunicationChildType, NetTaskInstance>>();
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
                        tmp.recvList = new Queue<byte[]>();
                        tmp.sendList = new Queue<byte[]>();
                        tmp.recvBuf = new byte[SocketInstance.length];
                        tmp.sendBuf = new byte[SocketInstance.length];

                        Dictionary<CommunicationChildType, NetTaskInstance> tmpDic =
                            new Dictionary<CommunicationChildType, NetTaskInstance>();
                        tmpDic.Add(CommunicationChildType.Recv,
                            new CommunicationChildCenter(tmp, CommunicationChildType.Recv));
                        tmpDic.Add(CommunicationChildType.Send,
                            new CommunicationChildCenter(tmp, CommunicationChildType.Send));

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