using System;
using UnityEngine;

namespace RPG.Core
{
    public class SingleTon<T> : MonoBehaviour where T : new()
    {
        public static T Ins
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }

        private static T _instance;
    }
}