using System;
using UnityEngine;

namespace RPG.Core
{
    public class SingleTonWithMono<T> : MonoBehaviour where T : SingleTonWithMono<T>
    {
        public static T Ins
        {
            get { return _instance; }
        }

        private static T _instance;

        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                Debug.LogError(((T)this).GetType().Name + " Init On Awake");
            }
            else
            {
                Debug.LogError(((T)this).GetType().Name + " Already Exist, Be Care On Ins");
            }
        }
    }
}