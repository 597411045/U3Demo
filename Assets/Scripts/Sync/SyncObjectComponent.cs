using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PRG.Sync
{
    public class SyncObjectComponent : MonoBehaviour
    {
        public string ControllerSIID;
        public string PrefabName;

        private void Awake()
        {
        }
    }
}