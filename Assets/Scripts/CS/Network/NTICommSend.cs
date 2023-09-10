using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CS.Log;

namespace CS.Network
{
    public class NTICommSend : NetThreadBase
    {
        User _user;

        public NTICommSend(Socket s, User user)
        {
            socket = s;
            _user = user;
            CookedSendBuffer = new Queue<byte[]>();

            BuildNTICommSend();
        }

        public int GetCount()
        {
            return CookedSendBuffer.Count;
        }

        public byte[] GetMsg()
        {
            return CookedSendBuffer.Dequeue();
        }

        public void AddMsg(byte[] msg)
        {
            CookedSendBuffer.Enqueue(msg);
        }

        public void BuildNTICommSend()
        {
            thread = new Thread(() =>
            {
                while (true)
                {
                    this.manualResetEvent.WaitOne();

                    int count = CookedSendBuffer.Count;
                    for (int i = count; i > 0; i--)
                    {
                        byte[] tmp2 = CookedSendBuffer.Dequeue();
                        byte[] tmp = new byte[tmp2.Length + 2];
                        tmp[0] = 2;
                        tmp[tmp.Length - 1] = 3;
                        tmp2.CopyTo(tmp, 1);
                        try
                        {
                            socket.Send(tmp, 0, tmp.Length, SocketFlags.None);
                            string str = Encoding.ASCII.GetString(tmp);
                            LogManagement.SingleTon.LogNetContentOnlyInFile(this.GetType().Name, "Thread",
                                _user.Send.GetRemoteEndPoint(), _user.Name, Encoding.ASCII.GetString(tmp));
                        }
                        catch (Exception e)
                        {
                            LogManagement.SingleTon.LogNetContent(this.GetType().Name, "Thread",
                                _user.Send.GetRemoteEndPoint(), _user.Name, e.Message);
                            manualResetEvent.Reset();
                            _user.DoDestroy();
                            return;
                        }
                    }

                    manualResetEvent.Reset();
                }
            });
            thread.Start();
        }
    }
}