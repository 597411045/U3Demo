using System;
using System.Collections;
using Cinematic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.Scene
{
    public class PortalTriggerComponent : MonoBehaviour
    {
        [SerializeField] private int sceneToLoad = -1;
        [SerializeField] private GameObject spawnPoint;
        [SerializeField] private int toPortalId = 0;
        [SerializeField] private int portalId = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                StartCoroutine(ChangeSceneAsync());
            }
        }

        IEnumerator ChangeSceneAsync()
        {
            UpdateManager.UpdateActions.Clear();
            DontDestroyOnLoad(gameObject);

            
            FindObjectOfType<SavingWrapper>().Save();
            
            StopCoroutine("FadeIn");
            CameraShaderComponent csc = Camera.main.GetComponent<CameraShaderComponent>();
            yield return csc.FadeOut(1);

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            FindObjectOfType<SavingWrapper>().Load();

            SetPlayerPosition();

            CameraShaderComponent csc2 = Camera.main.GetComponent<CameraShaderComponent>();
            yield return csc2.FadeIn(0.5f);

            FindObjectOfType<SavingWrapper>().Save();
            Destroy(gameObject);
        }

        private void SetPlayerPosition()
        {
            GameObject player = GameObject.FindWithTag("Player");
            foreach (var child in FindObjectsOfType<PortalTriggerComponent>())
            {
                if (child.portalId == toPortalId)
                {
                    player.GetComponent<NavMeshAgent>().Warp(child.spawnPoint.transform.position);
                    player.transform.rotation = child.spawnPoint.transform.rotation;
                    return;
                }
            }
        }
    }
}