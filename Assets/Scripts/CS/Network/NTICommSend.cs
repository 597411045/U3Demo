using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CS.Log;

namespace CS.Network
{
    public class NTICommSend : NetThreadInstance
    {
        Client client;

        public NTICommSend(Socket s, Client _client)
        {
            socket = s;
            client = _client;
            sendList = new Queue<byte[]>();

            BuildNTICommSend();
        }

        public void BuildNTICommSend()
        {
            thread = new Thread(() =>
            {
                while (true)
                {
                    this.manualResetEvent.WaitOne();

                    int count = sendList.Count;
                    for (int i = count; i > 0; i--)
                    {
                        byte[] tmp2 = sendList.Dequeue();
                        byte[] tmp = new byte[tmp2.Length + 2];
                        tmp[0] = 2;
                        tmp[tmp.Length - 1] = 3;
                        tmp2.CopyTo(tmp, 1);
                        try
                        {
                            socket.Send(tmp, 0, tmp.Length, SocketFlags.None);
                            LogManagement.Log("Send to " + client.name + ":" + Encoding.ASCII.GetString(tmp));
                        }
                        catch (Exception e)
                        {
                            LogManagement.Log(e.Message);
                            manualResetEvent.Reset();
                            client.state = ClientState.PendingDestroy;
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