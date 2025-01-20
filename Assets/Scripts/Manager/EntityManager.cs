using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class EntityManager : MonoBehaviour
{
    [NonSerialized] public List<AIController> aiList = new List<AIController>();
    [NonSerialized] public List<AIController> enemyList = new List<AIController>();
    [NonSerialized] public List<AIController> playerList = new List<AIController>();

    public GameObject BulletPrefab;
    public GameObject SplinePrefab;
    public GameObject CylinderDamagePrefab;
    public GameObject BoxDamagePrefab;
    public GameObject 近战敌人Prefab;


    public GameObject BulletGroup;

    private bool DoOnce = true;

    private void Awake()
    {
        GameMode.Instance.AddActionToAwakeOrderDic(0, M_Awake);
        GameMode.Instance.AddActionToUpdateOrderDic(999, M_Update);
    }

    private void M_Awake()
    {
        var entities =
            GameObject.FindObjectsByType<AIController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        AIController c1 = null;
        AIController c2 = null;
        AIController c3 = null;
        foreach (var iter in entities)
        {
            if (iter.gameObject.tag == "Player")
            {
                if (iter.gameObject.name == "C1")
                {
                    c1 = iter;
                }

                if (iter.gameObject.name == "C2")
                {
                    c2 = iter;
                }

                if (iter.gameObject.name == "C3")
                {
                    c3 = iter;
                }

                aiList.Add(iter);
            }

            if (iter.gameObject.tag == "Enemy")
            {
                enemyList.Add(iter);
                aiList.Add(iter);
            }
        }

        if (c1 != null)
        {
            playerList.Add(c1);
        }

        if (c2 != null)
        {
            playerList.Add(c2);
        }

        if (c3 != null)
        {
            playerList.Add(c3);
        }
    }

    private void M_Update()
    {
        if (DoOnce && enemyList.Count != 0)
        {
            foreach (var iter in enemyList)
            {
                if (iter.characterData.currentHealth > 0)
                {
                    return;
                }
            }

            GameMode.Instance.OnEnemyClear();

            DoOnce = false;
        }
    }

    public Bullet SpawnBullet(Vector3 pos, Quaternion qua)
    {
        //GameObject bullet = Resources.Load("Prefab/Bullet") as GameObject;
        GameObject bulletInstance = Instantiate(BulletPrefab,
            pos,
            qua);
        bulletInstance.transform.parent = BulletGroup.transform;
        return bulletInstance.GetComponent<Bullet>();
    }

    public SplineContainer SpawnSpline()
    {
        GameObject splineInstance = Instantiate(SplinePrefab,
            Vector3.zero,
            quaternion.identity);
        var result = splineInstance.GetComponent<SplineContainer>();
        return result;
    }

    public FunctionItem SpawnCylinderDamage(Vector3 pos)
    {
        return Instantiate(CylinderDamagePrefab, pos, Quaternion.identity).GetComponent<FunctionItem>();
    }

    public FunctionItem SpawnBoxDamage(Vector3 pos, Quaternion qua)
    {
        return Instantiate(BoxDamagePrefab, pos, qua).GetComponent<FunctionItem>();
    }

    public AIController SpawnMeleeEnemy(Vector3 pos, Quaternion qua)
    {
        return Instantiate(近战敌人Prefab, pos, qua).GetComponent<AIController>();
    }
}