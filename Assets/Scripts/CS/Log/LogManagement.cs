using System;
using System.IO;
using System.Text;
using CS.Network;

#if UNITY_EDITOR||UNITY_STANDALONE
using UnityEngine;
#endif

namespace CS.Log
{
    public class LogManagement : SingleTonBase<LogManagement>
    {
        private string path = Directory.GetCurrentDirectory();
        private StreamWriter sw;

        public void Initial(string fileName)
        {
            string fullPath = $"{path}/{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
            sw = new StreamWriter(fullPath, false, Encoding.UTF8);
            Flush("StartLog");

            IfInitialed = true;
        }

        public LogManagement() : base()
        {
        }

        public void Log(string typeName, string funcName, string content = "")
        {
            string log =
                $"<Class>{typeName}</Class><FunctionName>{funcName}</FunctionName><Content>{content}</Content>";
            Flush(log);

#if UNITY_EDITOR||UNITY_STANDALONE
            Debug.Log(content);
#else
            Console.WriteLine($"{DateTime.Now.ToString("u")}:" + log);
#endif
        }

        public void LogOnlyInFile(string typeName, string funcName, string content = "")
        {
            string log =
                $"<Class>{typeName}</Class><FunctionName>{funcName}</FunctionName><Content>{content}</Content>";
            Flush(log);
        }


        public void LogNetContent(string typeName, string funcName, string userEP, string userName,
            string content = "")
        {
            string log =
                $"<Class>{typeName}</Class><FunctionName>{funcName}</FunctionName><UserEP>{userEP}</UserEP><UserName>{userName}</UserName><Content>{content}</Content>";
            Flush(log);

#if UNITY_EDITOR||UNITY_STANDALONE
            Debug.Log(content);
#else
            Console.WriteLine($"{DateTime.Now.ToString("u")}:" + log);
#endif
        }

        public void LogNetContentOnlyInFile(string typeName, string funcName, string userEP, string userName,
            string content)
        {
            string log =
                $"<Class>{typeName}</Class><FunctionName>{funcName}</FunctionName><UserEP>{userEP}</UserEP><UserName>{userName}</UserName><Content>{content}</Content>";
            Flush(log);
        }

        private void Flush(string log)
        {
            lock (sw)
            {
                sw.WriteLine($"{DateTime.Now.ToString("u")}:" + log);
                sw.Flush();
            }
        }

        ~LogManagement()
        {
            sw.Close();
            sw.Dispose();
        }
    }
}