namespace PRG.Sync
{
    public interface ISyncObject
    {
        public string BuildSyncObject();

        public void ApplySyncStata();

        public void ApplySyncData();
    }
}