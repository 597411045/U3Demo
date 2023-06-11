using System.Text;
using PRG.Network;
using RPG.UI;
using UnityEngine;

namespace RGP.Cmd
{
    public class CMDSyncObject
    {
        //3.2客户端发送出同步信息
        public void Send(string siUid, string json)
        {
            Debug.LogError("SendSyncObject");
            //构建协议字符
            string cmd = $"SendSyncObject|{json}";
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[3.2]服务端接收到同步信息
        public  void Recv(string cmd)
        {
            Debug.LogError("RecSyncObject");
        }
    }
}