using System.Text;
using PRG.Cmd;
using PRG.Network;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDLogin : CMDBase<CMDLogin>
    {
        public CMDLogin() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<Username><Password>";
        }

        //1，客户端率先发起登录信息
        public override void Send(string siUid, params string[] paras)
        {
            string cmd = ReplaceParam(paras);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[1].服务器收到登录信息
        public override void Recv(string siid, string cmd)
        {
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);


            //获取,验证客户端信息（状态，Transform等）
            //TODO:获取信息
            //服务器本地创建
            string PrefabName = "Player";
            string GObjectName = GetParam(cmd, 0);
            Vector3 position = Vector3.zero;
            SceneEntityManager.GeneratePurePrefab("Player", GObjectName, position, siid);

            CMDChangeScene.Ins.Send(siid, "Assets/Scenes/Sandbox 1 Client/Sandbox 1 Client.unity");
            CMDGeneratePrefab.Ins.Send(siid, PrefabName, GObjectName);
        }
    }
}