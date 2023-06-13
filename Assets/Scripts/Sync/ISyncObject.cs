namespace PRG.Sync
{
    public interface ISyncObject
    {
        public string BuildSyncObject();

        public void ApplySyncStata();

        public void ApplySyncData();

        public PTTransform SyncObject { get; set; }

        public void RegisterToSyncComponent();
    }
}