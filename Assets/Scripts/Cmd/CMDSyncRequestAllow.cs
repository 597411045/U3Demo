using System.Text;
using PRG.Network;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDSyncRequestAllow
    {
        //3.1服务器回复允许同步
        public void Send(string siUid, string GameObjectName)
        {
            Debug.LogError("SendSyncRequestAllow");
            //构建协议字符
            string cmd = $"SendSyncRequestAllow|GameObjectName:{GameObjectName}";
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        // //[3.1]客户端接收到允许同步，开启此物体的同步功能
        public void Recv(string cmd)
        {
            Debug.LogError("RecvSyncRequestAllow");
            string key = cmd.Split(':')[1];
            if (SceneEntityManager.SyncEntities.ContainsKey(key))
            {
                SceneEntityManager.SyncEntities[key].GetComponent<SyncObjectComponent>().enabled = true;
            }

            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);
        }
    }
}