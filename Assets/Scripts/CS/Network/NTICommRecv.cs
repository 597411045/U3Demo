using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CS.Log;

namespace CS.Network
{
    public class NTICommRecv : NetThreadBase
    {
        User _user;
        bool StartValidContent = false;
        StringBuilder sb;

        public NTICommRecv(Socket s, User user)
        {
            socket = s;
            _user = user;
            TrueRecvBuffer = new Queue<byte>();
            CookedRecvBuffer = new Queue<byte[]>();
            sb = new StringBuilder();
            BuildNTICommRecv();
        }

        public int GetCount()
        {
            return CookedRecvBuffer.Count;
        }

        public byte[] GetMsg()
        {
            return CookedRecvBuffer.Dequeue();
        }

        public void AddMsg(byte[] msg)
        {
            CookedRecvBuffer.Enqueue(msg);
        }


        public void BuildNTICommRecv()
        {
            thread = new Thread(() =>
            {
                while (true)
                {
                    this.manualResetEvent.WaitOne();

                    int length = 0;
                    byte[] b = new byte[bufLength];
                    try
                    {
                        length = socket.Receive(b, 0, b.Length, SocketFlags.None);
                    }
                    catch (Exception e)
                    {
                        //销毁流程-Client-触发

                        LogManagement.SingleTon.LogNetContent(this.GetType().Name, "Thread",
                            _user.Send.GetRemoteEndPoint(), _user.Name, e.Message);
                        manualResetEvent.Reset();
                        _user.DoDestroy();
                        return;
                    }

                    if (length != 0)
                    {
                        //除0以外全部存入queue
                        for (int i = 0; i < b.Length; i++)
                        {
                            if (b[i] == 0)
                            {
                                continue;
                            }
                            else
                            {
                                TrueRecvBuffer.Enqueue(b[i]);
                            }
                        }

                        //开始分析头尾
                        int count = TrueRecvBuffer.Count;
                        for (int j = count; j > 0; j--)
                        {
                            byte oneByte = TrueRecvBuffer.Dequeue();
                            if (oneByte == 2)
                            {
                                sb.Clear();
                                StartValidContent = true;
                            }
                            else if (oneByte == 3)
                            {
                                StartValidContent = false;
                                string str = sb.ToString();
                                LogManagement.SingleTon.LogNetContentOnlyInFile(this.GetType().Name, "Thread",
                                    _user.Send.GetRemoteEndPoint(), _user.Name, sb.ToString());
                                CookedRecvBuffer.Enqueue(Encoding.ASCII.GetBytes(sb.ToString()));
                            }
                            else
                            {
                                if (StartValidContent)
                                {
                                    sb.Append((Char)oneByte);
                                }
                            }
                        }

                        //断截的msg存回queue
                        if (sb.Length > 0)
                        {
                            TrueRecvBuffer.Enqueue(2);
                            for (int i = 0; i < sb.Length; i++)
                            {
                                TrueRecvBuffer.Enqueue((Byte)sb[i]);
                            }
                        }
                    }

                    manualResetEvent.Reset();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }
}