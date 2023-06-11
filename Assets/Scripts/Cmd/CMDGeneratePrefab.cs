using System.Text;
using PRG.Network;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDGeneratePrefab
    {
        //2.2服务器回复创建玩家
        public void Send(string fromUid, string prefabName, string GameObjectName)
        {
            Debug.LogError("SendGeneratePrefab");
            //构建协议字符
            string cmd = $"SendGeneratePrefab|Prefab:{prefabName}|GameObjectName:{GameObjectName}";
            NetworkManagement.Ins.SendMessageBySocketUID(fromUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[2.2].客户端收到创建玩家消息，进行玩家创建
        public void Recv(string cmd)
        {
            Debug.LogError("RecvGeneratePrefab");
            SceneEntityManager.GeneratePlayerPrefab(cmd.Split('|')[2].Split(':')[1]);
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);
        }
    }
}