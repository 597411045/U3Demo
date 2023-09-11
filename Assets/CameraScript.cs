using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Player;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float axisX = Input.GetAxis("Horizontal");
        if (axisX != 0)
        {
            transform.Rotate(Vector3.up, axisX, Space.World);
        }

        float axisY = Input.GetAxis("Vertical");
        if (Mathf.Abs(axisY) > 10) return;
        if (axisY != 0)
        {
            if (transform.eulerAngles.x <= 60 && transform.eulerAngles.x >= 0)
            {
                DoRotate();
            }
            else if (transform.eulerAngles.x >= 60 && transform.eulerAngles.x <= 90 && axisY > 0)
            {
                DoRotate();
            }
            else if (transform.eulerAngles.x <= 360 && transform.eulerAngles.x >= 300)
            {
                DoRotate();
            }
            else if (transform.eulerAngles.x <= 300 && transform.eulerAngles.x >= 270 && axisY < 0)
            {
                DoRotate();
            }
            else if (transform.eulerAngles.x >= -1 && transform.eulerAngles.x <= 1)
            {
                DoRotate();
            }
        }

        if (Input.GetMouseButton(1))
        {
        }
        else
        {
            transform.position = Player.transform.position - transform.forward * 5;
        }
    }

    private void DoRotate()
    {
        transform.Rotate(Vector3.right, -Input.GetAxis("Vertical"), Space.Self);
    }
}