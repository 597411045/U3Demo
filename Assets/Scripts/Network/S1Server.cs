using System;
using System.Collections.Generic;
using System.Text;
using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;

namespace Network
{
    public class S1Server : MonoBehaviour
    {
        private Dictionary<string, GameObject> Entities;

        private StringBuilder sb;
        private PTTransform ptt;
        private NetworkCenter nc;

        private void Awake()
        {
            Entities = new Dictionary<string, GameObject>();
            sb = new StringBuilder();
            nc = FindObjectOfType<NetworkCenter>();
        }

        private void Start()
        {
            //GeneratePlayer();
        }

        public void GenerateEnemy1()
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Enemy"));
            go.transform.position = new Vector3(23.88f, 6.1f, 14.13f);
            go.transform.eulerAngles = new Vector3(0, -257.29f, 0);
            go.GetComponent<FighterActionComponent>().EquipItem(Resources.Load<WeaponConfig>("SwordWeapon"));
            go.GetComponent<BaseStats>().startingLevel = 2;
            go.GetComponent<BaseStats>().AwakeGen();
            go.GetComponent<PathPatrolComponent>().pathGroup = GameObject.Find("PatrolPoint");
            Entities.Add("Enemy1", go);
        }

        public void GeneratePlayer()
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Player"));
            go.transform.position = new Vector3(34f, 6.57f, 35.46f);
            go.transform.eulerAngles = new Vector3(0, 126.579f, 0);
            Entities.Add("Player", go);
        }

        private byte[] b;

        private void Update()
        {
            b = nc.GetMessageBySocketUID("tmpSocket1");
            if (b != null)
            {
                sb.Clear();
                sb.Append(Encoding.UTF8.GetString(b));
                ptt = PTTransform.Parser.ParseJson(sb.ToString());
                Entities["Player"].transform.position = new Vector3(ptt.PositionX, ptt.PositionY, ptt.PositionZ);
                Entities["Player"].transform.eulerAngles = new Vector3(ptt.AngleX, ptt.AngleY, ptt.AngleZ);
                Entities["Player"].GetComponent<Animator>().SetFloat("ForwardSpeed", ptt.Speed);
            }
        }
    }
}