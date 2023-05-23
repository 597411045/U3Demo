using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Scene
{
    public class PortalTriggerComponent : MonoBehaviour
    {
        [SerializeField] private int sceneToLoad = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                UpdateManager.UpdateActions.Clear();
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}