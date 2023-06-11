using System.Text;
using PRG.Network;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDLogin
    {
        //1，客户端率先发起登录信息
        public void Send(string siUid, string username, string password)
        {
            Debug.LogError("SendLogin");
            //构建协议字符
            string cmd = $"SendLogin|ID:{username}|PWD:{password}";

            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));

            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[1].服务器收到登录信息
        public void Recv(string cmd)
        {
            Debug.LogError("RecvLogin");
            //获取客户端信息（状态，Transform等）
            //TODO:获取信息
            //服务器本地创建
            SceneEntityManager.GeneratePlayerPrefab(cmd.Split('|')[1].Split(':')[1]);
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);
        }
    }
}