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
        [SerializeField] private string toSceneName;
        [SerializeField] private GameObject spawnPoint;
        [SerializeField] private string toPortalName;
        [SerializeField] private string PortalName;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                StartCoroutine(ChangeSceneAsync());
            }
        }

        IEnumerator ChangeSceneAsync()
        {
            UpdateManager.Ins.ClearAllLocalCompute();
            DontDestroyOnLoad(gameObject);


            FindObjectOfType<SavingWrapper>().Save();

            StopCoroutine("FadeIn");

            CameraShaderComponent csc = Camera.main.GetComponent<CameraShaderComponent>();
            csc.StopImme = true;
            yield return new WaitForSeconds(0.02f);
            yield return csc.Fade(0, 2);

            yield return SceneManager.LoadSceneAsync(toSceneName);

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
                if (child.PortalName == toPortalName)
                {
                    player.GetComponent<NavMeshAgent>().Warp(child.spawnPoint.transform.position);
                    player.transform.rotation = child.spawnPoint.transform.rotation;
                    return;
                }
            }
        }
    }
}