using Google.Protobuf;
using CS.Cmd;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CS.Log;

namespace CS.Network
{
    public enum UserState
    {
        Unknown,
        PendingValid,
        Valid,
        PendingDestroy
    }

    public enum UserType
    {
        Server,
        Client,
        Unknown
    }

    public class User
    {
        public string Name { get; set; }
        private UserState state;
        private UserType type;
        public NTICommRecv Recv { get; }
        public NTICommSend Send { get; }

        public User(string _name, Socket s)
        {
            Name = _name;
            Recv = new NTICommRecv(s, this);
            Send = new NTICommSend(s, this);
            state = UserState.Unknown;
            type = UserType.Unknown;
            LogManagement.SingleTon.LogNetContent(this.GetType().Name, "User", Send.GetRemoteEndPoint(), Name,"A New User In");
        }

        //销毁流程-Client
        public void DoDestroy()
        {
            if (IfStateDestroyed()) return;
            state = UserState.PendingDestroy;
            LogManagement.SingleTon.LogNetContent(this.GetType().Name, "DoDestroy", Send.GetRemoteEndPoint(), Name);
            Recv.DoDestroy();
            Send.DoDestroy();
        }

        public bool IfStateDestroyed()
        {
            if (state == UserState.PendingDestroy)
            {
                return true;
            }

            return false;
        }

        public bool IfStateUnknown()
        {
            if (state == UserState.Unknown)
            {
                return true;
            }

            return false;
        }

        public bool IfStateValid()
        {
            if (state == UserState.Valid)
            {
                return true;
            }

            return false;
        }

        public bool IfClient()
        {
            if (type == UserType.Client)
            {
                return true;
            }

            return false;
        }

        public bool IfServer()
        {
            if (type == UserType.Server)
            {
                return true;
            }

            return false;
        }

        public void NewUserAsClient()
        {
            type = UserType.Client;
            state = UserState.Valid;
        }

        public void NewUserAsServer()
        {
            type = UserType.Server;
            state = UserState.Valid;
        }

        public void SetStatePendingValid()
        {
            state = UserState.PendingValid;
        }
    }
}