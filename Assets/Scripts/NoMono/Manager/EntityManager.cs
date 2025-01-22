using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class EntityManager : SingleTon<EntityManager>
{
    [NonSerialized] private List<AIController> aiList = new List<AIController>();
    [NonSerialized] public List<AIController> aliveEnemyList = new List<AIController>();
    [NonSerialized] public List<AIController> deadEnemyList = new List<AIController>();
    [NonSerialized] public List<AIController> playerList = new List<AIController>();
    private GameObject BulletGroup;

    private bool DoOnce = true;


    public void Awake()
    {
        GameMode.Instance.AddActionToAwakeOrderDic(0, M_Awake);
        GameMode.Instance.AddActionToUpdateOrderDic(999, M_Update);
    }

    public List<AIController> GetAiList()
    {
        return aiList;
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
                aliveEnemyList.Add(iter);
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
        if (aliveEnemyList.Count > 0)
        {
            for (int i = aliveEnemyList.Count - 1; i >= 0; i--)
            {
                if (aliveEnemyList[i].characterData.currentHealth <= 0)
                {
                    deadEnemyList.Add(aliveEnemyList[i]);
                    aliveEnemyList.RemoveAt(i);
                }
            }
        }


        if (DoOnce && aliveEnemyList.Count == 0 && deadEnemyList.Count != 0)
        {
            GameMode.Instance.OnEnemyClear();
            DoOnce = false;
        }
    }

    public Bullet SpawnBullet(Vector3 pos, Quaternion qua)
    {
        //GameObject bullet = Resources.Load("Prefab/Bullet") as GameObject;
        GameObject bulletInstance = GameObject.Instantiate(GameMode.Instance.BulletPrefab,
            pos,
            qua);
        bulletInstance.transform.parent = BulletGroup.transform;
        return bulletInstance.GetComponent<Bullet>();
    }
 
    public SplineContainer SpawnSpline()
    {
        GameObject splineInstance = GameObject.Instantiate(GameMode.Instance.SplinePrefab,
            Vector3.zero,
            Quaternion.identity);
        var result = splineInstance.GetComponent<SplineContainer>();
        return result;
    }

    public FunctionItem SpawnCylinderDamage(Vector3 pos)
    {
        return GameObject.Instantiate(GameMode.Instance.CylinderDamagePrefab, pos, Quaternion.identity).GetComponent<FunctionItem>();
    }

    public FunctionItem SpawnBoxDamage(Vector3 pos, Quaternion qua)
    {
        return GameObject.Instantiate(GameMode.Instance.BoxDamagePrefab, pos, qua).GetComponent<FunctionItem>();
    }

    public AIController SpawnMeleeEnemy(Vector3 pos, Quaternion qua)
    {
        var result = GameObject.Instantiate(GameMode.Instance.近战敌人Prefab, pos, qua).GetComponent<AIController>();
        aiList.Add(result);
        aliveEnemyList.Add(result);
        return result;
    }

    public AIController GetCallerNearestEnemy(AIController caller)
    {
        float minDistance = float.MaxValue;
        AIController result = null;
        foreach (var iter in aiList)
        {
            if (iter != caller && iter.characterData.currentHealth > 0 && iter.gameObject.tag != caller.gameObject.tag)
            {
                var tmp = Vector3.Distance(caller.gameObject.transform.position, iter.gameObject.transform.position);
                if (tmp < minDistance)
                {
                    minDistance = tmp;
                    result = iter;
                }
            }
        }

        return result;
    }

    public AIController GetCallerNearestAlly(AIController caller)
    {
        float minDistance = float.MaxValue;
        AIController result = null;
        foreach (var iter in aiList)
        {
            if (iter != caller && iter.characterData.currentHealth > 0 && iter.gameObject.tag == caller.gameObject.tag)
            {
                var tmp = Vector3.Distance(caller.gameObject.transform.position, iter.gameObject.transform.position);
                if (tmp < minDistance)
                {
                    minDistance = tmp;
                    result = iter;
                }
            }
        }

        return result;
    }
}