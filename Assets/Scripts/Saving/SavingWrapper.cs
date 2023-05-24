using System;
using UnityEngine;

namespace RPG.Saving
{
    public class SavingWrapper:MonoBehaviour
    {
        private const string saveFile = "save";
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Save();
            }
        }

        private void Save()
        {
            GetComponent<JsonSavingSystem>().Save(saveFile);
        }

        private void Load()
        {
            GetComponent<JsonSavingSystem>().Load(saveFile);
        }
    }
}