using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Player;
    public GameObject SecondViewPoint;
    public GameObject WeaponFirePoint;
    public Camera _camera;

    public float camDistance = 5;
    public Vector3 camOffset = Vector3.zero;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        //注册进帧记录系统
        FrameRecorder.ManualUpdates.Add(ManualUpdate);
        FrameRecorder.ManualLateUpdates.Add(ManualLateUpdate);
    }

    void Start()
    {
    }

    public void ManualUpdate(int frame)
    {
        bool KeyCodeE = FrameRecorder.FrameRecords[frame]["KeyCodeE"].flag;
        if (KeyCodeE)
        {
            Ray r = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(r, out hit))
            {
                if (WeaponFirePoint != null)
                {
                    Debug.DrawLine(hit.point, WeaponFirePoint.transform.position, Color.red);
                }
            }
        }
    }


    // Update is called once per frame
    void ManualLateUpdate(int frame)
    {
        //float axisX = Input.GetAxis("Horizontal");
        //float axisY = Input.GetAxis("Vertical");

        float axisX = FrameRecorder.FrameRecords[frame]["AxisHorizontal"].value;
        float axisY = FrameRecorder.FrameRecords[frame]["AxisVertical"].value;
        bool MouseButton1 = FrameRecorder.FrameRecords[frame]["MouseButton1"].flag;


        axisX = Mathf.Clamp(axisX, -1f, 1f);
        if (axisX != 0)
        {
            transform.Rotate(Vector3.up, axisX, Space.World);
        }

        axisY = Mathf.Clamp(axisY, -0.25f, 0.25f);
        if (axisY != 0)
        {
            if (transform.eulerAngles.x <= 60 && transform.eulerAngles.x >= 0)
            {
                DoRotate(axisY);
            }
            else if (transform.eulerAngles.x >= 60 && transform.eulerAngles.x <= 90 && axisY > 0)
            {
                DoRotate(axisY);
            }
            else if (transform.eulerAngles.x <= 360 && transform.eulerAngles.x >= 300)
            {
                DoRotate(axisY);
            }
            else if (transform.eulerAngles.x <= 300 && transform.eulerAngles.x >= 270 && axisY < 0)
            {
                DoRotate(axisY);
            }
            else if (transform.eulerAngles.x >= -1 && transform.eulerAngles.x <= 1)
            {
                DoRotate(axisY);
            }
        }

        //if (Input.GetMouseButton(1))
        if (MouseButton1)
        {
            transform.position =
                SecondViewPoint.transform.position + camOffset - transform.forward * camDistance / 2;
        }
        else
        {
            transform.position = Player.transform.position + camOffset - transform.forward * camDistance;
        }
    }

    private void DoRotate(float axisY)
    {
        transform.Rotate(Vector3.right, -axisY, Space.Self);
    }
}