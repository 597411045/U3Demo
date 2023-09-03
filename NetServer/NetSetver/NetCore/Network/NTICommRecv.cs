using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetSetver.NetCore.Network
{



    public class NTICommRecv : NetThreadInstance
    {
        Client client;
        bool StartValidContent = false;
        StringBuilder sb;

        public NTICommRecv(Socket s, Client _client)
        {
            socket = s;
            client = _client;
            recvBuf = new Queue<byte>();
            recvList = new Queue<byte[]>();
            sb = new StringBuilder();
            BuildNTICommRecv();
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
                         Console.WriteLine("length:" + length);
                     }
                     catch (Exception e)
                     {
                         Console.WriteLine(e.Message);
                         manualResetEvent.Reset();
                         client.isPendingDestroy = true;
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
                                 recvBuf.Enqueue(b[i]);
                             }
                         }
                         //开始分析头尾
                         int count = recvBuf.Count;
                         for (int j = count; j > 0; j--)
                         {
                             byte oneByte = recvBuf.Dequeue();
                             if (oneByte == 2)
                             {
                                 sb.Clear();
                                 StartValidContent = true;
                             }
                             else if (oneByte == 3)
                             {
                                 StartValidContent = false;
                                 Console.WriteLine("Receive from " + client.name + ":" + sb.ToString());
                                 recvList.Enqueue(Encoding.ASCII.GetBytes(sb.ToString()));
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
                             recvBuf.Enqueue(2);
                             for (int i = 0; i < sb.Length; i++)
                             {
                                 recvBuf.Enqueue((Byte)sb[i]);
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