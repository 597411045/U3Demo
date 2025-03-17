using System;
using System.Collections;
using System.Collections.Generic;
using BattleLocal_UnityOnly.UnityConfig;
using BattleLocal.ConfigRuntime;
using BattleLocal.Projectile;
using Logic.Math;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineProxy;

namespace Logic.Misc_UnityOnly
{
    public class EasyPositonRotation : MonoBehaviour
    {
        public Transform positionParent;
        public Transform rotationParent;
        public bool 仅在特效开始时同步位置到Pos挂点位置;
        public bool 仅在特效开始时同步旋转到Rot挂点旋转;

        [FormerlySerializedAs("Rot挂点旋转同步后X轴归0")] [FormerlySerializedAs("lockRotationX")]
        public bool 继上一选项后X轴归0;

        [FormerlySerializedAs("ParentOnStart")]
        public bool 直接挂点到Pos挂点;

        public GameObject 仅在特效开始时想要转向这个物体;
        public Vector3 最后还想在Rot挂点的旋转上偏移多少;
        public Vector3 最后还想在Rot挂点的方向上偏移多少;

        [FormerlySerializedAs("go")] public GameObject 寻找骨骼的物体;
        public string 预设寻找的骨骼 = "";

        [FormerlySerializedAs("RotateByrotationParent")]
        public bool 持续保持和Rot挂点方向一致;

        public bool 持续保持和Pos挂点位置一致;

        public bool 让这个物体持续向前;
        public GameObject 继上一选项后想要追踪的目标;

        [FormerlySerializedAs("继上一选项后追上时停止特效")]
        public bool 继上一选项后追上时隐藏特效;

        public List<GameObject> 继上一选项后追上时想要开启的特效 = new List<GameObject>();

        public float 向前的速度 = 1;
        public bool 每次开始时从原点开始;


        private bool localActive;
        public bool 将在实机中生效;
        private bool 正在实机中;
        public bool NeedLog;


        private Vector3 originalPosition;
        public Vector3 originalScale;


        private void Start()
        {
        }

        private void Update()
        {
            if (localActive == false)
            {
                return;
            }

            if (持续保持和Rot挂点方向一致 && rotationParent != null)
            {
                this.transform.rotation = rotationParent.rotation;
            }

            if (持续保持和Pos挂点位置一致 && positionParent != null)
            {
                this.transform.position = positionParent.position;
            }

            if (让这个物体持续向前 && 正在实机中 == false)
            {
                if (继上一选项后想要追踪的目标 != null)
                {
                    this.transform.forward = (继上一选项后想要追踪的目标.transform.position - this.transform.position).normalized;
                    if (继上一选项后追上时隐藏特效 && Vector3.Distance(继上一选项后想要追踪的目标.transform.position, this.transform.position) <
                        0.2f)
                    {
                        this.transform.position = Vector3.zero;
                        localActive = false;

                        if (继上一选项后追上时想要开启的特效 != null && 继上一选项后追上时想要开启的特效.Count > 0)
                        {
                            foreach (var per in 继上一选项后追上时想要开启的特效)
                            {
                                var epr = per.GetComponent<EasyPositonRotation>();
                                if (epr != null)
                                {
                                    if (epr.预设寻找的骨骼 != "")
                                    {
                                        epr.positionParent = GetChildenTransform(继上一选项后想要追踪的目标.transform, epr.预设寻找的骨骼);
                                        epr.rotationParent = epr.positionParent;
                                    }
                                }

                                per.SetActive(false);
                                per.SetActive(true);
                            }
                        }

                        return;
                    }
                }

                this.transform.position += this.transform.forward * Time.deltaTime * 向前的速度;
            }
        }

        private bool doOnce = false;

        private void OnEnable()
        {
            if (doOnce == false &&
                this.transform.parent != null &&
                this.transform.parent.name == "VFXPoolManager")
            {
                正在实机中 = true;
            }


            if (NeedLog)
            {
                Debug.Log("OnEnable:" + Time.frameCount);
            }

            if (正在实机中 == true && 将在实机中生效 == true)
            {
                if (this.gameObject.activeInHierarchy)
                {
                    StartCoroutine(Manual_OnEnable());
                }
            }
            else
            {
                var tmp = Manual_OnEnable();
                tmp?.MoveNext();
            }
        }

