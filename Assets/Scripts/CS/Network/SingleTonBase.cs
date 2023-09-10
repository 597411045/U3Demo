using System;

namespace CS.Network
{
    public class SingleTonBase<T> where T : SingleTonBase<T>, new()
    {
        protected bool IfInitialed;

        private static T _singleTon;

        public static T SingleTon
        {
            get
            {
                if (_singleTon == null)
                {
                    _singleTon = new T();
                }

                return _singleTon;
            }
        }

        protected private void InitialCheck()
        {
            if (!IfInitialed)
            {
                throw new SingleTonNotInitial();
            }
        }
    }

    public class SingleTonNotInitial : Exception
    {
        public SingleTonNotInitial() : base("单例未执行过初始化")
        {
        }
    }
}