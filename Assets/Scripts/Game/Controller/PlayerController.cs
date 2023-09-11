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
    public enum CameraMode
    {
        Third,
        First
    }

    public class PlayerController : MonoBehaviour
    {
        private GameObject camera;
        private Animator animator;


        private void Awake()
        {
            animator = this.GetComponent<Animator>();
        }

        private Ray GerRayFromCursor()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}