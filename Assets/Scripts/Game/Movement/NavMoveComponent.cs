using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PRG.Network;
using PRG.Sync;
using ProtoMsg;
using RPG.Control;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class NavMoveComponent : TaskPipelineBase<NavMoveComponent>, IAction, IJsonSaveable, ILocalCompute,
        ISyncObject
    {
        //SyncObject
        private PTTransform ptt;
        public Queue<string> syncBuffer;

        private void Awake()
        {
            ptt = new PTTransform();
            nmp = new NavMeshPath();
            syncBuffer = new Queue<string>();
        }

        private void Start()
        {
            base.Start();
        }

        public void LocalCompute()
        {
            UpdateAnimator();
        }


        #region 普通函数

        public void StartMoveToPosition(Vector3 destination, float speed = 0)
        {
            if (speed != 0)
            {
                this.GetComponent<NavMeshAgent>().speed = speed;
            }

            this.GetComponent<ActionSchedulerComponent>().StartAction(this);
            MoveToPosition(destination);
        }

        public void MoveToPosition(Vector3 destination)
        {
            if (!this.GetComponent<NavMeshAgent>().enabled) return;
            this.GetComponent<NavMeshAgent>().destination = destination;
            this.GetComponent<NavMeshAgent>().isStopped = false;
        }

        public void Cancel()
        {
            if (this.GetComponent<NavMeshAgent>().enabled)
                this.GetComponent<NavMeshAgent>().isStopped = true;
        }

        private void UpdateAnimator()
        {
            if (this.GetComponent<SyncObjectComponent>().ControllerSIID == "")
            {
                Vector3 velocity = this.GetComponent<NavMeshAgent>().velocity;
                Vector3 localVelocity = this.transform.InverseTransformDirection(velocity);
                this.GetComponent<Animator>().SetFloat("ForwardSpeed", localVelocity.z);
                ptt.Speed = localVelocity.z;
            }
        }

        #endregion

        #region 存档系统

        struct MoveSaveData
        {
            public JToken position;
            public JToken rotation;

            public MoveSaveData(JToken a, JToken b)
            {
                position = a;
                rotation = b;
            }

            public JToken ToToken()
            {
                JObject state = new JObject();
                IDictionary<string, JToken> stateDict = state;
                stateDict["position"] = position;
                stateDict["rotation"] = rotation;
                return state;
            }
        }

        public JToken CaptureAsJTokenInInterface()
        {
            // JObject state = new JObject();
            // IDictionary<string, JToken> stateDict = state;
            // stateDict["position"] = transform.position.ToToken();
            // stateDict["rotation"] = transform.eulerAngles.ToToken();

            return new MoveSaveData(transform.position.ToToken(), transform.eulerAngles.ToToken()).ToToken();
        }

        public void RestoreFormJToken(JToken state)
        {
            JObject s = state.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = s;
            this.GetComponent<NavMeshAgent>().Warp(stateDict["position"].ToVector3());
            this.transform.eulerAngles = stateDict["rotation"].ToVector3();
        }

        #endregion

        #region 射线系统

        NavMeshHit nmh;
        NavMeshPath nmp;
        private float SumPathLength;

        public bool IfMoveable(PlayerController p, RaycastHit h, out Vector3 des)
        {
            des = Vector3.zero;
            if (!NavMesh.SamplePosition(h.point, out nmh, 1, NavMesh.AllAreas))
            {
                p.SetCursor(CursorType.Deny);
                return false;
            }

            if (NavMesh.CalculatePath(p.transform.position, h.point, NavMesh.AllAreas, nmp) == false
                || nmp.status != NavMeshPathStatus.PathComplete)
            {
                p.SetCursor(CursorType.Deny);
                return false;
            }

            SumPathLength = 0;

            for (int i = 0; i < nmp.corners.Length - 1; i++)
            {
                SumPathLength += (nmp.corners[i] - nmp.corners[i + 1]).sqrMagnitude;
            }

            if (SumPathLength > 50)
            {
                p.SetCursor(CursorType.Deny);
                return false;
            }

            des = nmh.position;
            return true;
        }

        #endregion

        #region 同步系统

        public PTTransform SyncObject
        {
            get { return ptt; }
            set { ptt = value; }
        }

        public Queue<string> GetSyncBuffer()
        {
            return syncBuffer;
        }

        public string BuildSyncObject()
        {
            if (this.GetComponent<SyncObjectComponent>().ControllerSIID != "") return "";
            ptt.GameObjectName = this.gameObject.name;
            ptt.ComponentName = "NavMoveComponent";

            ptt.PositionX = this.transform.position.x;
            ptt.PositionY = this.transform.position.y;
            ptt.PositionZ = this.transform.position.z;
            ptt.AngleX = this.transform.eulerAngles.x;
            ptt.AngleY = this.transform.eulerAngles.y;
            ptt.AngleZ = this.transform.eulerAngles.z;

            return ptt.ToString();
        }

        public void ApplySyncData()
        {
        }


        public void ApplySyncState()
        {
            if (syncBuffer.Count > 0)
            {
                ptt = PTTransform.Parser.ParseJson(syncBuffer.Dequeue());
                this.transform.position =
                    new Vector3(ptt.PositionX, ptt.PositionY, ptt.PositionZ);
                this.transform.eulerAngles =
                    new Vector3(ptt.AngleX, ptt.AngleY, ptt.AngleZ);
                this.GetComponent<Animator>().SetFloat("ForwardSpeed", ptt.Speed);
            }
        }

        #endregion
    }
}