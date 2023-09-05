using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using CS.Log;

namespace CS.Cmd
{
    public class Cmd_TransformSync : CmdBase
    {
        TransformSyncRequest request;
        TransformSyncResponse response;

        //仅用作发送
        public Cmd_TransformSync(Client _Other, TransformSyncRequest _request) : base(_Other)
        {
            request = _request;
            request.Token = token;
        }

        public Cmd_TransformSync():base()
        {
        }
        
        public override void RecvRequest(string proto)
        {
            //收到了Request
            request = TransformSyncRequest.Parser.ParseJson(proto);

            //此业务无需Response
            state = CmdState.ResponseDone;
            UpdateTime();

            //业务处理
            LogManagement.Log("Get Sync:"+request.ToString());
        }
    }

}