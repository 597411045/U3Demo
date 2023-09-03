using System;
using System.Text;
using NetSetver.NetCore.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;

namespace NetSetver.NetCore.Cmd
{
    public enum CmdState
    {
        UnprocessRequest,
        UnprocessResponse,
        RequestSent,
        RequestReplied,
        ResponseDone,
        RequestTimeOut,
        PendingDestroy
    }


    public class CmdBase
    {
        public Client other;
        public CmdState state;
        public int RetryCount = 0;
        public string token;
        public DateTime timer;

        public CmdBase()
        {
            token = System.Guid.NewGuid().ToString();
        }

        public CmdBase(Client c)
        {
            token = System.Guid.NewGuid().ToString();
            other = c;
            state = CmdState.UnprocessRequest;
            CmdManagement.SingleTon.AddCmd(this);
            timer = DateTime.Now;
        }

        public virtual void RecvRequest(string proto)
        {
            UpdateTime();
        }

        public virtual void RecvResponse(string proto)
        {
            UpdateTime();
        }

        public virtual void SendRequest()
        {
            UpdateTime();
        }

        public virtual void SendResponse()
        {
            UpdateTime();
        }

        protected void UpdateTime()
        {
            timer = DateTime.Now;
        }
    }


    public class CmdManagement
    {
        public static CmdManagement SingleTon;

        private Dictionary<string, CmdBase> Cmds;
        private Dictionary<string, Type> avaliableCmd;
        private List<string> PendingDeleteCmds;

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
            //收到了任意消息
            if (!msg.Contains("|")) return;
            int index = msg.IndexOf("|");
            string head = msg.Substring(0, index);
            string proto = msg.Substring(index + 1, msg.Length - (index + 1));

            if (avaliableCmd.ContainsKey(head))//判断是否有此类型的指令
            {
                //构建Cmd类
                Type t = avaliableCmd[head];
                Object obj = t.Assembly.CreateInstance(t.FullName);
                CmdBase cmd = (CmdBase)obj;

                if (head.Contains("Response"))
                {
                    cmd.RecvResponse(proto);
                }
                if (head.Contains("Request"))
                {
                    cmd.other = c;
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
                Console.WriteLine("Existed Token:" + cmd.token);
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

        public void Process()
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
                            i.Value.state = CmdState.RequestTimeOut;
                            break;
                        }
                        i.Value.SendRequest();
                        i.Value.RetryCount++;
                        break;
                    case CmdState.RequestReplied:
                        break;
                    case CmdState.ResponseDone:
                        break;
                    case CmdState.RequestTimeOut:
                        break;
                    case CmdState.PendingDestroy:
                        break;
                    default:
                        break;
                }
            }
        }

        private void CheckValidCmd()
        {
            foreach (var i in Cmds)
            {
                if (i.Value.state == CmdState.RequestTimeOut)
                {
                    i.Value.other.isPendingDestroy = true;
                    i.Value.state = CmdState.PendingDestroy;
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

            Console.WriteLine("CmdCount:" + Cmds.Count);
        }
    }
}