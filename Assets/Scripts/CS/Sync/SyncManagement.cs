using System.Collections.Generic;
using System.Threading;
using RGP.Cmd;
using RPG.Core;
using RPG.Scene;
using UnityEngine;
using UnityEngine.Serialization;

namespace PRG.Sync
{
    public class SyncManagement : TaskPipelineBaseWithSTMN<SyncManagement>, ISendSyncObject, ISyncData, ISyncStats
    {
        private float timer = 0;

        [FormerlySerializedAs("syncSSID")] public List<string> syncSIID;

        private void Awake()
        {
            base.Awake();
            syncSIID = new List<string>();
        }

        public void SendSyncObject()
        {
            if (timer <= 0)
            {
                foreach (var c in FindObjectsOfType<SyncObjectComponent>())
                {
                    if (c.enabled && c.ControllerSIID == "")
                    {
                        foreach (var d in c.GetComponents<ISyncObject>())
                        {
                            string json = d.BuildSyncObject();
                            if (json != "")
                            {
                                foreach (var e in syncSIID)
                                {
                                    CMDSyncObject.Ins.Send(e, c.gameObject.name, json);
                                }
                            }
                        }
                    }
                }

                timer = 1f;
            }

            timer -= Time.deltaTime;
        }

        public void SyncStats()
        {
            if (timer <= 0)
            {
                foreach (var c in FindObjectsOfType<SyncObjectComponent>())
                {
                    if (c.ControllerSIID != "")
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
            if (timer <= 0)
            {
                foreach (var c in FindObjectsOfType<SyncObjectComponent>())
                {
                    if (c.ControllerSIID != "")
                    {
                        foreach (var d in c.GetComponents<ISyncObject>())
                        {
                            d.ApplySyncData();
                        }
                    }
                }
            }
        }
    }
}