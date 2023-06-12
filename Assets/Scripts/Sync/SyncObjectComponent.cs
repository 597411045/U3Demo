using System;
using UnityEngine;

namespace PRG.Sync
{
    public class SyncObjectComponent : MonoBehaviour
    {
        private void Awake()
        {
            this.enabled = false;
        }
    }
}