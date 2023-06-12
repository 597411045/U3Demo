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
        private IEnumerator<KeyValuePair<string, CMDBase>> ienum;

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
            CMDGeneratePrefab cmdGeneratePrefab = new CMDGeneratePrefab();
            CMDLogin cmdLogin = new CMDLogin();
            CMDSyncObject cmdSyncObject = new CMDSyncObject();
            CMDSyncRequest cmdSyncRequest = new CMDSyncRequest();
            CMDSyncRequestAllow cmdSyncRequestAllow = new CMDSyncRequestAllow();
            CMDHello cmdHello = new CMDHello();

            ienum = avaliableCmd.GetEnumerator();
        }

        public void RegisterCmd(string name, CMDBase p)
        {
            avaliableCmd.Add(name, p);
        }

        public string GetHint(string text)
        {
            foreach (var c in avaliableCmd)
            {
                if (c.Value.CmdFormat.Contains(text))
                {
                    return c.Value.CmdFormat;
                }
            }

            return "";
        }

        public string GetNextHint()
        {
            if (ienum.MoveNext())
            {
                return ienum.Current.Value.CmdFormat;
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