using System.Text;
using PRG.Cmd;
using PRG.Network;
using PRG.Sync;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDSyncRequestAllow : CMDBase<CMDSyncRequestAllow>
    {
        public CMDSyncRequestAllow() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<GameObjectName>";
        }


        //3.1服务器回复允许同步
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siUid"></param>
        /// <param name="para">1:GameObjectName</param>
        public override void Send(string siUid, params string[] paras)
        {
            string cmd = ReplaceParam(paras);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        // //[3.1]客户端接收到允许同步，开启此物体的同步功能
        public override void Recv(string siid, string cmd)
        {
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);

            string GameObjectName = GetParam(cmd, 0);
            GameObject go = GameObject.Find(GameObjectName);
            if (go != null)
            {
                go.GetComponent<SyncObjectComponent>().isSyncControlled = false;
                Debug.LogError(" go.GetComponent<SyncObjectComponent>().enabled = true;");
            }
        }
    }
}