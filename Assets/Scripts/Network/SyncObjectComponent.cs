using System;
using UnityEngine;

namespace PRG.Network
{
    public class SyncObjectComponent : MonoBehaviour
    {
        private void Awake()
        {
            this.enabled = false;
        }
    }
}