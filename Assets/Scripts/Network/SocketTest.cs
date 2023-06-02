using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class SocketTest : MonoBehaviour
    {
        private Socket _socket;
        private Queue<Socket> tmpClients = new Queue<Socket>();
        private Dictionary<string, Socket> validClientDic = new Dictionary<string, Socket>();
        private bool KeepAccept;

        private Thread ServerAcceptThead;
        private Thread ServerReceiveThead;
        private Thread ServerSendThead;
        private Thread ClientValidThead;

        private int clientCount = 0;


        private void Update()
        {
        }

        public void ServerBuild()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            EndPoint ep = new IPEndPoint(ip, 7000);
            _socket.Bind(ep);
            _socket.Listen(2);
            Debug.Log("Server Listening");

            //AcceptStart();

            //ValidStart();


            // _socket.BeginAccept((asyncResult) =>
            // {
            //     Socket handler = _socket.EndAccept(asyncResult);
            //     Debug.Log("one client join");
            //     handler.BeginSend(bt, 0, bt.Length, SocketFlags.None, (asyncResult) =>
            //     {
            //         int length = handler.EndSend(asyncResult);
            //     }, null);
            //     handler.BeginReceive(bt, 0, bt.Length, SocketFlags.None, (asyncResult) => { }, null);
            // }, null);
            //
            // handler.Send(Encoding.UTF8.GetBytes("Test"));
            //
            // int length = handler.Receive(bt);
            // Debug.Log(Encoding.UTF8.GetString(bt));
        }

        public void ValidStart()
        {
            ClientValidThead = new Thread(ClientValid);
            ClientValidThead.Name = "ClientValidThead";
            ClientValidThead.IsBackground = true;
            ClientValidThead.Start();
        }

        public void AcceptStart()
        {
            KeepAccept = true;
            ServerAcceptThead = new Thread(() =>
            {
                Socket tmpClient;
                while (KeepAccept)
                {
                    Debug.Log(Thread.CurrentThread.Name + "Watiting Accept");
                    tmpClient = _socket.Accept();
                    Debug.Log("A New Client In");

                    lock (tmpClients)
                    {
                        tmpClients.Enqueue(tmpClient);
                    }

                    clientCount++;
                }
            });
            ServerAcceptThead.Name = "ServerAcceptThead";
            ServerAcceptThead.IsBackground = true;
            ServerAcceptThead.Start();
        }

        private void ClientValid()
        {
            Socket tmpClient;
            byte[] bt = new byte[4];
            while (true)
            {
                while (tmpClients.Count > 0)
                {
                    Debug.Log(Thread.CurrentThread.Name + " Checking");

                    lock (tmpClients)
                    {
                        tmpClient = tmpClients.Dequeue();
                    }

                    Debug.Log(Thread.CurrentThread.Name + " Receiving");
                    tmpClient.BeginReceive(bt, 0, bt.Length, SocketFlags.None, ar =>
                    {
                        int length = tmpClient.EndReceive(ar);
                        Debug.Log($"Reveived {length} bytes");
                        if (length != 4)
                        {
                            tmpClient.Disconnect(false);
                            Debug.Log($"InValid Client {clientCount}");
                        }
                        else
                        {
                            lock (
                                validClientDic
                            )
                            {
                                validClientDic.Add((clientCount).ToString(), tmpClient);
                                Debug.Log($"Client {clientCount} Valid");
                            }
                        }
                    }, null);
                }

                Debug.Log(Thread.CurrentThread.Name + " Sleeping");
                Thread.Sleep(5000);
            }
        }


        public void StopAll()
        {
            KeepAccept = false;
            ServerAcceptThead.Abort();
            ClientValidThead.Abort();
        }

        private void OnDestroy()
        {
            StopAll();
        }

        public void ClientConnect()
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Thread clientThread = new Thread(() =>
            {
                client.Connect(IPAddress.Parse("127.0.0.1"), 7000);
                Debug.Log("connect success");
                client.Send(Encoding.UTF8.GetBytes("Test"));
                byte[] bt = new byte[27];
                client.Receive(bt, 0, bt.Length,SocketFlags.None);


            });
            clientThread.IsBackground = true;
            clientThread.Start();
        }
    }
}