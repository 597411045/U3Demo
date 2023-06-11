using System;
using System.Collections;
using Cinematic;
using PRG.Network;
using UnityEngine;

namespace RPG.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string saveFile = "save";

        public void StartGame()
        {
            StartCoroutine(IEStart());
        }

        IEnumerator IEStart()
        {
            Debug.Log("IE Start");

            yield return this.GetComponent<JsonSavingSystem>().LoadLastScene(saveFile);

            if (!NetworkManagement.isServer)
            {
                CameraShaderComponent csc = Camera.main.GetComponent<CameraShaderComponent>();
                yield return csc.FadeIn(1);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                LoadManual();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                Save();
            }
        }

        public void Save()
        {
            GetComponent<JsonSavingSystem>().Save(saveFile);
        }

        public void Load()
        {
            GetComponent<JsonSavingSystem>().Load(saveFile);
        }

        public void LoadManual()
        {
            StartCoroutine(IEStart());
        }
    }
}