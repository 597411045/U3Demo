using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using CS.Log;

namespace CS.Cmd
{
    public class Cmd_TransformSync : CmdBase2<TransformSyncRequest, TransformSyncResponse>
    {
        //仅用作发送
        public Cmd_TransformSync(User _user, TransformSyncRequest _request) : base(_user, _request)
        {
            request.Token = Token;
        }

        public Cmd_TransformSync() : base()
        {
        }

        public override void ExecRequest(string proto)
        {
            //
            request = TransformSyncRequest.Parser.ParseJson(proto);
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
            LogManagement.SingleTon.Log("", "", $"!!!GET ASYNC{request.GameObjectName}");
            //
            base.ExecRequest(proto);
        }
    }
}