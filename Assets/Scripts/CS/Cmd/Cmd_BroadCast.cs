using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using CS.Log;

namespace CS.Cmd
{
    public class Cmd_BroadCast : CmdBase2<BroadCastRequest, BroadCastResponse>
    {
        public Cmd_BroadCast(User _user, BroadCastRequest _request) : base(_user, _request)
        {
            request.Token = Token;
        }

        public Cmd_BroadCast() : base()
        {
        }


        public override void PassResponseToSendBuffer()
        {
            //
            response = new BroadCastResponse() { Msg = "Broad OK", Token = Token };
            //
            base.PassResponseToSendBuffer();
        }

        public override void ExecRequest(string proto)
        {
            //
            request = BroadCastRequest.Parser.ParseJson(proto);
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
            foreach (var i in NetworkManagement.SingleTon.GetValidClients(UserType.Client))
            {
                i.Send.AddMsg(Encoding.ASCII.GetBytes(request.Msg));
            }

            //
            base.ExecRequest(proto);
        }

        public override void ExecResponse(string proto)
        {
            //
            response = BroadCastResponse.Parser.ParseJson(proto);
            if (Token != response.Token)
            {
                CmdManagement.SingleTon.GetCmdByToken(response.Token)?.ExecResponse(proto);
                return;
            }
            //

            //
            base.ExecResponse(proto);
        }
    }
}