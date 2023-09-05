using System;
using CS.Cmd;
using CS.Log;
using CS.Network;
using UnityEngine;

namespace RPG.Core
{
    public class CmdManager : MonoBehaviour
    {
        private CmdManagement cm;
      
        private void Awake()
        {
            cm = new CmdManagement();
        }
    }
}