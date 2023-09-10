using System;
using System.Text;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using CS.Log;
using CS.Network;

namespace CS.Cmd
{
    /*
     *根据network的流程
            //1，允许执行一次accept，接入一个Client
            //2.缓存的临时Client加入Client集合
            //3.检测上一帧过后，是否有无效的Client
            //4.对每个Client尝试接收数据
            //5.对每个Client接收的数据转交给Cmd系统，并执行
            //6.如果有其他操作可能要加入CmdList，注册后在这里执行
            //7.发送Msg
            
            第5点：msg传给cmd后，cmd会直接处理一次rec或者respone，然后加入cmd map，后面监测cmd的处理状态
            第6点：加入的新的cmd时，全部先进入buffer，然后根据状态处理，可能的状态1：全新的未发送的cmd request，需要发送；可能的状态的，由recvrequest和recvresp创建出的指令，也可以发送
     */


    public class CmdManagement : SingleTonBase<CmdManagement>
    {
        private Dictionary<string, CmdBase> CookedCmdDic; //准备好处理的指令表
        private Dictionary<string, Type> RecognizedCmd; //可识别的Cmd表
        private List<string> PendingDeleteCmdKey; //记录要删除的Cmd Key

        public CmdManagement()
        {
            CookedCmdDic = new Dictionary<string, CmdBase>();
            PendingDeleteCmdKey = new List<string>();

            RecognizedCmd = new Dictionary<string, Type>();
            RecognizedCmd.Add(typeof(WhoAreYouRequest).Name, typeof(Cmd_WhoAreYou));
            RecognizedCmd.Add(typeof(WhoAreYouResponse).Name, typeof(Cmd_WhoAreYou));
            RecognizedCmd.Add(typeof(BroadCastRequest).Name, typeof(Cmd_BroadCast));
            RecognizedCmd.Add(typeof(BroadCastResponse).Name, typeof(Cmd_BroadCast));
            RecognizedCmd.Add(typeof(TransformSyncRequest).Name, typeof(Cmd_TransformSync));
            RecognizedCmd.Add(typeof(TransformSyncResponse).Name, typeof(Cmd_TransformSync));
            RecognizedCmd.Add(typeof(LoginRequest).Name, typeof(Cmd_Login));
            RecognizedCmd.Add(typeof(LoginResponse).Name, typeof(Cmd_Login));
        }

        public void PassRecvMsgToCmdAndExec(string msg, User c)
        {
            //处理消息，划分head和content
            if (!msg.Contains("|")) return;
            int index = msg.IndexOf("|");
            string head = msg.Substring(0, index);
            string proto = msg.Substring(index + 1, msg.Length - (index + 1));


            if (RecognizedCmd.ContainsKey(head)) //判断是否有此类型的指令
            {
                //构建Cmd类，因为proto的解析在Cmd类里，必须要new一个cmd
                Type t = RecognizedCmd[head];
                Object obj = t.Assembly.CreateInstance(t.FullName);
                CmdBase cmd = (CmdBase)obj;


                if (head.Contains("Request")) //收到request时，RecvRequest里会将此Cmd加入执行队列
                {
                    cmd.UserRef = c; //手动赋值Client
                    cmd.ExecRequest(proto);
                }

                if (head.Contains("Response")) //有response，说明已经有一个相同token的request在执行列表中
                {
                    cmd.ExecResponse(proto); //recv方法的时候，会去检测发起request的cmd，然而此cmd就没有了，可能有性能问题
                }
            }
            else
            {
                LogManagement.SingleTon.Log(this.GetType().Name, "PassRecvMsgToCmdAndExec",
                    $"!!!WARNING UNKOWN CMD:{head}");
            }
        }

        public void AddRequestedCmdInCookedDic(CmdBase cmd)
        {
        }

        public bool AddNewRequestCmdInCookedDicAndSend(CmdBase cmd)
        {
            if (CookedCmdDic.ContainsKey(cmd.Token))
            {
                LogManagement.SingleTon.LogNetContent(this.GetType().Name, "AddCmdInBuffer",
                    cmd.UserRef.Send.GetRemoteEndPoint(), "CookedCmdDic Same Token");
                return false;
            }
            else
            {
                cmd.PassRequestToSendBuffer();
                CookedCmdDic.Add(cmd.Token, cmd);
                return true;
            }
        }

        public CmdBase GetCmdByToken(string token)
        {
            if (CookedCmdDic.ContainsKey(token))
            {
                return CookedCmdDic[token];
            }

            return null;
        }

        public void Process() //仅针对cmd的状态作处理
        {
            CleanDestroyedCmd();

            foreach (var i in CookedCmdDic)
            {
                i.Value.CheckState();
            }
        }

        private void CleanDestroyedCmd()
        {
            foreach (var i in CookedCmdDic)
            {
                if (i.Value.State == CmdState.PendingDestroy)
                {
                    PendingDeleteCmdKey.Add(i.Key);
                }
            }

            foreach (var i in PendingDeleteCmdKey)
            {
                //销毁流程-Cmd-执行
                CookedCmdDic.Remove(i);
            }

            PendingDeleteCmdKey.Clear();
            LogManagement.SingleTon.LogOnlyInFile(this.GetType().Name, "FlushCmdBufferAndCleanDestroyed",
                $"Current Cmd Count:{CookedCmdDic.Count}");
        }
    }
}