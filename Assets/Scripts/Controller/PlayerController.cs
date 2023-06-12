using System;
using Cinemachine;
using PRG.Network;
using RPG.Cmd;
using RPG.Core;
using UnityEngine;
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


    public class PlayerController : TaskPipelineBase, ILocalCompute
    {
        private GameObject cineMachine;

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
                cineMachine.GetComponent<CinemachineVirtualCamera>().Follow = this.gameObject.transform;
                if (!SceneManager.GetActiveScene().name.Contains("Solo"))
                {
                    TaskPipelineManager.Ins.Register(() =>
                    {
                        CMDSyncRequest.Ins.Send("ClientMainSocket", this.gameObject.name);
                    });
                }
            }
        }

        private Vector3 oldMousePosition;
        private Vector2 mouseMoveDelta;

        public void OnLocalCompute()
        {
            if (this.enabled == false) return;
            if (!this.gameObject.activeInHierarchy) return;
            if (NetworkManagement.isServer) return;

            if (cineMachine != null)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    oldMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                }

                if (Input.GetMouseButton(1))
                {
                    mouseMoveDelta = (Camera.main.ScreenToViewportPoint(Input.mousePosition) - oldMousePosition);
                    cineMachine.transform.Rotate(Vector3.up, mouseMoveDelta.x * 50, Space.World);
                    cineMachine.transform.Rotate(Vector3.right, -mouseMoveDelta.y * 50);
                    cineMachine.transform.eulerAngles = new Vector3(
                        Mathf.Clamp(cineMachine.transform.eulerAngles.x, 10, 60), cineMachine.transform.eulerAngles.y,
                        cineMachine.transform.eulerAngles.z);

                    oldMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                }
            }


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