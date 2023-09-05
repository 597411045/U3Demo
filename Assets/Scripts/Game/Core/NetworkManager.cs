using System;
using CS.Network;
using UnityEngine;

namespace RPG.Core
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager SingleTon;
        public NetworkManagement nm;

        public NetworkManager()
        {
            if (SingleTon == null)
            {
                SingleTon = this;
                nm = new NetworkManagement(NTI_type.Client, "81.68.87.60", 7000);
            }
        }

        public void Process()
        {
            nm.AutoProcess();
        }
        
        private void Start()
        {
        }
    }
}