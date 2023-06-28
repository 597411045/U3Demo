using System;
using Cinemachine;
using Game.Item;
using PRG.Network;
using PRG.Sync;
using RPG.Cmd;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RPG.Control
{
    public enum CursorType
    {
        None,
        Movement,
        Combat,
        UI,
        PickUp,
        Deny,
    }

    
    public class PlayerController : ControllerBase, ILocalCompute
    {
        private GameObject cineMachine;
        private Animator animator;


        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] _cursorMappings;

        private void Awake()
        {
            if (!NetworkManagement.isServer)
            {
                cineMachine = GameObject.Find("CM vcam1");
                animator = this.GetComponent<Animator>();
                cineMachine.GetComponent<CinemachineVirtualCamera>().Follow = this.gameObject.transform;
            }
        }

        private Vector3 oldMousePosition;
        private Vector2 mouseMoveDelta;

        public void OnLocalCompute()
        {
            if (this.enabled == false) return;
            if (!this.gameObject.activeInHierarchy) return;
            if (this.GetComponent<SyncObjectComponent>().ControllerSIID != "") return;


            AnimatorClipInfo[] a = animator.GetCurrentAnimatorClipInfo(0);
            // int b = animator.GetCurrentAnimatorClipInfoCount(0);
            // AnimatorStateInfo c = animator.GetCurrentAnimatorStateInfo(0);
            // AnimatorTransitionInfo d = animator.GetAnimatorTransitionInfo(0);
            // RuntimeAnimatorController e = animator.runtimeAnimatorController;
            // AnimationClip[] f = e.animationClips;

            this.GetComponent<NavMoveComponent>().ActMoveUpdating();


            //Camera
            if (cineMachine != null)
            {
                //右键旋转模式，通过鼠标视口的位置判断，问题：超出视口无效
                if (false && Input.GetMouseButtonDown(1))
                {
                    oldMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                }

                if (false && Input.GetMouseButton(1))
                {
                    mouseMoveDelta = (Camera.main.ScreenToViewportPoint(Input.mousePosition) - oldMousePosition);
                    cineMachine.transform.Rotate(Vector3.up, mouseMoveDelta.x * 50, Space.World);
                    cineMachine.transform.Rotate(Vector3.right, -mouseMoveDelta.y * 50);
                    cineMachine.transform.eulerAngles = new Vector3(
                        Mathf.Clamp(cineMachine.transform.eulerAngles.x, 10, 60), cineMachine.transform.eulerAngles.y,
                        cineMachine.transform.eulerAngles.z);

                    oldMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                }

                //全局旋转模式，通过GetAxis
                if (Mathf.Abs(Input.GetAxis("Mouse X")) >= 0.1f)
                {
                    cineMachine.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * 10,
                        Space.World);
                }

                if (Mathf.Abs(Input.GetAxis("Mouse Y")) >= 0.1f)
                {
                    cineMachine.transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * Time.deltaTime * 10);
                    cineMachine.transform.eulerAngles = new Vector3(
                        Mathf.Clamp(cineMachine.transform.eulerAngles.x, 10, 60), cineMachine.transform.eulerAngles.y,
                        cineMachine.transform.eulerAngles.z);
                }

                //锁鼠标
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetMouseButton(0))
            {
                this.GetComponent<FighterActionComponent>().ActAttack();
            }

            //老操作模式，纯鼠标操作，以后可用于纯ui操作界面：
            if (false)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    SetCursor(CursorType.UI);
                    return;
                }

                RaycastHit[] hit = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
                for (int i = 0; i < hit.Length; i++)
                {
                    for (int j = i + 1; j < hit.Length; j++)
                    {
                        if (hit[i].distance > hit[j].distance)
                        {
                            (hit[j], hit[i]) = (hit[i], hit[j]);
                        }
                    }

                    if (hit[i].transform.GetComponent<IRayCastAble>() != null)
                    {
                        if (hit[i].transform.GetComponent<IRayCastAble>().HandleRaycaset(this, hit[i]))
                        {
                            return;
                        }
                    }
                }
            }

            //if (CanDoCombat()) return;
            //if (CanSetNavDestinationToCursor()) return;
            //SetCursor(CursorType.None);
        }

        private bool CanDoCombat()
        {
            // if (Input.GetMouseButtonDown(0))
            // {
            //     SetCursor(CursorType.Combat);
            //     RaycastHit[] raycastHits = Physics.RaycastAll(GerRayFromCursor());
            //     foreach (RaycastHit hit in raycastHits)
            //     {
            //         CombatAbleComponent cac = hit.transform.GetComponent<CombatAbleComponent>();
            //         if (cac == null) continue;
            //         if (this.GetComponent<FighterActionComponent>().TryMakeTargetBeAttackTarget(cac))
            //         {
            //             return true;
            //         }
            //     }
            // }

            return false;
        }

        public void SetCursor(CursorType e)
        {
            CursorMapping m = GetCursorMapping(e);

            Cursor.SetCursor(m.texture, m.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var c in _cursorMappings)
            {
                if (c.type == type)
                {
                    return c;
                }
            }

            return _cursorMappings[0];
        }


        private bool CanSetNavDestinationToCursor()
        {
// s            if (Input.GetMouseButtonDown(0))
//             {
//                 SetCursor(CursorType.Movement);
//
//                 RaycastHit raycastHit;
//                 bool ifHit = Physics.Raycast(GerRayFromCursor(), out raycastHit);
//                 if (ifHit)
//                 {
//                     this.GetComponent<NavMoveComponent>().StartMoveToPosition(raycastHit.point);
//                     return true;
//                 }
//             }

            return false;
        }

        private Ray GerRayFromCursor()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public void LocalCompute()
        {
            OnLocalCompute();
        }
    }
}