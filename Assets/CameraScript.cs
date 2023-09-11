using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Player;
    public GameObject SecondViewPoint;


    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float axisX = Input.GetAxis("Horizontal");
        axisX = Mathf.Clamp(axisX, -1f, 1f);
        Debug.Log(axisX);
        if (axisX != 0)
        {
            transform.Rotate(Vector3.up, axisX, Space.World);
        }

        float axisY = Input.GetAxis("Vertical");
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

        if (Input.GetMouseButton(1))
        {
            transform.position = SecondViewPoint.transform.position - transform.forward * 3;
        }
        else
        {
            transform.position = Player.transform.position - transform.forward * 5;
        }
    }

    private void DoRotate(float axisY)
    {
        transform.Rotate(Vector3.right, -axisY, Space.Self);
    }
}