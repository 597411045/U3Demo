using System.Text.RegularExpressions;
using Cmd;
using RPG.Core;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDBase<T> : SingleTon<T>, ICMDAction where T : CMDBase<T>, new()
    {
        public string CmdFormat;
        private static Regex _regex;

        public CMDBase()
        {
            _regex = new Regex(@"<.*?>");
        }


        protected string GetParam(string cmd, int index)
        {
            string str = _regex.Matches(cmd)[index].ToString();
            str = str.Substring(1, str.Length - 2);
            return str;
        }

        protected string ReplaceParam(string[] paras)
        {
            string cmd = CmdFormat;
            for (int i = 0; i < paras.Length; i++)
            {
                cmd = cmd.Replace($"<{GetParam(CmdFormat, i)}>", $"<{paras[i]}>");
            }

            return cmd;
        }

        public virtual void Send(string siid, params string[] paras)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Recv(string siid, string cmd)
        {
            throw new System.NotImplementedException();
        }

        public string FORMAT => CmdFormat;
    }
}