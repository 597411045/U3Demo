using System.Text;
using System.Text.RegularExpressions;
using PRG.Cmd;
using PRG.Network;
using RPG.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Cmd
{
    public class CMDChangeScene : CMDBase<CMDChangeScene>
    {
        public CMDChangeScene() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<SceneName>";
        }

        //2.1 发送切换场景
        //TODO:场景服务器

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siUid"></param>
        /// <param name="para">1个参数：场景名</param>
        public override void Send(string siUid, params string[] paras)
        {
            //构建协议字符
            string cmd = ReplaceParam(paras);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[2.1]客户端收到切换场景消息，进行场景切换
        public override void Recv(string siid, string cmd)
        {
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);

            //TODO:需要携程进行后续操作,携程内容：切换场景-屏幕渐变+生成角色-发送同步请求
            string SceneName = GetParam(cmd, 0);
            SceneManager.LoadScene(SceneName);
            
        }
    }
}