using System.Text;
using PRG.Network;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDLogin : CMDBase
    {
        public static CMDLogin Ins;

        public CMDLogin() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<Username><Password>";
            Ins = this;
        }

        //1，客户端率先发起登录信息
        public override void Send(string siUid, params string[] para)
        {
            string cmd = CmdFormat.Replace(GetParam(CmdFormat, 0), para[0])
                .Replace(GetParam(CmdFormat, 1), para[1]);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[1].服务器收到登录信息
        public override void Recv(string cmd)
        {
            //获取,验证客户端信息（状态，Transform等）
            //TODO:获取信息
            //服务器本地创建
            string PrefabName = "Player";
            string GObjectName = GetParam(cmd, 1);
            Vector3 position = Vector3.zero;
            SceneEntityManager.GeneratePurePrefab("Player", GObjectName, position);
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);
        }
    }
}