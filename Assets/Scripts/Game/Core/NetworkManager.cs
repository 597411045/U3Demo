using System;
using CS.Cmd;
using CS.Log;
using CS.Network;
using UnityEngine;

namespace RPG.Core
{
    public class NetworkManager : MonoBehaviour
    {
        private NetworkManagement nm;

        public void StageRecv()
        {
            if (nm.TryConnect()) return;
            nm.FlushClients();
            nm.CheckInvalidClients();
            nm.TryReceiveExceptPD();
            nm.TryExecuteRecv();
        }

        public void StageSend()
        {
            if (nm.TryConnect()) return;
            nm.DoCustomActions();
            nm.TryExecuteSend();
            nm.TrySendExceptPD();
        }

        private void Awake()
        {
            nm = new NetworkManagement(NTI_type.Client, "81.68.87.60", 7000);
        }

        private void Start()
        {
            TaskPipelineManager.SingleTon.PreActions.Add("NetworkManager.StageRecv", StageRecv);
            TaskPipelineManager.SingleTon.EndActions.Add("NetworkManager.StageSend", StageSend);
        }

        private void OnDestroy()
        {
            nm.CloseAll();
        }
    }
}