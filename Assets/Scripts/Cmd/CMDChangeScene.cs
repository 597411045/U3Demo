using System.Text;
using System.Text.RegularExpressions;
using PRG.Network;
using RPG.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Cmd
{
    public class CMDChangeScene : CMDBase
    {
        public static CMDChangeScene Ins;

        public CMDChangeScene() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<SceneName>";
            Ins = this;
        }

        //2.1 发送切换场景
        //TODO:场景服务器

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siUid"></param>
        /// <param name="para">1个参数：场景名</param>
        public override void Send(string siUid, params string[] para)
        {
            //构建协议字符
            string cmd = CmdFormat.Replace(GetParam(CmdFormat, 0), para[0]);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[2.1]客户端收到切换场景消息，进行场景切换
        public override void Recv(string cmd)
        {
            //TODO:需要携程进行后续操作,携程内容：切换场景-屏幕渐变+生成角色-发送同步请求
            string SceneName = GetParam(cmd, 0);
            SceneManager.LoadScene(SceneName);
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);
        }
    }
}