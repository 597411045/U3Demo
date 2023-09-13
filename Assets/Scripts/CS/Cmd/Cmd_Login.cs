using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using CS.Log;

namespace CS.Cmd
{
    public class Cmd_Login : CmdBase2<LoginRequest, LoginResponse>
    {
        //仅用作发送
        public Cmd_Login(User _user, LoginRequest _request) : base(_user, _request)
        {
            request.Token = Token;
        }

        public Cmd_Login() : base()
        {
        }

        public override void PassResponseToSendBuffer()
        {
            //
            response = new LoginResponse() { Result = "OK", Token = Token };
            //
            base.PassResponseToSendBuffer();
        }

        public override void ExecRequest(string proto)
        {
            //
            request = LoginRequest.Parser.ParseJson(proto);
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
            ChangeSceneRequest newRequest = new ChangeSceneRequest() { SceneName = "Scenes/Play/Play" };
            Cmd_ChangeScene cmd = new Cmd_ChangeScene(UserRef, newRequest);
            CmdManagement.SingleTon.AddNewRequestCmdInCookedDicAndSend(cmd);
            //
            base.ExecRequest(proto);
        }

        public override void ExecResponse(string proto)
        {
            //
            response = LoginResponse.Parser.ParseJson(proto);
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