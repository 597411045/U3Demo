using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;

namespace CS.Cmd
{
    public class Cmd_WhoAreYou : CmdBase2<WhoAreYouRequest, WhoAreYouResponse>
    {
        public Cmd_WhoAreYou(User _user, WhoAreYouRequest _request) : base(_user, _request)
        {
            request.Token = Token;
        }

        public Cmd_WhoAreYou() : base()
        {
        }


        public override void PassRequestToSendBuffer()
        {
            //
            UserRef.SetStatePendingValid();
            //
            base.PassRequestToSendBuffer();
        }


        public override void PassResponseToSendBuffer()
        {
            //
            response = new WhoAreYouResponse() { ClientName = Environment.MachineName, Token = Token };

            //
            base.PassResponseToSendBuffer();
        }

        public override void ExecRequest(string proto)
        {
            //
            request = WhoAreYouRequest.Parser.ParseJson(proto);
            if (Token != request.Token)
            {
                CmdBase CmdAgent = CmdManagement.SingleTon.GetCmdByToken(request.Token);
                if (CmdAgent != null)
                {
                    CmdAgent.ExecResponse(proto);
                    return;
                }
                else
                {
                    Token = request.Token;
                    CmdManagement.SingleTon.AddRequestedCmdInCookedDic(this);
                }
            }

            //
            UserRef.Name = request.ServerName;
            UserRef.NewUserAsServer();
            //
            base.ExecRequest(proto);
        }

        public override void ExecResponse(string proto)
        {
            //
            response = WhoAreYouResponse.Parser.ParseJson(proto);
            if (Token != response.Token)
            {
                CmdBase CmdAgent = CmdManagement.SingleTon.GetCmdByToken(response.Token);
                if (CmdAgent != null)
                {
                    CmdAgent.ExecResponse(proto);
                    return;
                }
            }

            //
            UserRef.Name = response.ClientName + "," + UserRef.Send.GetRemoteEndPoint();
            UserRef.NewUserAsClient();
            //
            base.ExecResponse(proto);
        }
    }
}