using RGP.Cmd;
using RPG.Core;

namespace PRG.Sync
{
    public class SyncManagement : TaskPipelineBase, ISendSyncObject, ISyncData, ISyncStats
    {
        public void SendSyncObject()
        {
            foreach (var c in FindObjectsOfType<SyncObjectComponent>())
            {
                if (c.enabled)
                {
                    foreach (var d in GetComponents<ISyncObject>())
                    {
                        CMDSyncObject.Ins.Send("ClientMainSocket", d.BuildSyncObject());
                    }
                }
            }
        }

        public void SyncStats()
        {
        }

        public void SyncData()
        {
        }
    }
}