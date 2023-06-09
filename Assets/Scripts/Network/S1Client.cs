using System;
using System.Collections.Generic;
using System.Text;
using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Network
{
    public class S1Client : MonoBehaviour
    {
        private Dictionary<string, GameObject> Entities;

        private StringBuilder sb;
        private PTTransform ptt;
        private NetworkCenter nc;

        private void Awake()
        {
            Entities = new Dictionary<string, GameObject>();
            sb = new StringBuilder();
            ptt = new PTTransform();

            Entities.Add("Player", GameObject.FindWithTag("Player"));
            nc = FindObjectOfType<NetworkCenter>();
            Entities["Player"].SetActive(false);

            CommandExecuter.SendLogin(nc);
        }

        private void Start()
        {
        }

        void GenerateEnemy1()
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

        void GeneratePlayer()
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Player"));
            go.transform.position = new Vector3(34f, 6.57f, 35.46f);
            go.transform.eulerAngles = new Vector3(0, 126.579f, 0);
            Entities.Add("Player", go);
        }

        private void Update()
        {
            //if (CommunicationCenter.clientCommunications.ContainsKey("Server") == false) return;
            // ptt.PositionX = Entities["Player"].transform.position.x;
            // ptt.PositionY = Entities["Player"].transform.position.y;
            // ptt.PositionZ = Entities["Player"].transform.position.z;
            // ptt.AngleX = Entities["Player"].transform.eulerAngles.x;
            // ptt.AngleY = Entities["Player"].transform.eulerAngles.y;
            // ptt.AngleZ = Entities["Player"].transform.eulerAngles.z;
            //
            // Vector3 velocity = Entities["Player"].GetComponent<NavMeshAgent>().velocity;
            // Vector3 localVelocity = Entities["Player"].transform.InverseTransformDirection(velocity);
            //
            // ptt.Speed = localVelocity.z;
            // nc.SendMessageBySocketUID("ClientMainSocket", Encoding.UTF8.GetBytes(ptt.ToString()));

            //Debug.Log(ptt.ToByteArray());
            //Debug.Log(ptt.ToByteString());
            //Debug.Log(ptt.ToString());
            //Debug.Log(PTTransform.Parser.ParseJson(ptt.ToString()));
        }

        private float timer;

        private void FixedUpdate()
        {
        }
    }
}