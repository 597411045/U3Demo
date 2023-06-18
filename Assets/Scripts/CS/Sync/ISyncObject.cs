using System.Collections.Generic;
using ProtoMsg;

namespace PRG.Sync
{
    public interface ISyncObject
    {
        public string BuildSyncObject();

        public void ApplySyncState();

        public void ApplySyncData();

        public PTTransform SyncObject { get; set; }

        public Queue<string> GetSyncBuffer();
    }
}