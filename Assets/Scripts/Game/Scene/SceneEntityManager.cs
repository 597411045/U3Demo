using System;
using System.Collections.Generic;
using System.Text;
using Game.Item;
using PRG.Sync;
using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Scene
{
    public class SceneEntityManager : MonoBehaviour
    {
        private void Start()
        {
            //GenerateEnemyPrefab();
        }

        public static GameObject GenerateEnemyPrefab()
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Enemy"));
            go.transform.position = new Vector3(27.79f, 1.6f, 6.26f);
            go.transform.eulerAngles = new Vector3(0, -257.29f, 0);
            go.GetComponent<BaseStats>().startingLevel = 2;
            go.GetComponent<BaseStats>().InitBaseStat();
            go.GetComponent<PathPatrolComponent>().pathGroup = GameObject.Find("PatrolPoint");
            
            
            return go;
        }

        public static void GeneratePurePrefab(string prefabName, string gobjectName, Vector3 position, string siid)
        {
            if (prefabName == "Player")
            {
                GameObject go = Instantiate(Resources.Load<GameObject>(prefabName));
                go.GetComponent<NavMeshAgent>().Warp(new Vector3(31.22f, 3.88f, 35.46f));
                go.transform.eulerAngles = new Vector3(0, 126.579f, 0);
                go.name = gobjectName;
                if (siid != "")
                {
                    go.GetComponent<SyncObjectComponent>().ControllerSIID = siid;
                }
            }
            else
            {
                GenerateEnemyPrefab();
            }
        }
    }
}