using System;
using System.IO;
using System.Text;

#if UNITY_EDITOR||UNITY_STANDALONE
    using UnityEngine;
#endif

namespace CS.Log
{
    public class LogManagement
    {
        private static LogManagement SingleTon;
        private string path = Directory.GetCurrentDirectory();
        private static StreamWriter sw;

        public LogManagement(string fileName)
        {
            if (SingleTon == null)
            {
                SingleTon = this;
                string fullPath = $"{path}/{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
                sw = new StreamWriter(fullPath, false, Encoding.UTF8);
                sw.WriteLine("StartLog");
                sw.Flush();
            }
        }

        public static void Log(string str)
        {
            sw.WriteLine(str);
            sw.Flush();

#if UNITY_EDITOR||UNITY_STANDALONE
            Debug.Log(str);
#else
            Console.WriteLine(str);
#endif
        }

        ~LogManagement()
        {
            sw.Close();
            sw.Dispose();
        }
    }
}