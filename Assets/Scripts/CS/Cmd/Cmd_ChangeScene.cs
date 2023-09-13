using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CS.Cmd
{
    public class Cmd_ChangeScene : CmdBase2<ChangeSceneRequest, ChangeSceneResponse>
    {
        public Cmd_ChangeScene(User _user, ChangeSceneRequest _request) : base(_user, _request)
        {
            request.Token = Token;
        }

        public Cmd_ChangeScene() : base()
        {
        }


        public override void PassRequestToSendBuffer()
        {
            //

            //
            base.PassRequestToSendBuffer();
        }


        public override void PassResponseToSendBuffer()
        {
            //
            response = new ChangeSceneResponse() { CurrentScene = SceneManager.GetActiveScene().name, Token = Token };

            //
            base.PassResponseToSendBuffer();
        }

        public override void ExecRequest(string proto)
        {
            //
            request = ChangeSceneRequest.Parser.ParseJson(proto);
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
            SceneManager.LoadSceneAsync(request.SceneName);
            //
            base.ExecRequest(proto);
        }

        public override void ExecResponse(string proto)
        {
            //
            response = ChangeSceneResponse.Parser.ParseJson(proto);
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
            
            //
            base.ExecResponse(proto);
        }
    }
}