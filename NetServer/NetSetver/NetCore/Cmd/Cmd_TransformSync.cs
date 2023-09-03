using System;
using System.Text;
using NetSetver.NetCore.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;

namespace NetSetver.NetCore.Cmd
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

        public Cmd_TransformSync()
        {
        }

        public override void SendRequest()
        {
            //发送
            if (request == null) return;
            //int j = request.CalculateSize();
            //byte[] tmp = new byte[j];
            //request.WriteTo(tmp);
            other.send.sendList.Enqueue(Encoding.ASCII.GetBytes(request.GetType().Name + "|" + request.ToString()));

            //仅做简单发送成功验证
            state = CmdState.RequestSent;
            UpdateTime();

        }

        public override void SendResponse()
        {
            //构建response
           // response = new TransformSyncResponse() { Msg = "Broad OK", Token = token };

            //业务处理
            //发送
            //int j = response.CalculateSize();
            //byte[] tmp = new byte[j];
            //response.WriteTo(tmp);
            //other.send.sendList.Enqueue(Encoding.ASCII.GetBytes(response.GetType().Name + "|" + Encoding.ASCII.GetString(tmp)));

            //仅做简单发送成功验证
            state = CmdState.ResponseDone;
            UpdateTime();

        }

        public override void RecvRequest(string proto)
        {
            //收到了Request
            request = TransformSyncRequest.Parser.ParseJson(proto);
            //token = request.Token;

            Console.WriteLine(request.ToString());

            //Cmd留存
            //CmdManagement.SingleTon.AddCmd(this);


            //准备发送response
            state = CmdState.PendingDestroy;
            UpdateTime();

        }

        public override void RecvResponse(string proto)
        {
            //收到了response
            response = TransformSyncResponse.Parser.ParseJson(proto);
            if (token != response.Token)
            {
                CmdManagement.SingleTon.GetCmd(response.Token)?.RecvResponse(proto);
                return;
            };
            //业务处理
            Console.WriteLine(response.Msg);
            //Cmd完成
            state = CmdState.RequestReplied;
            UpdateTime();

        }
    }

}