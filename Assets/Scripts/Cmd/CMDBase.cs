using System.Text.RegularExpressions;
using RPG.UI;
using UnityEngine;

namespace RPG.Cmd
{
    public class CMDBase
    {
        public string CmdFormat;
        private Regex _regex;
        private Regex _regex2;

        public CMDBase()
        {
            _regex = new Regex(@"<.*?>");
        }

        public virtual void Send(string siid, params string[] paras)
        {
        }

        public virtual void Recv(string siid, string cmd)
        {
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
    }
}