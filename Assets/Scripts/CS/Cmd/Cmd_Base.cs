using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;

namespace CS.Cmd
{
    
    
    public enum CmdState
    {
        UnprocessRequest,
        UnprocessResponse,
        RequestSent,
        RequestReplied,
        ResponseDone,
        OutRetryLimit,
        PendingDestroy
    }


    public class CmdBase
    {
        public Client other;
        public CmdState state;
        public int RetryCount = 0;
        public string token;
        public DateTime timer;

        public IMessage request;
        public IMessage response;

        public CmdBase()//一般用于response
        {
            token = System.Guid.NewGuid().ToString();
        }

        public CmdBase(Client c)//一般用于request
        {
            token = System.Guid.NewGuid().ToString();
            other = c;
            //准备发送
            state = CmdState.UnprocessRequest;
            timer = DateTime.Now;
        }

        public virtual void RecvRequest(string proto)
        {
        }

        public virtual void RecvResponse(string proto)
        {
        }

        public virtual void SendRequest()
        {
        }

        public virtual void SendResponse()
        {
        }

        protected void UpdateTime()//记录上一次操作等时间
        {
            timer = DateTime.Now;
        }

        public void JoinCmdExecDic()//将自身加入到Cmd执行列表
        {
            CmdManagement.SingleTon.AddCmd(this);
        }
    }


   
}