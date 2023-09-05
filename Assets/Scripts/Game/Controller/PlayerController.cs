using System;
using Cinemachine;
using Game.Item;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private GameObject cineMachine;
        private Animator animator;


        private void Awake()
        {
            animator = this.GetComponent<Animator>();
            cineMachine = GameObject.Find("CM FreeLook1");
        }

        private Ray GerRayFromCursor()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

    }
}