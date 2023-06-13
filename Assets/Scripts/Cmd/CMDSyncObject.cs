using System.Text;
using PRG.Network;
using PRG.Sync;
using RPG.Cmd;
using RPG.UI;
using UnityEngine;

namespace RGP.Cmd
{
    public class CMDSyncObject : CMDBase<CMDSyncObject>
    {
        public CMDSyncObject() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<PTT>";
        }

        //3.2客户端发送出同步信息
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


            string PTT = GetParam(cmd, 0);
            PTTransform ptt = PTTransform.Parser.ParseJson(PTT);
            Vector3 position = new Vector3(ptt.PositionX, ptt.PositionY, ptt.PositionZ);

            foreach (var c in GameObject.Find(ptt.GameObjectName).GetComponents<ISyncObject>())
            {
                if (((Component)c).name.Equals(ptt.ComponentName))
                {
                    c.SyncObject = ptt;
                }
            }
        }
    }
}