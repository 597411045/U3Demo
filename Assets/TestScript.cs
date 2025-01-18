using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float speed;

    public Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            _rigidbody.velocity = new Vector3(-1 * speed, 0, 0);
        }
    }
}