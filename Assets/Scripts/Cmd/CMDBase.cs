using System.Text.RegularExpressions;
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
            _regex = new Regex(@"<\w*>");
            _regex2 = new Regex(@"\w*");
        }

        public virtual void Send(string siid, params string[] para)
        {
        }

        public virtual void Recv(string cmd)
        {
        }

        protected string GetParam(string cmd,int index)
        {
            return _regex2.Match(_regex.Matches(cmd)[index].ToString()).ToString();
        }
    }
}