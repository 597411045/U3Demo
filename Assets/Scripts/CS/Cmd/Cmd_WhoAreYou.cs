using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;

namespace CS.Cmd
{
    public class Cmd_WhoAreYou : CmdBase
    {
        WhoAreYouRequest request;
        WhoAreYouResponse response;

        //仅用作发送
        public Cmd_WhoAreYou(Client _Other, WhoAreYouRequest _request) : base(_Other)
        {
            request = _request;
            request.Token = token;
        }

        public Cmd_WhoAreYou() : base()
        {
        }

        public override void SendRequest()
        {
            //发送
            if (request == null) return;
            other.send.sendList.Enqueue(Encoding.ASCII.GetBytes(request.GetType().Name + "|" + request.ToString()));
            //发送完成
            state = CmdState.RequestSent;
            UpdateTime();

            //业务处理
            other.state = ClientState.PendingValid;
        }

        public override void SendResponse()
        {
            //构建response
            response = new WhoAreYouResponse() { ClientName = Environment.MachineName, Token = token };
            //加入Client发送队列
            other.send.sendList.Enqueue(Encoding.ASCII.GetBytes(response.GetType().Name + "|" + response.ToString()));
            //回复完成
            state = CmdState.ResponseDone;
            UpdateTime();
        }

        public override void RecvRequest(string proto)
        {
            //收到了Request
            request = WhoAreYouRequest.Parser.ParseJson(proto);
            token = request.Token;
            //Cmd留存
            JoinCmdExecDic();
            //准备发送response
            state = CmdState.UnprocessResponse;
            UpdateTime();

            //业务处理
            other.name = request.ServerName;
            other.state = ClientState.Valid;
        }

        public override void RecvResponse(string proto)
        {
            //收到了response，去寻找发出request的cmd
            response = WhoAreYouResponse.Parser.ParseJson(proto);
            if (token != response.Token)
            {
                CmdManagement.SingleTon.GetCmd(response.Token)?.RecvResponse(proto);
                return;
            }

            //收到回复
            state = CmdState.RequestReplied;
            UpdateTime();

            //业务处理
            other.name = "Valid:" + response.ClientName + ",:" + other.send.socket.RemoteEndPoint.ToString();
            other.state = ClientState.Valid;
        }
    }
}