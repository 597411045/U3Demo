using System.Text;
using PRG.Network;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDGeneratePrefab : CMDBase
    {
        public static CMDGeneratePrefab Ins;

        public CMDGeneratePrefab() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<PrefabName><PTT>";
            Ins = this;
        }

        //2.2服务器回复创建玩家
        public override void Send(string fromUid, params string[] para)
        {
            string cmd = CmdFormat.Replace(GetParam(CmdFormat, 0), para[0])
                .Replace(GetParam(CmdFormat, 1), para[1]);
            NetworkManagement.Ins.SendMessageBySocketUID(fromUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[2.2].客户端收到创建玩家消息，进行玩家创建
        public override void Recv(string cmd)
        {
            string PrefabName = GetParam(cmd, 0);
            string PTT = GetParam(cmd, 1);
            PTTransform ptt = PTTransform.Parser.ParseJson(PTT);
            Vector3 position = new Vector3(ptt.PositionX, ptt.PositionY, ptt.PositionZ);
            SceneEntityManager.GeneratePurePrefab(PrefabName, ptt.GameObjectName, position);
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);
        }
    }
}