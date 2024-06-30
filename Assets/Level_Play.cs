using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level_Play : MonoBehaviour
{
    public GameObject[] SpawnPoint;
    private bool flag1;
    private bool flag2;

    public Camera camera;

    private void Awake()
    {
        camera = Camera.main;
        flag1 = true;
        flag2 = true;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }


    private void OnGUI()
    {
        if (GUILayout.Button("Initial a Button"))
        {
            if (!flag1) return;
            GameObject go = GeneratePlayerAt(SpawnPoint[Random.Range(0, SpawnPoint.Length)].transform.position);
            InitialPlayerComponent(go);
            flag1 = false;
        }

        if (GUILayout.Button("Initial a AI"))
        {
            if (!flag2) return;
            GameObject ai = GeneratePlayerAt(SpawnPoint[Random.Range(0, SpawnPoint.Length)].transform.position);
            DisAbleComponent(ai);

            flag2 = false;
        }
    }

    public GameObject GeneratePlayerAt(Vector3 v)
    {
        return Instantiate(Resources.Load<GameObject>("FPSPlayer"), v,
            quaternion.identity);
    }

    public void InitialPlayerComponent(GameObject player)
    {
        Transform cubeT;
        player.transform.FindAlongChild("Cube", out cubeT);
        player.GetComponent<NavMoveComponent>().camera = camera.gameObject;
        camera.GetComponent<CameraScript>().SecondViewPoint = cubeT.gameObject;
        camera.GetComponent<CameraScript>().Player = player;

        player.GetComponent<PlayerController>().enabled = true;
        camera.GetComponent<CameraScript>().enabled = true;
        Transform tmpT;
        if (player.transform.FindAlongChild("ArrowEnd", out tmpT))
        {
            camera.GetComponent<CameraScript>().WeaponFirePoint = tmpT.gameObject;
        }

        player.GetComponent<NavMoveComponent>().enabled = true;
    }

    public void DisAbleComponent(GameObject player)
    {
        player.GetComponent<NavMoveComponent>().enabled = false;
        //camera.GetComponent<CameraScript>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
    }
}