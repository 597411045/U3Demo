using System;
using System.Text;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using CS.Log;
using CS.Network;

namespace CS.Cmd
{
    public class CmdManagement
    {
        public static CmdManagement SingleTon;

        private Dictionary<string, CmdBase> Cmds;
        private Dictionary<string, Type> avaliableCmd; //可识别的Cmd集合
        private List<string> PendingDeleteCmds; //记录要删除的Cmd Key

        public CmdManagement()
        {
            SingleTon = this;

            Cmds = new Dictionary<string, CmdBase>();
            PendingDeleteCmds = new List<string>();

            avaliableCmd = new Dictionary<string, Type>();
            avaliableCmd.Add(typeof(WhoAreYouRequest).Name, typeof(Cmd_WhoAreYou));
            avaliableCmd.Add(typeof(WhoAreYouResponse).Name, typeof(Cmd_WhoAreYou));
            avaliableCmd.Add(typeof(BroadCastRequest).Name, typeof(Cmd_BroadCast));
            avaliableCmd.Add(typeof(BroadCastResponse).Name, typeof(Cmd_BroadCast));
            avaliableCmd.Add(typeof(TransformSyncRequest).Name, typeof(Cmd_TransformSync));
            avaliableCmd.Add(typeof(TransformSyncResponse).Name, typeof(Cmd_TransformSync));
        }

        public void CommandExec(string msg, Client c)
        {
            //处理消息，划分head和content
            if (!msg.Contains("|")) return;
            int index = msg.IndexOf("|");
            string head = msg.Substring(0, index);
            string proto = msg.Substring(index + 1, msg.Length - (index + 1));


            if (avaliableCmd.ContainsKey(head)) //判断是否有此类型的指令
            {
                //构建Cmd类，因为proto的解析在Cmd类里，必须要new一个cmd
                Type t = avaliableCmd[head];
                Object obj = t.Assembly.CreateInstance(t.FullName);
                CmdBase cmd = (CmdBase)obj;

                if (head.Contains("Response")) //有response，说明已经有一个相同token的request在执行列表中
                {
                    cmd.RecvResponse(proto); //recv方法的时候，会去检测发起request的cmd，然而此cmd就没有了，可能有性能问题
                }

                if (head.Contains("Request")) //收到request时，RecvRequest里会将此Cmd加入执行队列
                {
                    cmd.other = c; //手动赋值Client
                    cmd.RecvRequest(proto);
                }
            }
        }

        public void AddCmd(CmdBase cmd)
        {
            if (!Cmds.ContainsKey(cmd.token))
            {
                Cmds.Add(cmd.token, cmd);
            }
            else
            {
                LogManagement.Log("Existed Token:" + cmd.token);
            }
        }

        public CmdBase GetCmd(string token)
        {
            if (Cmds.ContainsKey(token))
            {
                return Cmds[token];
            }

            return null;
        }

        public void Process() //仅针对cmd的状态作处理
        {
            CheckValidCmd();

            foreach (var i in Cmds)
            {
                switch (i.Value.state)
                {
                    case CmdState.UnprocessRequest:
                        i.Value.SendRequest();
                        break;
                    case CmdState.UnprocessResponse:
                        i.Value.SendResponse();
                        break;
                    case CmdState.RequestSent:
                        if (i.Value.RetryCount > 2)
                        {
                            i.Value.state = CmdState.OutRetryLimit;
                            break;
                        }

                        i.Value.SendRequest();
                        i.Value.RetryCount++;
                        break;
                    case CmdState.RequestReplied:
                        break;
                    case CmdState.ResponseDone:
                        break;
                    case CmdState.OutRetryLimit:
                        break;
                    case CmdState.PendingDestroy:
                        break;
                    default:
                        break;
                }
            }
        }

        private void CheckValidCmd() //针对cmd状态，进行清理
        {
            foreach (var i in Cmds)
            {
                if (i.Value.state == CmdState.OutRetryLimit)
                {
                    i.Value.state = CmdState.PendingDestroy;
                    i.Value.other.state = ClientState.PendingDestroy;
                }

                if (DateTime.Now.Subtract(i.Value.timer).Seconds > 10)
                {
                    i.Value.state = CmdState.PendingDestroy;
                }

                if (i.Value.state == CmdState.PendingDestroy)
                {
                    PendingDeleteCmds.Add(i.Key);
                }
            }

            foreach (var i in PendingDeleteCmds)
            {
                Cmds.Remove(i);
            }

            LogManagement.Log("CmdCount:" + Cmds.Count);
        }
    }
}