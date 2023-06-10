using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
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

        public CommunicationCenter(string name) : base(name)
        {
            BuildCommunicationNTI();
            InstanceCount++;

            clientCommunications =
                new Dictionary<string, Dictionary<CommunicationChildType, NetTaskInstance>>();
        }

        public void BuildCommunicationNTI()
        {
            Debug.LogError("Comm Start");
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
                            new CommunicationChildCenter(tmp, CommunicationChildType.Recv, tmp.UID + " Recv"));
                        tmpDic.Add(CommunicationChildType.Send,
                            new CommunicationChildCenter(tmp, CommunicationChildType.Send, tmp.UID + " Send"));

                        clientCommunications.Add(tmp.UID, tmpDic);
                    }
                    else
                    {
                        //Debug.LogError("NetworkCenter.valSocketInstance.Count < 0, Wait 5 Seconds");
                        Thread.Sleep(1000);
                    }
                }
            }), "BuildCommunicationNTI");
            StartTask();
            NetworkCenter.allNTI[NTI_type.Communication].Add(this);
        }
    }
}