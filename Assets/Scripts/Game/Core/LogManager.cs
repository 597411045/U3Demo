using System;
using CS.Log;
using UnityEngine;

namespace RPG.Core
{
    public class LogManager:MonoBehaviour
    {
        private void Awake()
        {
            LogManagement.SingleTon.Initial("UnityClient");
        }
    }
}