using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using CS.Log;

namespace CS.Cmd
{
    public enum CmdState
    {
        WaitingResponse,
        ResponseGot,
        ResponseSent,
        OutRetryLimit,
        PendingDestroy
    }

    public class CmdBase
    {
        public User UserRef { get; set; }
        public CmdState State { get; set; }
        private int RetryCount = 0;
        public string Token { get; set; }
        private DateTime timer;

        public CmdBase() //一般用于response
        {
            Token = System.Guid.NewGuid().ToString();
        }

        public CmdBase(User c) //一般用于request
        {
            Token = System.Guid.NewGuid().ToString();
            UserRef = c;
            timer = DateTime.Now;
        }


        public virtual void PassRequestToSendBuffer()
        {
            UpdateTime();
            State = CmdState.WaitingResponse;
        }


        public virtual void PassResponseToSendBuffer()
        {
            UpdateTime();
            State = CmdState.ResponseSent;
        }

        public virtual void ExecRequest(string proto)
        {
            UpdateTime();
        }

        public virtual void ExecResponse(string proto)
        {
            UpdateTime();
            State = CmdState.ResponseGot;
        }

        protected void UpdateTime() //记录上一次操作等时间
        {
            timer = DateTime.Now;
        }

        //已弃用，标识002，不允许Cmd主动注册到CmdManagement，只能CmdManagement注册Cmd
        // public void JoinCmdExecDic() //将自身加入到Cmd执行列表
        // {
        //     CmdManagement.SingleTon.AddCmd(this);
        // }

        public void CheckState()
        {
            switch (State)
            {
                case CmdState.WaitingResponse:
                    if (RetryCount > 2)
                    {
                        State = CmdState.OutRetryLimit;
                        break;
                    }

                    if (RetryCount == 0)
                    {
                        RetryCount++;
                        break;
                    }

                    RetryCount++;
                    LogManagement.SingleTon.LogNetContentOnlyInFile(this.GetType().Name, "CheckState",
                        UserRef.Send.GetRemoteEndPoint(), UserRef.Name, $"Retry Count:RetryCount{RetryCount}");
                    PassRequestToSendBuffer();


                    break;
                case CmdState.ResponseGot:
                case CmdState.ResponseSent:
                    if (DateTime.Now.Subtract(timer).Seconds > 5)
                    {
                        State = CmdState.PendingDestroy;
                    }

                    break;
                case CmdState.OutRetryLimit:
                    if (DateTime.Now.Subtract(timer).Seconds > 10)
                    {
                        State = CmdState.PendingDestroy;
                        UserRef.DoDestroy();
                    }

                    break;
                case CmdState.PendingDestroy:
                    break;
                default:
                    break;
            }
        }

        public bool IfDestroy()
        {
            if (State == CmdState.PendingDestroy)
            {
                return true;
            }

            return false;
        }
    }

    public class CmdBase2<T, D> : CmdBase
    {
        protected T request;
        protected D response;

        public CmdBase2(User _user, T _request) : base(_user)
        {
            request = _request;
        }

        public CmdBase2() : base()
        {
        }

        public override void PassRequestToSendBuffer()
        {
            if (request != null)
            {
                UserRef.Send.AddMsg(Encoding.ASCII.GetBytes(request.GetType().Name + "|" + request.ToString()));
                LogManagement.SingleTon.LogNetContentOnlyInFile(this.GetType().Name, "PassRequestToSendBuffer",
                    UserRef.Send.GetRemoteEndPoint(), UserRef.Name, request.ToString());
            }

            base.PassRequestToSendBuffer();
        }


        public override void PassResponseToSendBuffer()
        {
            if (response != null)
            {
                UserRef.Send.AddMsg(Encoding.ASCII.GetBytes(response.GetType().Name + "|" + response.ToString()));
                LogManagement.SingleTon.LogNetContentOnlyInFile(this.GetType().Name, "PassResponseToSendBuffer",
                    UserRef.Send.GetRemoteEndPoint(), UserRef.Name, response.ToString());
            }

            base.PassResponseToSendBuffer();
        }

        public override void ExecRequest(string proto)
        {
            LogManagement.SingleTon.LogNetContentOnlyInFile(this.GetType().Name, "ExecRequest",
                UserRef.Send.GetRemoteEndPoint(), UserRef.Name, proto);
            PassResponseToSendBuffer();
            base.ExecRequest(proto);
        }

        public override void ExecResponse(string proto)
        {
            LogManagement.SingleTon.LogNetContentOnlyInFile(this.GetType().Name, "ExecResponse",
                UserRef.Send.GetRemoteEndPoint(), UserRef.Name, proto);
            base.ExecResponse(proto);
        }
    }
}