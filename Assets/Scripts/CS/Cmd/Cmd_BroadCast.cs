using System;
using System.Text;
using CS.Network;
using ProtoMsg;
using Google.Protobuf;
using System.Collections.Generic;
using CS.Log;

namespace CS.Cmd
{
    public class Cmd_BroadCast : CmdBase
    {
        BroadCastRequest request;
        BroadCastResponse response;

        //仅用作发送
        public Cmd_BroadCast(Client _Other, BroadCastRequest _request) : base(_Other)
        {
            request = _request;
            request.Token = token;
        }

        public Cmd_BroadCast() : base()
        {
        }

        public override void SendRequest()
        {
            //发送
            if (request == null) return;
            other.send.sendList.Enqueue(Encoding.ASCII.GetBytes(request.GetType().Name + "|" + request.ToString()));
            //发送完成
            state = CmdState.ResponseDone;
            UpdateTime();
        }

        public override void SendResponse()
        {
            //构建response
            response = new BroadCastResponse() { Msg = "Broad OK", Token = token };
            //加入Client发送队列
            other.send.sendList.Enqueue(Encoding.ASCII.GetBytes(response.GetType().Name + "|" + response.ToString()));
            //回复完成
            state = CmdState.ResponseDone;
            UpdateTime();
        }

        public override void RecvRequest(string proto)
        {
            //收到了Request
            request = BroadCastRequest.Parser.ParseJson(proto);
            token = request.Token;
            //Cmd留存
            JoinCmdExecDic();
            //准备发送response
            state = CmdState.UnprocessResponse;
            UpdateTime();
            
            //业务处理
            foreach (var i in NetworkManagement.SingleTon.ClientList)
            {
                //直接将要广播的消息加入各自Client发送队列
                i.send.sendList.Enqueue(Encoding.ASCII.GetBytes(request.Msg));
            }
        }

        public override void RecvResponse(string proto)
        {
            //收到了response
            response = BroadCastResponse.Parser.ParseJson(proto);
            if (token != response.Token)
            {
                CmdManagement.SingleTon.GetCmd(response.Token)?.RecvResponse(proto);
                return;
            }

            ;
            //业务处理
            LogManagement.Log(response.Msg);
            //Cmd完成
            state = CmdState.RequestReplied;
            UpdateTime();
        }
    }
}