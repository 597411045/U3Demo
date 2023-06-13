using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmd;
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
    public class CommandExecuter : SingleTon<CommandExecuter>
    {
        private Dictionary<string, ICMDAction> avaliableCmd;
        private IEnumerator<KeyValuePair<string, ICMDAction>> ienum;

        public CommandExecuter()
        {
            // if (Ins == null)
            // {
            //     Debug.LogError(this.ToString() + " Construction");
            //     Ins = this;
            // }
            // else
            // {
            //     Debug.LogError("For Now, Only One " + this.ToString() + " Allowed");
            // }

            avaliableCmd = new Dictionary<string, ICMDAction>();

            avaliableCmd.Add(typeof(CMDChangeScene).Name, CMDChangeScene.Ins);
            avaliableCmd.Add(typeof(CMDGeneratePrefab).Name, CMDGeneratePrefab.Ins);
            avaliableCmd.Add(typeof(CMDLogin).Name, CMDLogin.Ins);
            avaliableCmd.Add(typeof(CMDSyncObject).Name, CMDSyncObject.Ins);
            avaliableCmd.Add(typeof(CMDSyncRequest).Name, CMDSyncRequest.Ins);
            avaliableCmd.Add(typeof(CMDSyncRequestAllow).Name, CMDSyncRequestAllow.Ins);
            avaliableCmd.Add(typeof(CMDHello).Name, CMDHello.Ins);

            // CMDChangeScene cmdChangeScene = new CMDChangeScene();
            // CMDGeneratePrefab cmdGeneratePrefab = new CMDGeneratePrefab();
            // CMDLogin cmdLogin = new CMDLogin();
            // CMDSyncObject cmdSyncObject = new CMDSyncObject();
            // CMDSyncRequest cmdSyncRequest = new CMDSyncRequest();
            // CMDSyncRequestAllow cmdSyncRequestAllow = new CMDSyncRequestAllow();
            // CMDHello cmdHello = new CMDHello();

            ienum = avaliableCmd.GetEnumerator();
        }

        public string GetHint(string text)
        {
            foreach (var c in avaliableCmd)
            {
                if (c.Value.FORMAT.Contains(text))
                {
                    return c.Value.FORMAT;
                }
            }

            return "";
        }

        public string GetNextHint()
        {
            if (ienum.MoveNext())
            {
                return ienum.Current.Value.FORMAT;
            }
            else
            {
                ienum.Reset();
                return "";
            }
        }

        public void CommandExec(string fromUid, string cmd)
        {
            string title = cmd.Split('|')[0];
            if (avaliableCmd.ContainsKey(title))
            {
                avaliableCmd[title].Recv(fromUid, cmd);
                return;
            }

            CmdManagement.Ins.LogOnScreen("UNKNOWN Recv:" + cmd);
        }
    }
}