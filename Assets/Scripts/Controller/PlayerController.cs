using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf;
using Network;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Scene;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

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


    public class PlayerController : MonoBehaviour
    {
        private PTTransform ptt;

        public bool readySync = false;


        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] _cursorMappings;

        private NetworkCenter nc;
        StringBuilder sb;

        private void Awake()
        {
            if (NetworkCenter.isServer) return;
            UpdateManager.RegisterAction(UpdateMethod, this.gameObject.GetHashCode());
            ptt = new PTTransform();

            nc = GameObject.FindObjectOfType<NetworkCenter>();
            sb = new StringBuilder();
            SceneEntityManager.Entities.Add("123", this.gameObject);
        }

        private void Start()
        {
            if (!NetworkCenter.isServer)
            {
                this.gameObject.SetActive(false);
                CommandExecuter.SendLogin();
            }
        }

        void UpdateMethod()
        {
            if (this.enabled == false) return;

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

            if (CanDoCombat()) return;
            if (CanSetNavDestinationToCursor()) return;
            //SetCursor(CursorType.None);
        }

        private void Update()
        {
            if (readySync)
            {
                ptt.PositionX = this.transform.position.x;
                ptt.PositionY = this.transform.position.y;
                ptt.PositionZ = this.transform.position.z;
                ptt.AngleX = this.transform.eulerAngles.x;
                ptt.AngleY = this.transform.eulerAngles.y;
                ptt.AngleZ = this.transform.eulerAngles.z;

                Vector3 velocity = this.GetComponent<NavMeshAgent>().velocity;
                Vector3 localVelocity =
                    this.transform.InverseTransformDirection(velocity);

                ptt.Speed = localVelocity.z;
                NetworkCenter.ins.SendMessageBySocketUID("ClientMainSocket",
                    Encoding.UTF8.GetBytes("123|position|" + ptt.ToString()));
            }
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
    }
}