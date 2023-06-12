using System;
using System.Collections.Generic;
using System.Text;
using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Scene
{
    public class SceneEntityManager : MonoBehaviour
    {
        public static void GenerateEnemyPrefab()
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Enemy"));
            go.transform.position = new Vector3(27.79f, 1.6f, 6.26f);
            go.transform.eulerAngles = new Vector3(0, -257.29f, 0);
            go.GetComponent<FighterActionComponent>().EquipItem(Resources.Load<WeaponConfig>("SwordWeapon"));
            go.GetComponent<BaseStats>().startingLevel = 2;
            go.GetComponent<BaseStats>().InitBaseStat();
            go.GetComponent<PathPatrolComponent>().pathGroup = GameObject.Find("PatrolPoint");
        }

        public static void GeneratePurePrefab(string prefabName, string gobjectName, Vector3 position)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(prefabName));
            go.GetComponent<NavMeshAgent>().Warp(new Vector3(31.22f, 3.88f, 35.46f));
            go.transform.eulerAngles = new Vector3(0, 126.579f, 0);
            go.name = gobjectName;
            Debug.LogError(go.transform.position);
        }
    }
}