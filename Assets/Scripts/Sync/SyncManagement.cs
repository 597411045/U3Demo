using System.Collections.Generic;
using System.Threading;
using RGP.Cmd;
using RPG.Core;
using RPG.Scene;
using UnityEngine;

namespace PRG.Sync
{
    public class SyncManagement : TaskPipelineBase<SyncManagement>, ISendSyncObject, ISyncData, ISyncStats
    {
        private float timer = 0;

        public List<string> clientsSIID;

        private void Awake()
        {
            base.Awake();
            clientsSIID = new List<string>();
        }

        public void SendSyncObject()
        {
            if (timer <= 0)
            {
                foreach (var c in FindObjectsOfType<SyncObjectComponent>())
                {
                    if (c.ControllerSIID == "")
                    {
                        foreach (var d in c.GetComponents<ISyncObject>())
                        {
                            string json = d.BuildSyncObject();
                            if (json != "")
                            {
                                foreach (var e in clientsSIID)
                                {
                                    CMDSyncObject.Ins.Send(e, c.gameObject.name, json);
                                }
                            }
                        }
                    }
                }

                timer = 0.3f;
            }

            timer -= Time.deltaTime;
        }

        public void SyncStats()
        {
            if (timer <= 0)
            {
                foreach (var c in FindObjectsOfType<SyncObjectComponent>())
                {
                    if (c.ControllerSIID == "")
                    {
                        foreach (var d in c.GetComponents<ISyncObject>())
                        {
                            d.ApplySyncState();
                        }
                    }
                }
            }
        }

        public void SyncData()
        {
        }
    }
}