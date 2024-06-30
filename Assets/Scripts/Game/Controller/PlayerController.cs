using System;
using Cinemachine;
using DefaultNamespace;
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
        public GameObject LeftHandGrip;

        public GameObject weapon;

        private void Awake()
        {
            animator = this.GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("IfAttack");
                weapon.GetComponent<WeaponScrpit>().Fire();

                AnimatorClipInfo[] a = animator.GetCurrentAnimatorClipInfo(0);
                Debug.Log(a[0].clip.name);

                if (a[0].clip.name.Contains("Firing"))
                {
                    animator.Play("Firing", 0, 0);
                    ;
                }

                animator.GetCurrentAnimatorStateInfo(0);
            }
        }

        private Ray GerRayFromCursor()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        }

        public void FiringEnd()
        {
            animator.SetBool("IfAttack", false);
        }
    }
}