using System.Text;
using PRG.Network;
using PRG.Sync;
using ProtoMsg;
using RPG.Cmd;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RGP.Cmd
{
    public class CMDSyncObject : CMDBase<CMDSyncObject>
    {
        public CMDSyncObject() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<GameObjectName><PTT>";
        }

        //3.2客户端发送出同步信息
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siUid"></param>
        /// <param name="paras">1:GameObjectName,2:PTT</param>
        public override void Send(string siUid, params string[] paras)
        {
            string cmd = ReplaceParam(paras);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[3.2]服务端接收到同步信息
        public override void Recv(string siid, string cmd)
        {
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);


            string GameObjectName = GetParam(cmd, 0);
            string json = GetParam(cmd, 1);

            PTTransform tmp = PTTransform.Parser.ParseJson(json);

            GameObject go = GameObject.Find(GameObjectName);
            if (go != null)
            {
                foreach (var c in go.GetComponents<ISyncObject>())
                {
                    c.GetSyncBuffer().Enqueue(json);
                }
            }
            else
            {
                //如果收到同步信息，没有该物体，生成该物体
                
            }
            
            
        }
    }
}