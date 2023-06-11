using System.Text;
using PRG.Network;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDSyncRequest
    {
        //同步阶段
        //3接着创建完成后，controller会发出同步请求协议
        public void Send(string siUid, string GameObjectName)
        {
            Debug.LogError("SendSyncRequest");
            //构建协议字符
            string cmd = $"SendSyncRequest|GameObjectName:{GameObjectName}";
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[3]服务器收到同步协议，根据同步列表是否有此物体，进行回复
        //若是，则开启该物体的被同步功能
        public bool Recv(string cmd, out string param)
        {
            Debug.LogError("RecvSyncRequest");
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);

            string key = cmd.Split(':')[1];
            if (SceneEntityManager.SyncEntities.ContainsKey(key))
            {
                param = key;
                return true;
            }
            else
            {
                param = "";
                return false;
            }
        }
    }
}