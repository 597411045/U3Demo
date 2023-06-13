using System.Text;
using Cmd;
using PRG.Cmd;
using PRG.Network;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDSyncRequest : CMDBase<CMDSyncRequest>,ICMDAction
    {

        public CMDSyncRequest() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<GameObjectName>";
        }


        //同步阶段
        //3接着创建完成后，controller会发出同步请求协议
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siUid"></param>
        /// <param name="paras">1:GameObjectName</param>
        public void Send(string siUid, params string[] paras)
        {
            string cmd = ReplaceParam(paras);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[3]服务器收到同步协议，根据同步列表是否有此物体，进行回复
        //若是，则开启该物体的被同步功能
        public void Recv(string siid, string cmd)
        {
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);

            string GameObjectName = GetParam(cmd, 0);

            GameObject go = GameObject.Find(GameObjectName);
            if (go != null)
            {
                CMDSyncRequestAllow.Ins.Send(siid, GameObjectName);
            }
        }
    }
}