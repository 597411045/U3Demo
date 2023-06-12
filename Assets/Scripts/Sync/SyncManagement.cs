using System.Threading;
using RGP.Cmd;
using RPG.Core;
using UnityEngine;

namespace PRG.Sync
{
    public class SyncManagement : TaskPipelineBase, ISendSyncObject, ISyncData, ISyncStats
    {
        private float timer = 0;

        public void SendSyncObject()
        {
            if (timer <= 0)
            {
                foreach (var c in FindObjectsOfType<SyncObjectComponent>())
                {
                    Debug.LogError(c.enabled);
                    if (c.enabled)
                    {
                        foreach (var d in GetComponents<ISyncObject>())
                        {
                            //CMDSyncObject.Ins.Send("ClientMainSocket", d.BuildSyncObject());
                            Debug.LogError("SendSyncObject");
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