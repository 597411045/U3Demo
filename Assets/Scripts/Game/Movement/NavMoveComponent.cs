using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class NavMoveComponent : MonoBehaviour
    {
        //移动组件
        //有2种移动方式，但均用预测值
        //1.通过寻路
        //2.通过物理

        //视角相关
        public GameObject camera;
        public CameraMode cameraMode;
        public GameObject SecondViewPoint;

        //移动相关
        private Rigidbody _rigidbody;
        NavMeshHit nmh;
        NavMeshPath nmp;
        private NavMeshAgent nma;
        private float SumPathLength;

        public float speed;

        //动画相关
        private Animator _animator;


        //内置参数
        private Vector3 gizmoDesPos;

        private void Awake()
        {
            Initial();
        }

        private void Initial()
        {
            cameraMode = CameraMode.First;
            nmp = new NavMeshPath();
            nma = this.GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        public void Update()
        {
            UpdateAnimator();
            UpdateMove();
        }

        public void UpdateMove()
        {
            //获得世界坐标预测方向
            Vector3 movingPredict = BuildMovingPredict();
            //获得人物对于摄像机正前方向
            Vector3 camForward = (this.transform.position - new Vector3(camera.transform.position.x,
                this.transform.position.y, camera.transform.position.z)).normalized;
            //转成rotation
            Quaternion rotationToCamForward = Quaternion.LookRotation(camForward);
            Vector3 desPredict = (rotationToCamForward * movingPredict) + this.transform.position;


            _rigidbody.velocity = rotationToCamForward * movingPredict;


            //若按右键，则进入瞄准模式，人物方向始终向前
            if (Input.GetMouseButton(1))
            {
                transform.rotation = rotationToCamForward;
            }
            else
            {
                Quaternion rotationToMovingPredict =
                    Quaternion.FromToRotation(Vector3.forward, (desPredict - transform.position).normalized);
                //调整人物旋转，普通模式，人物和摄像机互不影响旋转
                //Bug:用判断防止未操作也会进行转向
                if (movingPredict.sqrMagnitude > 0.1f)
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotationToMovingPredict, 0.9f);
            }

            //寻路可行性检测
            Vector3 destination = Vector3.zero;
            if (IfNavAccessable(desPredict, out destination))
            {
                nma.SetDestination(desPredict);
            }

#if UNITY_EDITOR
            gizmoDesPos = desPredict;
#endif
        }

        //更新行走动画
        private void UpdateAnimator()
        {
            Vector3 velocity = _rigidbody.velocity;
            Vector3 localVelocity = this.transform.InverseTransformDirection(velocity);
            _animator.SetFloat("ForwardSpeed",
                Mathf.Max(Mathf.Abs(localVelocity.x), Mathf.Abs(localVelocity.y), Mathf.Abs(localVelocity.z)));
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(gizmoDesPos, 0.5f);
            Gizmos.DrawLine(this.transform.position,
                gizmoDesPos);
        }

        private Vector3 BuildMovingPredict()
        {
            Vector3 movingPredict = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                if (movingPredict.z < 2)
                    movingPredict.z += 0.5f;
            }
            else
            {
                if (movingPredict.z > 0)
                {
                    movingPredict.z -= 0.5f;
                }
            }

            if (Input.GetKey(KeyCode.S))
            {
                if (movingPredict.z > -2)
                    movingPredict.z -= 0.5f;
            }
            else
            {
                if (movingPredict.z < 0)
                {
                    movingPredict.z += 0.5f;
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                if (movingPredict.x > -2)

                    movingPredict.x -= 0.5f;
            }
            else
            {
                if (movingPredict.x < 0)
                {
                    movingPredict.x += 0.5f;
                }
            }

            if (Input.GetKey(KeyCode.D))
            {
                if (movingPredict.x < 2)

                    movingPredict.x += 0.5f;
            }
            else
            {
                if (movingPredict.x > 0)
                {
                    movingPredict.x -= 0.5f;
                }
            }

            return movingPredict * speed;
        }

        private bool IfNavAccessable(Vector3 desPredict, out Vector3 destination)
        {
            destination = Vector3.zero;
            //是否可采样出
            if (!NavMesh.SamplePosition(desPredict, out nmh, 1, NavMesh.AllAreas)) return false;
            //是否可计算出目标点
            if (NavMesh.CalculatePath(this.transform.position, desPredict, NavMesh.AllAreas, nmp) == false
                || nmp.status != NavMeshPathStatus.PathComplete) return false;
            //计算寻路总长度
            SumPathLength = 0;
            if (nmp.corners.Length > 2) return false;
            for (int i = 0; i < nmp.corners.Length - 1; i++)
            {
                SumPathLength += (nmp.corners[i] - nmp.corners[i + 1]).sqrMagnitude;
            }

            if (SumPathLength > 50) return false;

            destination = nmh.position;
            return true;
        }
    }
}