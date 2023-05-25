using System;
using System.Collections;
using Cinematic;
using UnityEngine;

namespace RPG.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string saveFile = "save";


        IEnumerator Start()
        {
            
            Debug.Log("IE Start");
            
            CameraShaderComponent csc = Camera.main.GetComponent<CameraShaderComponent>();
            csc.contrast = 0;
            
            UpdateManager.UpdateActions.Clear();
            yield return this.GetComponent<JsonSavingSystem>().LoadLastScene(saveFile);
            
            CameraShaderComponent csc2 = Camera.main.GetComponent<CameraShaderComponent>();
            yield return csc2.FadeIn(1);
        }

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

        public void Save()
        {
            GetComponent<JsonSavingSystem>().Save(saveFile);
        }

        public void Load()
        {
            GetComponent<JsonSavingSystem>().Load(saveFile);
        }
    }
}