using System;
using CS.Cmd;
using CS.Log;
using CS.Network;
using UnityEngine;

namespace RPG.Core
{
    public class NetworkManager : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void StageRecv()
        {
            if (NetworkManagement.SingleTon.TryConnect()) return;
            NetworkManagement.SingleTon.FlushUserBUffer();
            NetworkManagement.SingleTon.DeleteDestroyedClient();
            NetworkManagement.SingleTon.TryReceiveMsg();
            NetworkManagement.SingleTon.PassMsgToCmdSys();
        }

        public void StageSend()
        {
            if (NetworkManagement.SingleTon.TryConnect()) return;
            NetworkManagement.SingleTon.DoExtraActions();
            CmdManagement.SingleTon.Process();
            NetworkManagement.SingleTon.TrySendMsg();
        }


        private void Start()
        {
            NetworkManagement.SingleTon.Initial(NTI_type.Client, "81.68.87.60", 7000);
            TaskPipelineManager.SingleTon.PreActions.Add("NetworkManager.StageRecv", StageRecv);
            TaskPipelineManager.SingleTon.EndActions.Add("NetworkManager.StageSend", StageSend);
        }

        private void OnDestroy()
        {
            NetworkManagement.SingleTon.CloseAllUser();
        }
    }
}