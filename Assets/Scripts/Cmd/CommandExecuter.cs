using System.Collections.Generic;
using System.Linq;
using System.Text;
using RGP.Cmd;
using RPG.Cmd;
using RPG.Control;
using RPG.Core;
using RPG.Scene;
using RPG.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PRG.Cmd
{
    public class CommandExecuter
    {
        public static CommandExecuter Ins;
        private Dictionary<string, CMDBase> avaliableCmd;

        public CommandExecuter()
        {
            if (Ins == null)
            {
                Debug.LogError(this.ToString() + " Construction");
                Ins = this;
            }
            else
            {
                Debug.LogError("For Now, Only One " + this.ToString() + " Allowed");
            }

            avaliableCmd = new Dictionary<string, CMDBase>();

            CMDChangeScene cmdChangeScene = new CMDChangeScene();
            avaliableCmd.Add(cmdChangeScene.GetType().Name,cmdChangeScene);
            CMDGeneratePrefab cmdGeneratePrefab = new CMDGeneratePrefab();
            avaliableCmd.Add(cmdGeneratePrefab.GetType().Name,cmdGeneratePrefab);
        }

        

        public void CommandExec(string fromUid, string cmd)
        {
            //初始化阶段
            //0.客户端收到服务器链接成功的消息
            //1，客户端率先发起登录信息
            if (cmd.Equals("Hello Client"))
            {
                CMDLogin.Ins.Send(fromUid, "TestClient", "TestClient");
                return;
            }

            //[1].服务器收到登录信息
            if (cmd.Contains("SendLogin|"))
            {
                CMDLogin.Recv(cmd);
                //2.1 发送切换场景
                CMDChangeScene.Ins.Send(fromUid, "Scenes/Sandbox 1 Client/Sandbox 1 Client");

                //2.2服务器回复创建玩家
                //TODO:广播
                CMDGeneratePrefab.Ins.Send(fromUid, "Player", "TestTransform");
                return;
            }

            //[2.1]客户端收到切换场景消息，进行场景切换
            if (cmd.Contains("SendChangeScene|"))
            {
                CMDChangeScene.Ins.Recv(cmd);
                return;
            }

            //[2.2].客户端收到创建玩家消息，进行玩家创建
            if (cmd.Contains("SendGeneratePrefab|"))
            {
                CMDGeneratePrefab.Ins.Recv(cmd);
                return;
            }


            //3接着创建完成后，controller会发出同步请求协议

            //同步阶段
            //[3]服务器收到同步协议，根据同步列表是否有此物体，进行回复
            if (cmd.Contains("SendSyncRequest|"))
            {
                string param;
                if (CMDSyncRequest.Ins.Recv(cmd, out param))
                {
                    //3.1服务器回复允许同步
                    CMDSyncRequestAllow.Ins.Send(fromUid, param);
                }

                return;
            }

            //[3.1]客户端接收到允许同步，开启此物体的同步功能
            if (cmd.Contains("SendSyncRequestAllow|"))
            {
                CMDSyncRequestAllow.Ins.Recv(cmd);
                return;
            }

            //3.2客户端发送出同步信息
            //[3.2]服务端接收到同步信息
            if (cmd.Contains("SendSyncObject|"))
            {
                CMDSyncObject.Ins.Recv(cmd);
                return;
            }

            CmdManagement.Ins.LogOnScreen("UNKNOWN Recv:" + cmd);
        }
    }
}