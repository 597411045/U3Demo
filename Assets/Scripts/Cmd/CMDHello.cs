using System.Text;
using PRG.Cmd;
using PRG.Network;
using RPG.UI;

namespace RPG.Cmd
{
    public class CMDHello : CMDBase
    {
        public static CMDHello Ins;

        public CMDHello() : base()
        {
            CommandExecuter.Ins.RegisterCmd(this.GetType().Name, this);
            CmdFormat = $"{this.GetType().Name}|<Hello>";
            Ins = this;
        }


        //3.1服务器回复允许同步
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siUid"></param>
        /// <param name="para">1:GameObjectName</param>
        public override void Send(string siUid, params string[] para)
        {
            //构建协议字符
            string cmd = CmdFormat.Replace(GetParam(CmdFormat, 0), para[0]);
            NetworkManagement.Ins.SendMessageBySocketUID(siUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        public void Send(SocketInstance si, params string[] paras)
        {
            //构建协议字符
            string cmd = ReplaceParam(paras);
            si.sendList.Enqueue(Encoding.UTF8.GetBytes(cmd));
            TaskPipelineManager.Ins.Register(() => { CmdManagement.Ins.LogOnScreen("SendInBuffer:" + cmd); });
        }

        // //[3.1]客户端接收到允许同步，开启此物体的同步功能
        public override void Recv(string siid, string cmd)
        {
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);

            string hello = GetParam(cmd, 0);
            if (!NetworkManagement.isServer)
            {
                CMDLogin.Ins.Send(siid, "Username", "Password");
            }
        }
    }
}