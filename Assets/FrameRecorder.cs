using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ActionClass
{
    public string name;
    public bool flag;
    public float value;
}

public class StatusRecord
{
    public GameObject go;
    public Vector3 position;
    public Quaternion rotation;
}

public class FrameRecorder : MonoBehaviour
{
    //帧记录集合
    public static Dictionary<int, Dictionary<string, ActionClass>> FrameRecords =
        new Dictionary<int, Dictionary<string, ActionClass>>();

    //状态记录集合
    public static Dictionary<int, Dictionary<GameObject, StatusRecord>> StatusRecords =
        new Dictionary<int, Dictionary<GameObject, StatusRecord>>();

    //update
    public static List<Action<int>> ManualUpdates = new List<Action<int>>();

    //late update
    public static List<Action<int>> ManualLateUpdates = new List<Action<int>>();

    private bool isRecordStart;

    void Awake()
    {
    }

    public static int CurFrame = 0;
    public int DesFrame = 0;
    private float timer;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(100, 100, 100, 100));
        if (GUILayout.Button("StartGame"))
        {
            isRecordStart = true;
        }

        GUILayout.EndArea();
    }

    void Update()
    {
        if (!isRecordStart) return;

        //回放
        if (CurFrame > 300)
        {
            if (DesFrame > 300)
            {
                Debug.Log("RollBack");
                DesFrame = 0;
                //回一次状态帧
                foreach (var i in StatusRecords[1])
                {
                    Debug.Log(i.Key.name + i.Value.position);
                    i.Key.transform.position = i.Value.position;
                    i.Key.transform.rotation = i.Value.rotation;
                }
            }
            else
            {
                DesFrame++;
                foreach (var i in ManualUpdates)
                {
                    i.Invoke(DesFrame);
                }

                foreach (var i in ManualLateUpdates)
                {
                    i.Invoke(DesFrame);
                }
            }

            return;
        }

        CurFrame++;
        DesFrame++;
        FrameRecords.Add(CurFrame, new Dictionary<string, ActionClass>());
        //E按键记录
        if (Input.GetKey(KeyCode.E))
        {
            FrameRecords[CurFrame].Add("KeyCodeE", new ActionClass() { flag = true });
        }
        else
        {
            FrameRecords[CurFrame].Add("KeyCodeE", new ActionClass() { flag = false });
        }

        //鼠标横轴记录
        FrameRecords[CurFrame].Add("AxisHorizontal", new ActionClass() { value = Input.GetAxis("Horizontal") });
        //鼠标纵轴记录
        FrameRecords[CurFrame].Add("AxisVertical", new ActionClass() { value = Input.GetAxis("Vertical") });
        //鼠标右键记录
        FrameRecords[CurFrame].Add("MouseButton1", new ActionClass() { flag = Input.GetMouseButton(1) });
        //鼠标左键记录
        FrameRecords[CurFrame].Add("MouseButton0", new ActionClass() { flag = Input.GetMouseButton(0) });

        StatusRecords.Add(CurFrame, new Dictionary<GameObject, StatusRecord>());
        //状态记录
        foreach (var i in FindObjectsOfType<StatusRecordComp>())
        {
            StatusRecords[CurFrame]
                .Add(i.gameObject, new StatusRecord()
                {
                    go = i.gameObject,
                    position = i.gameObject.transform.position,
                    rotation = i.gameObject.transform.rotation
                });
        }

        //本地正常调用
        foreach (var i in ManualUpdates)
        {
            i.Invoke(DesFrame);
        }

        foreach (var i in ManualLateUpdates)
        {
            i.Invoke(DesFrame);
        }
    }
}