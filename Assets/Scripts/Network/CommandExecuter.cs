using System.Linq;
using System.Text;
using RPG.Control;
using RPG.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class CommandExecuter
    {
        public static void CommandExec(string fromUid, string str)
        {
            if (str.Contains("|Player Login"))
            {
                RecvLogin(fromUid, str.Split('|')[0]);
            }

            if (str.Equals("Player Exit"))
            {
                RecvExit();
            }

            if (str.Equals("Player Login Valid"))
            {
                RecvLoginValid(fromUid);
            }

            if (str.Contains("ID:"))
            {
                RecvID(fromUid);
            }

            if (str.Contains("ChangeScene:"))
            {
                RecvCS(str.Split(':')[1]);
            }

            if (str.Contains("|position|"))
            {
                RecvTransformSync(str.Split('|')[0], str.Split('|')[2]);
                return;
            }

            Debug.LogError(fromUid + ":" + str);
        }

        private static void RecvID(string fromUid)
        {
            Debug.LogError("RecvID");
            NetworkCenter.ins.SendMessageBySocketUID(fromUid,
                Encoding.UTF8.GetBytes("ChangeScene:Dev/Scenes/Sandbox 1 Client/Sandbox 1 Client"));
        }

        private static void RecvCS(string sceneName)
        {
            Debug.LogError("RecvCS");
            SceneManager.LoadScene(sceneName);
        }

        public static void SendLogin()
        {
            Debug.LogError("SendLogin");
            if (NetworkCenter.ins != null)
            {
                NetworkCenter.ins.SendMessageBySocketUID("ClientMainSocket",
                    Encoding.UTF8.GetBytes("123|Player Login"));
            }
        }

        public static void RecvLogin(string fromUid, string id)
        {
            Debug.LogError("RecvLogin");
            SceneEntityManager.GeneratePlayer(id);
            SendLoginValid(fromUid);
        }

        public static void SendLoginValid(string fromUid)
        {
            Debug.LogError("SendLoginVaild");
            NetworkCenter.ins.SendMessageBySocketUID(fromUid, Encoding.UTF8.GetBytes("Player Login Valid"));
        }

        public static void RecvLoginValid(string fromUid)
        {
            Debug.LogError("RecvLoginVaild");
            SceneEntityManager.Entities["123"].SetActive(true);
            SceneEntityManager.Entities["123"].GetComponent<PlayerController>().readySync = true;
        }

        public static void RecvTransformSync(string id, string json)
        {
            PTTransform ptt = PTTransform.Parser.ParseJson(json);
            SceneEntityManager.Entities[id].transform.position =
                new Vector3(ptt.PositionX, ptt.PositionY, ptt.PositionZ);
            SceneEntityManager.Entities[id].transform.eulerAngles =
                new Vector3(ptt.AngleX, ptt.AngleY, ptt.AngleZ);
            SceneEntityManager.Entities[id].GetComponent<Animator>().SetFloat("ForwardSpeed", ptt.Speed);
        }

        public static void SendExit()
        {
            Debug.LogError("SendExit");
            NetworkCenter.ins.SendMessageBySocketUID("ClientMainSocket", Encoding.UTF8.GetBytes("Player Exit"));
        }

        public static void RecvExit()
        {
            Debug.LogError("RecvExit");
            SceneEntityManager.DestroyPlayer();
        }
    }
}