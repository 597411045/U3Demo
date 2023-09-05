using System;
using CS.Cmd;
using CS.Log;
using CS.Network;
using UnityEngine;

namespace RPG.Core
{
    public class LogManager : MonoBehaviour
    {
        private LogManagement lm;
     
        private void Awake()
        {
            lm = new LogManagement("UnityClient");
        }
    }
}