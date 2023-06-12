using System.Text;
using PRG.Network;
using RPG.Cmd;
using RPG.UI;
using UnityEngine;

namespace RGP.Cmd
{
    public class CMDSyncObject : CMDBase
    {
        public static CMDSyncObject Ins;

        public CMDSyncObject() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<PTT>";
            Ins = this;
        }

        //3.2客户端发送出同步信息
        public override void Send(string siUid, params string[] para)
        {
            string cmd = CmdFormat.Replace(GetParam(CmdFormat, 0), para[0]);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[3.2]服务端接收到同步信息
        public override void Recv(string cmd)
        {
            string PTT = GetParam(cmd, 0);
            PTTransform ptt = PTTransform.Parser.ParseJson(PTT);
            Vector3 position = new Vector3(ptt.PositionX, ptt.PositionY, ptt.PositionZ);
            
            GameObject.Find()

            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);
        }
    }
}