using System.Threading;
using RGP.Cmd;
using RPG.Core;
using UnityEngine;

namespace PRG.Sync
{
    public class SyncManagement : TaskPipelineBase<SyncManagement>, ISendSyncObject, ISyncData, ISyncStats
    {
        private float timer = 0;

        private void Awake()
        {
            base.Awake();
        }

        public void SendSyncObject()
        {
            if (timer <= 0)
            {
                foreach (var c in FindObjectsOfType<SyncObjectComponent>())
                {
                    Debug.LogError(c.enabled);
                    if (c.enabled && c.SIID != "")
                    {
                        foreach (var d in c.syncObjects)
                        {
                            CMDSyncObject.Ins.Send("ClientMainSocket", d.Value.BuildSyncObject());
                        }
                    }
                }

                timer = 5;
            }

            timer -= Time.deltaTime;
        }

        public void SyncStats()
        {
            if (timer <= 0)
            {
                Debug.LogError("SyncStats");
            }
        }

        public void SyncData()
        {
            if (timer <= 0)
            {
                Debug.LogError("SyncData");
            }
        }
    }
}