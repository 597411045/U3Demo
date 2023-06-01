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
            UpdateManager.ClearAllActions();
            DontDestroyOnLoad(gameObject);


            FindObjectOfType<SavingWrapper>().Save();

            StopCoroutine("FadeIn");

            CameraShaderComponent csc = Camera.main.GetComponent<CameraShaderComponent>();
            csc.StopImme = true;
            yield return new WaitForSeconds(0.02f);
            yield return csc.Fade(0, 2);

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            FindObjectOfType<SavingWrapper>().Load();

            SetPlayerPosition();

            FindObjectOfType<SavingWrapper>().Save();

            CameraShaderComponent csc2 = Camera.main.GetComponent<CameraShaderComponent>();
            csc2.contrast = 0;
            yield return csc2.Fade(1, 2);

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