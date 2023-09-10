using System;
using CS.Cmd;
using CS.Log;
using CS.Network;
using ProtoMsg;
using UnityEngine;

namespace RPG.Core
{
    public class CmdManager : MonoBehaviour
    {
        private void Awake()
        {
        }

        public static void SendGuestCmdLogin()
        {
            Debug.Log("SendGuestCmdLogin");

            LoginRequest request = new LoginRequest() { Username = "Guest", Password = "Guest" };
            User c = NetworkManagement.SingleTon.GetUserByName("MainServer");
            if (c == null) return;
            Cmd_Login cmd = new Cmd_Login(NetworkManagement.SingleTon.GetUserByName("MainServer"), request);
            CmdManagement.SingleTon.AddNewRequestCmdInCookedDicAndSend(cmd);
        }
    }
}