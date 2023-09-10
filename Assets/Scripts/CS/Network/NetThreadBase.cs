using Google.Protobuf;
using CS.Cmd;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CS.Log;

namespace CS.Network
{
    public enum NTI_type
    {
        Server,
        Client
    }

    public class NetThreadBase //网络多线程基类
    {
        //Thread
        protected Thread thread;
        protected ManualResetEvent manualResetEvent;

        protected Socket socket; //相关Socket
        protected static int bufLength = 256;
        protected Queue<byte> TrueRecvBuffer; //接收的真实msg缓存
        protected Queue<byte[]> CookedSendBuffer; //byte处理过准备发送的msg缓存
        protected Queue<byte[]> CookedRecvBuffer; //byte处理过的准备进行Cmd处理的缓存

        protected NetThreadBase()
        {
            manualResetEvent = new ManualResetEvent(false);
            manualResetEvent.Reset();
        }

        //销毁流程-NetThread
        public void DoDestroy()
        {
            //网络任务销毁流程：
            //1.暂停多线程
            manualResetEvent.Reset();
            if (thread != null)
            {
                //2.中断多线程
                thread.Abort();
            }

            if (socket != null)
            {
                if (socket.Connected)
                {
                    //3.断开Socket连接
                    socket.Disconnect(false);
                }

                //4.回收Socket
                socket.Dispose();
            }
        }

        public void AllowNextFrame()
        {
            manualResetEvent.Set();
        }

        public bool IfNotConnected()
        {
            if (socket != null && thread != null && !socket.Connected)
            {
                return true;
            }
            return false;
        }
        
        public string GetLocalEndPoint()
        {
            if (!IfNotConnected())
            {
                return socket.LocalEndPoint.ToString();
            }

            return "ERROR";
        }
       
        public string GetRemoteEndPoint()
        {
            if (!IfNotConnected())
            {
                return socket.RemoteEndPoint.ToString();
            }
            return "ERROR";
        }

    }
}