using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentClassComponent : MonoBehaviour
    {
        [SerializeField] private GameObject go;
        [SerializeField] private GameObject go1;
        [SerializeField] private GameObject go2;

        private static bool hasSpawn;

        private void Awake()
        {
            if (hasSpawn) return;
            SpawnObject();
        }

        

        private void SpawnObject()
        {
            hasSpawn = true;
            if (go != null)
            {
                DontDestroyOnLoad(Instantiate(go));
            }

            if (go1 != null)
            {
                DontDestroyOnLoad(Instantiate(go1));
            }

            if (go2 != null)
            {
                DontDestroyOnLoad(Instantiate(go2));
            }
        }
    }
}