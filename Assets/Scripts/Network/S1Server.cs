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

        private void Awake()
        {
            Entities = new Dictionary<string, GameObject>();
            sb = new StringBuilder();
        }

        private void Start()
        {
            GeneratePlayer();

            if (NetworkCenter.isServerForS1) return;
            //GenerateEnemy1();
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
            if (NetworkCenter.isServerForS1)
            {
                if (CommunicationCenter.clientCommunications.ContainsKey("Client1") == false) return;
                while (CommunicationCenter.clientCommunications["Client1"][CommunicationChildType.Recv].socketInstance
                           .recvList.Count > 0)
                {
                    byte[] b = CommunicationCenter.clientCommunications["Client1"][CommunicationChildType.Recv].socketInstance
                        .recvList
                        .Dequeue();
                    sb.Clear();
                    for (int i = 0; i < b.Length; i++)
                    {
                        if (b[i] == 0) break;
                        sb.Append((char)b[i]);
                    }

                    int a = 0;
                    ptt = PTTransform.Parser.ParseJson(sb.ToString());
                    Entities["Player"].transform.position = new Vector3(ptt.PositionX, ptt.PositionY, ptt.PositionZ);
                    Entities["Player"].transform.eulerAngles = new Vector3(ptt.AngleX, ptt.AngleY, ptt.AngleZ);
                }
            }
            else
            {
                ptt.PositionX = Entities["Player"].transform.position.x;
                ptt.PositionY = Entities["Player"].transform.position.y;
                ptt.PositionZ = Entities["Player"].transform.position.z;
                ptt.AngleX = Entities["Player"].transform.eulerAngles.x;
                ptt.AngleY = Entities["Player"].transform.eulerAngles.y;
                ptt.AngleZ = Entities["Player"].transform.eulerAngles.z;
                NetworkCenter.ClientSendText(ptt.ToString());
            }

            //Debug.Log(ptt.ToByteArray());
            //Debug.Log(ptt.ToByteString());
            //Debug.Log(ptt.ToString());
            //Debug.Log(PTTransform.Parser.ParseJson(ptt.ToString()));
        }
    }
}