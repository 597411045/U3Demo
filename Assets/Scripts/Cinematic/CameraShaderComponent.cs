using System;
using System.Collections;
using UnityEngine;

namespace Cinematic
{
    public class CameraShaderComponent : MonoBehaviour
    {
        [Range(0, 100)] public float brightness = 1;
        [Range(0, 100)] public float saturation = 1;
        [Range(0, 100)] public float contrast = 1;

        public Shader shader;
        private Material mat;

        public bool StopImme;

        private void Awake()
        {
            mat = new Material(shader);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (mat != null)
            {
                mat.SetFloat("_Brightness", brightness);
                mat.SetFloat("_Saturation", saturation);
                mat.SetFloat("_Contrast", contrast);
                Graphics.Blit(src, dest, mat);
            }
        }

        public IEnumerator Fade(float value, float time)
        {
            StopImme = false;
            while (!Mathf.Approximately(contrast, value) && StopImme == false)
            {
                contrast = Mathf.MoveTowards(contrast, value, Time.deltaTime / time);
                yield return null;
            }
        }

        public IEnumerator FadeOut(float time)
        {
            contrast = 1;
            StopImme = false;
            while (contrast >= 0 && StopImme == false)
            {
                contrast -= Time.deltaTime / time;
                yield return null;
            }

            contrast = 0;
        }

        public IEnumerator FadeIn(float time)
        {
            contrast = 0;
            StopImme = false;
            while (contrast <= 1 && StopImme == false)
            {
                contrast += Time.deltaTime / time;
                yield return null;
            }

            contrast = 1;
        }
    }
}