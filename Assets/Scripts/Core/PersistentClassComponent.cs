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
            DontDestroyOnLoad(Instantiate(go));
            DontDestroyOnLoad(Instantiate(go1));
            DontDestroyOnLoad(Instantiate(go2));
        }
    }
}