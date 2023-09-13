using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScrpit : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject ArrowStart;
    public GameObject ArrowEnd;
    public GameObject Bullet;
    private float timer;

    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Fire();
        // }
    }

    public void Fire()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Bullet"), ArrowEnd.transform.position,
            Quaternion.identity);
        go.transform.forward = (ArrowEnd.transform.position - ArrowStart.transform.position).normalized;
        go.GetComponent<Rigidbody>().AddForce(go.transform.forward * 10, ForceMode.Impulse);
    }
}