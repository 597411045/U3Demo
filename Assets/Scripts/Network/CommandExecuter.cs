using System.Text;
using UnityEngine;

namespace Network
{
    public class CommandExecuter
    {
        public static void CommandExec(S1Server s, string str)
        {
            if (str.Equals("Player Login"))
            {
                s.GeneratePlayer();
            }
        }


        public static void SendLogin(NetworkCenter nc)
        {
            nc.SendMessageBySocketUID("ClientMainSocket", Encoding.UTF8.GetBytes("Player Login"));
            Debug.LogError("SendLogin");
        }

        public static void RecvLogin(NetworkCenter nc)
        {
            nc.GetMessageBySocketUID("tmpSocket1");
        }

        public static void PositionSync()
        {
        }

        public static void LoginOut()
        {
        }
    }
}