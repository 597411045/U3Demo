using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PRG.Network
{
    public class NTICommCenter : NetTaskInstance
    {
        public static int InstanceCount = 0;

        public Dictionary<string, Dictionary<CommunicationChildType, NetTaskInstance>> clientCommunications;

        public NTICommCenter(string name) : base(name)
        {
            BuildCommunicationNTI();
            InstanceCount++;

            clientCommunications =
                new Dictionary<string, Dictionary<CommunicationChildType, NetTaskInstance>>();
        }

        public void BuildCommunicationNTI()
        {
            this.threadInstance = new ThreadInstance(new Thread(() =>
            {
                Debug.LogError("BuildCommunicationNTI Start");

                SocketInstance tmp;
                while (true)
                {
                    manualResetEvent.WaitOne();
                    tmp = NetworkManagement.Ins.DequeueSI();
                    if (tmp != null)
                    {
                        Dictionary<CommunicationChildType, NetTaskInstance> tmpDic =
                            new Dictionary<CommunicationChildType, NetTaskInstance>();
                        tmpDic.Add(CommunicationChildType.Recv,
                            new NTICommChild(tmp, CommunicationChildType.Recv, tmp.UID + " Recv"));
                        tmpDic.Add(CommunicationChildType.Send,
                            new NTICommChild(tmp, CommunicationChildType.Send, tmp.UID + " Send"));

                        clientCommunications.Add(tmp.UID, tmpDic);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }), "BuildCommunicationNTI");
            StartTask();
            NetworkManagement.Ins.AddNTI(NTI_type.Communication, this);
        }
    }
}