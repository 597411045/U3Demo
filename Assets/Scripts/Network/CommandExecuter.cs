using System.Linq;
using System.Text;
using RPG.Control;
using RPG.Core;
using RPG.Scene;
using RPG.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PRG.Network
{
    public class CommandExecuter
    {

        public static CommandExecuter Ins;

        public CommandExecuter()
        {
            if (Ins == null)
            {
                Debug.LogError(this.ToString() + " Construction");
                Ins = this;
            }
            else
            {
                Debug.LogError("For Now, Only One " + this.ToString() + " Allowed");
            }
        }
        
        public void CommandExec(string fromUid, string cmd)
        {
            //初始化阶段
            //0.客户端收到服务器链接成功的消息
            //1，客户端率先发起登录信息
            if (cmd.Equals("Hello Client"))
            {
                SendLogin(fromUid, "TestClient", "TestClient");
                return;
            }

            //[1].服务器收到登录信息
            if (cmd.Contains("SendLogin|"))
            {
                RecvLogin(cmd);
                //2.1 发送切换场景
                SendChangeScene(fromUid, "Scenes/Sandbox 1 Client/Sandbox 1 Client");

                //2.2服务器回复创建玩家
                //TODO:广播
                SendGeneratePrefab(fromUid, "Player", "TestTransform");
                return;
            }

            //[2.1]客户端收到切换场景消息，进行场景切换
            if (cmd.Contains("SendChangeScene|"))
            {
                RecvChangeScene(cmd);
                return;
            }

            //[2.2].客户端收到创建玩家消息，进行玩家创建
            if (cmd.Contains("SendGeneratePrefab|"))
            {
                RecvGeneratePrefab(cmd);
                return;
            }


            //3接着创建完成后，controller会发出同步请求协议

            //同步阶段
            //[3]服务器收到同步协议，根据同步列表是否有此物体，进行回复
            if (cmd.Contains("SendSyncRequest|"))
            {
                string param;
                if (RecvSyncRequest(cmd, out param))
                {
                    //3.1服务器回复允许同步
                    SendSyncRequestAllow(fromUid, param);
                }

                return;
            }
            //[3.1]客户端接收到允许同步，开启此物体的同步功能

            if (cmd.Contains("SendSyncRequestAllow|"))
            {
                RecvSyncRequestAllow(cmd);
                return;
            }

            CmdUI.Ins.LogOnScreen("UNKNOWN Recv:" + cmd);
        }

        //初始化阶段
        //0.客户端收到服务器链接成功的消息
        //1，客户端率先发起登录信息
        public void SendLogin(string siUid, string username, string password)
        {
            Debug.LogError("SendLogin");
            //构建协议字符
            string cmd = $"SendLogin|ID:{username}|PWD:{password}";

            NetworkCenter.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));

            CmdUI.Ins.LogOnScreen("Send:" + cmd);
        }

        //[1].服务器收到登录信息
        private void RecvLogin(string cmd)
        {
            Debug.LogError("RecvLogin");
            //获取客户端信息（状态，Transform等）
            //TODO:获取信息
            //服务器本地创建
            SceneEntityManager.GeneratePlayer(cmd.Split('|')[1].Split(':')[1]);
            CmdUI.Ins.LogOnScreen("Recv:" + cmd);
        }

        //2.1 发送切换场景
        //TODO:场景服务器
        public void SendChangeScene(string siUid, string sceneName)
        {
            Debug.LogError("SendChangeScene");
            //构建协议字符
            string cmd = $"SendChangeScene|sceneName:{sceneName}";

            NetworkCenter.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdUI.Ins.LogOnScreen("Send:" + cmd);
        }

        //[2.1]客户端收到切换场景消息，进行场景切换
        private void RecvChangeScene(string cmd)
        {
            Debug.LogError("RecvSendChangeScene");
            //TODO:需要携程进行后续操作,携程内容：切换场景-屏幕渐变+生成角色-发送同步请求
            SceneManager.LoadScene(cmd.Split(':')[1]);
            CmdUI.Ins.LogOnScreen("Recv:" + cmd);
        }

        //2.2服务器回复创建玩家
        public void SendGeneratePrefab(string fromUid, string prefabName, string transformProto)
        {
            Debug.LogError("SendGeneratePrefab");
            //构建协议字符
            string cmd = $"SendGeneratePrefab|Prefab:{prefabName}|transformProto:{transformProto}";
            NetworkCenter.Ins.SendMessageBySocketUID(fromUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdUI.Ins.LogOnScreen("Send:" + cmd);
        }

        //[2.2].客户端收到创建玩家消息，进行玩家创建
        private void RecvGeneratePrefab(string cmd)
        {
            Debug.LogError("RecvGeneratePrefab");
            SceneEntityManager.GeneratePlayer("TestClient");
            CmdUI.Ins.LogOnScreen("Recv:" + cmd);
        }

        //同步阶段
        //3接着创建完成后，controller会发出同步请求协议
        public void SendSyncRequest(string siUid, string EntityName)
        {
            Debug.LogError("SendSyncRequest");
            //构建协议字符
            string cmd = $"SendSyncRequest|EntityName:{EntityName}";
            NetworkCenter.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdUI.Ins.LogOnScreen("Send:" + cmd);
        }

        //[3]服务器收到同步协议，根据同步列表是否有此物体，进行回复
        //若是，则开启该物体的被同步功能
        private bool RecvSyncRequest(string cmd, out string param)
        {
            Debug.LogError("RecvSyncRequest");
            CmdUI.Ins.LogOnScreen("Recv:" + cmd);

            string key = cmd.Split(':')[1];
            if (SceneEntityManager.Entities.ContainsKey(key))
            {
                SceneEntityManager.Entities[key].GetComponent<SyncComponent>().beSynced = true;

                param = key;
                return true;
            }
            else
            {
                param = "";
                return false;
            }
        }

        //3.1服务器回复允许同步
        public void SendSyncRequestAllow(string siUid, string EntityName)
        {
            Debug.LogError("SendSyncRequestAllow");
            //构建协议字符
            string cmd = $"SendSyncRequestAllow|EntityName:{EntityName}";
            NetworkCenter.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdUI.Ins.LogOnScreen("Send:" + cmd);
        }

        // //[3.1]客户端接收到允许同步，开启此物体的同步功能
        private void RecvSyncRequestAllow(string cmd)
        {
            Debug.LogError("RecvyncRequestAllow");
            string key = cmd.Split(':')[1];
            if (SceneEntityManager.Entities.ContainsKey(key))
            {
                SceneEntityManager.Entities[key].GetComponent<SyncComponent>().ifDoSync = true;
            }

            CmdUI.Ins.LogOnScreen("Recv:" + cmd);
        }


        //退出阶段

        private static void RecvID(string fromUid)
        {
            Debug.LogError("RecvID");
            NetworkCenter.Ins.SendMessageBySocketUID(fromUid,
                Encoding.UTF8.GetBytes("ChangeScene:Scenes/Sandbox 1 Client/Sandbox 1 Client"));
            NetworkCenter.Ins.SendMessageBySocketUID(fromUid,
                Encoding.UTF8.GetBytes("Generate:Player"));
        }

        private static void RecvCS(string sceneName)
        {
            Debug.LogError("RecvCS");
            SceneManager.LoadScene(sceneName);
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
    }
}