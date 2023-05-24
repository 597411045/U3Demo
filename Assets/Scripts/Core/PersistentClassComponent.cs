using System;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentClassComponent : MonoBehaviour
    {
        [SerializeField] private GameObject go;
        [SerializeField] private GameObject go1;

        private static bool hasSpawn;

        private void Awake()
        {
            if (hasSpawn) return;
            SpawnObject();
        }

        private void SpawnObject()
        {
            DontDestroyOnLoad(Instantiate(go));
            DontDestroyOnLoad(Instantiate(go1));
        }
    }
}