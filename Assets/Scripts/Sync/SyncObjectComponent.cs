using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRG.Sync
{
    public class SyncObjectComponent : MonoBehaviour
    {
        public string SIID;
        public Dictionary<string, ISyncObject> syncObjects;
        public bool isSyncControlled = true;

        private void Awake()
        {
            syncObjects = new Dictionary<string, ISyncObject>();
        }
    }
}