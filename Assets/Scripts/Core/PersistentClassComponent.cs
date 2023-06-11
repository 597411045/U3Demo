using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentClassComponent : MonoBehaviour
    {
        [SerializeField] List<GameObject> gos;

        private static bool hasSpawn;

        private void Awake()
        {
            if (hasSpawn) return;
            SpawnObject();
        }


        private void SpawnObject()
        {
            hasSpawn = true;
            foreach (var c in gos)
            {
                if (c != null)
                    DontDestroyOnLoad(Instantiate(c));
            }
        }
    }
}