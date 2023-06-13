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

        private void Awake()
        {
            base.Awake();
            GameObject go = SceneEntityManager.GenerateEnemyPrefab();
            go.GetComponent<SyncObjectComponent>().isSyncControlled = false;
        }

        public void SendSyncObject()
        {
            if (timer <= 0)
            {
                foreach (var c in FindObjectsOfType<SyncObjectComponent>())
                {
                    if (!c.isSyncControlled && c.SIID != "")
                    {
                        foreach (var d in c.syncObjects)
                        {
                            string json = d.Value.BuildSyncObject();
                            if (json != "")
                            {
                                CMDSyncObject.Ins.Send(c.SIID, c.gameObject.name, json);
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
                    if (c.enabled && c.SIID != "")
                    {
                        foreach (var d in c.syncObjects)
                        {
                            d.Value.ApplySyncData();
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
                    if (c.enabled && c.SIID != "")
                    {
                        foreach (var d in c.syncObjects)
                        {
                            d.Value.ApplySyncData();
                        }
                    }
                }
            }
        }
    }
}