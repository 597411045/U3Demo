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

        public IEnumerator FadeOut(float time)
        {
            contrast = 1;
            while (contrast >= 0)
            {
                contrast -= Time.deltaTime / time;
                yield return null;
            }
            contrast = 0;

        }

        public IEnumerator FadeIn(float time)
        {
            contrast = 0;
            while (contrast <= 1)
            {
                contrast += Time.deltaTime / time;
                yield return null;
            }
            contrast = 1;
        }
    }
}