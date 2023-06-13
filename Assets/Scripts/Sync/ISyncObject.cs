using System.Collections.Generic;

namespace PRG.Sync
{
    public interface ISyncObject
    {
        public string BuildSyncObject();

        public void ApplySyncState();

        public void ApplySyncData();

        public PTTransform SyncObject { get; set; }

        public void RegisterToSyncComponent();

        public Queue<string> GetSyncBuffer();
    }
}