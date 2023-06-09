﻿using System.Text;
using PRG.Cmd;
using PRG.Network;
using PRG.Sync;
using RPG.Scene;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDGeneratePrefab : CMDBase<CMDGeneratePrefab>
    {
        public CMDGeneratePrefab() : base()
        {
            CmdFormat = $"{this.GetType().Name}|<PrefabName><GameObjectName><YouAreOwner>";
        }

        //2.2服务器回复创建玩家
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromUid"></param>
        /// <param name="para">1:PrefabName,2:GameObjectName,3:YouAreOwner</param>
        public override void Send(string fromUid, params string[] paras)
        {
            string cmd = ReplaceParam(paras);
            NetworkManagement.Ins.SendMessageBySocketUID(fromUid,
                Encoding.UTF8.GetBytes(cmd));
            CmdManagement.Ins.LogOnScreen("Send:" + cmd);
        }

        //[2.2].客户端收到创建玩家消息，进行玩家创建
        public override void Recv(string siid, string cmd)
        {
            CmdManagement.Ins.LogOnScreen("Recv:" + cmd);

            string PrefabName = GetParam(cmd, 0);
            string GameObjectName = GetParam(cmd, 1);
            string YouAreOwner = GetParam(cmd, 2);
            //PTTransform ptt = PTTransform.Parser.ParseJson(PTT);
            //Vector3 position = new Vector3(ptt.PositionX, ptt.PositionY, ptt.PositionZ);
            if (YouAreOwner == "TRUE")
            {
                SyncManagement.Ins.syncSIID.Add(siid);
                siid = "";
                //客户端：仅将服务器作为广播对象
            }
            SceneEntityManager.GeneratePurePrefab(PrefabName, GameObjectName, Vector3.zero, siid);
        }
    }
}