using System;
using System.Collections.Generic;
using System.Text;
using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;

namespace RPG.Scene
{
    public class SceneEntityManager : MonoBehaviour
    {
        public static Dictionary<string, GameObject> Entities = new Dictionary<string, GameObject>();

        public static void GenerateEnemy1()
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Enemy"));
            go.transform.position = new Vector3(23.88f, 6.1f, 14.13f);
            go.transform.eulerAngles = new Vector3(0, -257.29f, 0);
            go.GetComponent<FighterActionComponent>().EquipItem(Resources.Load<WeaponConfig>("SwordWeapon"));
            go.GetComponent<BaseStats>().startingLevel = 2;
            go.GetComponent<BaseStats>().InitBaseStat();
            go.GetComponent<PathPatrolComponent>().pathGroup = GameObject.Find("PatrolPoint");
            Entities.Add("Enemy1", go);
        }

        public static void GeneratePlayer(string id)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Player"));
            go.transform.position = new Vector3(34f, 6.57f, 35.46f);
            go.transform.eulerAngles = new Vector3(0, 126.579f, 0);
            Entities.Add(id, go);
        }

        public static void DestroyPlayer()
        {
            Destroy(Entities["Player"]);
            Entities.Remove("Player");
        }
    }
}