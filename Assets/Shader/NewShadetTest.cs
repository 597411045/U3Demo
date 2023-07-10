using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class NewShadetTest : MonoBehaviour
{
    private Material mat;
    private Shader shad;

    private float totalTime;

    private void Awake()
    {
        mat = this.GetComponent<SkinnedMeshRenderer>().material;
        shad = mat.shader;
    }

    private void Update()
    {
        totalTime += Time.deltaTime;
        if (mat != null)
        {
            mat.SetFloat("_DeltaTime", totalTime);
            mat.SetVector("_CamPosition", Camera.main.transform.position);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
    }
}