        IEnumerator Manual_OnEnable()
        {
            if (正在实机中 == true && 将在实机中生效 == true)
            {
                yield return null;
            }

            if (正在实机中 == false || (正在实机中 == true && 将在实机中生效 == true))
            {
                if (NeedLog)
                {
                    Debug.Log("Manual OnEnable:" + Time.frameCount);
                }

                localActive = true;
                if (每次开始时从原点开始 && originalPosition == Vector3.zero)
                {
                    originalPosition = this.transform.position;
                }

                if (每次开始时从原点开始 && originalPosition != Vector3.zero)
                {
                    this.transform.position = originalPosition;
                }

                if (仅在特效开始时同步位置到Pos挂点位置 && positionParent != null)
                {
                    this.transform.position = positionParent.position;
                }

                if (仅在特效开始时同步旋转到Rot挂点旋转 && rotationParent != null)
                {
                    this.transform.rotation = rotationParent.rotation;
                    if (继上一选项后X轴归0)
                    {
                        this.transform.eulerAngles += new Vector3(-this.transform.eulerAngles.x, 0, 0);
                    }
                }

                if (直接挂点到Pos挂点 && positionParent != null)
                {
                    this.transform.SetParent(positionParent, false);
                }

                if (仅在特效开始时想要转向这个物体 != null)
                {
                    this.transform.forward = (仅在特效开始时想要转向这个物体.transform.position - this.transform.position).normalized;
                }

                if (最后还想在Rot挂点的旋转上偏移多少 != Vector3.zero && rotationParent != null)
                {
                    this.transform.localRotation = Quaternion.Euler(最后还想在Rot挂点的旋转上偏移多少) * rotationParent.rotation;
                }

                if (最后还想在Rot挂点的方向上偏移多少 != Vector3.zero && rotationParent != null)
                {
                    this.transform.localPosition += rotationParent.rotation * 最后还想在Rot挂点的方向上偏移多少;
                }

                if (让这个物体持续向前 == true && 正在实机中 == true)
                {
                    this.transform.localRotation = Quaternion.identity;
                }
            }

            yield return null;
        }

        public void CallByRuntime(GameObject cha)
        {
            if (将在实机中生效 == true && 正在实机中 == true)
            {
                最后还想在Rot挂点的旋转上偏移多少 = Vector3.zero;
                最后还想在Rot挂点的方向上偏移多少 = Vector3.zero;

                var curLossyScale = this.gameObject.transform.lossyScale;
                var xScale = originalScale.x / curLossyScale.x;
                var yScale = originalScale.y / curLossyScale.y;
                var zScale = originalScale.z / curLossyScale.z;
                var curLocalScale = this.transform.localScale;
                this.gameObject.transform.localScale = new Vector3(xScale * curLocalScale.x,
                    yScale * curLocalScale.y, zScale * curLocalScale.z);
                if (NeedLog)
                {
                    Debug.Log(this.gameObject.transform.lossyScale);
                }

                positionParent = cha.transform;
                rotationParent = cha.transform;
                寻找骨骼的物体 = cha;
                GetBone();
                OnEnable();
            }
        }

        public void ApplyTransform()
        {
            // this.transform.position = positionParent.position;
            //
            // this.transform.rotation = rotationParent.rotation;
            // if (继上一选项后X轴归0)
            // {
            //     this.transform.eulerAngles += new Vector3(-this.transform.eulerAngles.x, 0, 0);
            // }
        }

        [Button]
        public void GetBone()
        {
            if (寻找骨骼的物体 != null && 预设寻找的骨骼 != "")
            {
                positionParent = GetChildenTransform(寻找骨骼的物体.transform, 预设寻找的骨骼);
#if UNITY_EDITOR
                if (EditorApplication.isPlaying == false)
                {
                    originalScale = this.gameObject.transform.lossyScale;
                    AssetDatabase.SaveAssetIfDirty(this);
                    AssetDatabase.SaveAssets();
                }
#endif
            }
        }

        private Transform GetChildenTransform(Transform trans, string name)
        {
            int childrenCount = trans.childCount;
            if (childrenCount > 0)
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    if (trans.GetChild(i).name == name)
                    {
                        return trans.GetChild(i);
                    }
                    else
                    {
                        var result = GetChildenTransform(trans.GetChild(i), name);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }
    }
}