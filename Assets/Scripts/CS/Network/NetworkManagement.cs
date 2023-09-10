using Google.Protobuf;
using CS.Cmd;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CS.Log;

namespace CS.Network
{
    public class NetworkManagement : SingleTonBase<NetworkManagement>
    {
        NTI_type type; //记录类型

        private NTIAccept nTIAccept; //默认有一个Accept任务
        private NTIConnect nTIConnect; //默认有一个Connect任务
        private List<User> CookedUserList; //已经连接正常的Client集合
        private Queue<User> UserBuffer; //已连接，但是还未进入集合的Client缓存

        //已弃用，标识001：应由Cmd管理
        //private Queue<CmdBase> CmdBuffer;
        //public void AddCmd(CmdBase cmd)
        //{
        //    CmdBuffer.Enqueue(cmd);
        //}


        private Queue<Action> ExtraActions; //由外部传入的委托，一般用于接收后，发送前

        //一般由Server Accept调用
        public void AddClientInBuffer(Socket s, string name)
        {
            InitialCheck();
            //传入一个临时的Client
            User tmpUser = new User(name, s);
            //临时Client进入Client缓存
            UserBuffer.Enqueue(tmpUser);
        }


        //单例new需要
        public NetworkManagement()
        {
        }


        //初始化
        public void Initial(NTI_type _type, string ip = "127.0.0.1", int port = 7000)
        {
            type = _type;
            CookedUserList = new List<User>();
            UserBuffer = new Queue<User>();
            ExtraActions = new Queue<Action>();

            if (type == NTI_type.Server)
            {
                //new accept任务时会自动开启线程
                nTIAccept = new NTIAccept(ip, port);
            }

            if (type == NTI_type.Client)
            {
                //new accept任务时会自动开启线程
                nTIConnect = new NTIConnect(ip, port);
            }


            //已弃用，标识001
            //CmdBuffer = new Queue<CmdBase>();

            IfInitialed = true; //初始化标记
        }

        //销毁流程-Client-执行
        public void DeleteDestroyedClient()
        {
            InitialCheck();

            for (int i = CookedUserList.Count - 1; i >= 0; i--)
            {
                if (CookedUserList[i].IfStateDestroyed())
                {
                    //Client的销毁由收发异常、Cmd或者其他主动销毁，这里只要移除被执行过销毁的Client即可
                    //移除队列
                    CookedUserList.RemoveAt(i);
                }
            }
        }

        public bool TryConnect()
        {
            InitialCheck();
            if (nTIConnect.IfNotConnected())
            {
                nTIConnect.AllowNextFrame();
                return true;
            }

            return false;
        }

        public void FlushUserBUffer()
        {
            InitialCheck();
            int count = UserBuffer.Count();
            for (int i = count; i > 0; i--)
            {
                CookedUserList.Add(UserBuffer.Dequeue());
            }
        }

        public void TryReceiveMsg()
        {
            InitialCheck();

            foreach (var i in CookedUserList)
            {
                //多线程异常可能随时发生，要随时检测Client是否健康
                if (!i.IfStateDestroyed())
                {
                    i.Recv.AllowNextFrame();
                }
            }
        }

        public void PassMsgToCmdSys()
        {
            InitialCheck();

            //对接收对cmd做处理
            foreach (var i in CookedUserList)
            {
                if (!i.IfStateDestroyed())
                {
                    //处理过程中可能多线程又添加了新的msg，所以只处理开始运行前的个数
                    int count = i.Recv.GetCount();
                    for (int j = count; j > 0; j--)
                    {
                        //Cmd支持传入msg和client自动生成cmd,流程上建议收到后立即处理一次recv request
                        CmdManagement.SingleTon.PassRecvMsgToCmdAndExec(Encoding.ASCII.GetString(i.Recv.GetMsg()), i);
                    }
                }
            }
        }

        //已弃用，标识001
        //public void TryExecuteSend()
        // {
        //     //对预备发送的cmd做处理
        //     int c2 = CmdBuffer.Count;
        //     for (int i = c2; i > 0; i--)
        //     {
        //         CmdBuffer.Dequeue().JoinCmdExecDic();
        //     }
        //
        //     //Cmd综合处理
        //     CmdManagement.SingleTon.Process();
        // }

        public void DoExtraActions()
        {
            InitialCheck();
            int count = ExtraActions.Count;
            for (int i = count; i > 0; i--)
            {
                ExtraActions.Dequeue().Invoke();
            }
        }

        public void TrySendMsg()
        {
            InitialCheck();
            foreach (var i in CookedUserList)
            {
                if (!i.IfStateDestroyed())
                {
                    i.Send.AllowNextFrame();
                }
            }
        }

        //主动调用关闭所有Client
        public void CloseAllUser()
        {
            InitialCheck();
            LogManagement.SingleTon.Log(this.GetType().Name, "CloseAllUser");
            foreach (var i in CookedUserList)
            {
                //销毁流程-Client-触发
                i.DoDestroy();
            }

            DeleteDestroyedClient();
        }

        public User GetUserByName(string name)
        {
            return CookedUserList.Find((a) => { return a.Name == name; });
        }


        //全自动流程入口
        public void AutoProcess()
        {
            InitialCheck();

            if (type == NTI_type.Client)
            {
                ClientAutoProcess();
            }

            if (type == NTI_type.Server)
            {
                ServerAutoProcess();
            }
        }

        private void ServerAutoProcess()
        {
            InitialCheck();
            //服务器自动流程：
            //1，允许执行一次accept，接入一个Client
            nTIAccept.AllowNextFrame();
            //2.缓存的临时Client加入Client集合
            FlushUserBUffer();
            //3.检测上一帧过后，是否有无效的Client
            DeleteDestroyedClient();
            //4.对每个Client尝试接收数据
            TryReceiveMsg();
            //5.对每个Client接收的数据转交给Cmd系统，并执行
            PassMsgToCmdSys();
            //6.如果有其他操作可能要加入CmdList，注册后在这里执行
            ExtraActions.Enqueue(ServerAction);
            DoExtraActions();

            //让所有cmd在这一帧执行一轮
            CmdManagement.SingleTon.Process();
            //已弃用，标识001
            //TryExecuteSend();
            //7.发送Msg
            TrySendMsg();
        }

        private void ClientAutoProcess()
        {
            InitialCheck();
            //Client流程
            //特殊一点在于判断是否已和server连接，已连接则处理逻辑，否则尝试连接
            if (TryConnect())
            {
            }
            else
            {
                FlushUserBUffer();
                DeleteDestroyedClient();
                TryReceiveMsg();
                PassMsgToCmdSys();
                ExtraActions.Enqueue(ClientAction);
                DoExtraActions();
                //让所有cmd在这一帧执行一轮
                CmdManagement.SingleTon.Process();
                TrySendMsg();
            }
        }

        private void ServerAction()
        {
            //server 特有逻辑，对未知的用户去验证
            foreach (var i in CookedUserList)
            {
                if (i.IfStateUnknown())
                {
                    //Cmd也支持预先生成Cmd，并直接加入Cmd集合
                    Cmd_WhoAreYou cmd = new Cmd_WhoAreYou(i, new WhoAreYouRequest() { ServerName = "MainServer" });
                    //已弃用，标识001
                    //AddCmd(cmd);
                    CmdManagement.SingleTon.AddNewRequestCmdInCookedDicAndSend(cmd);
                }
            }
        }

        public void ClientAction()
        {
            InitialCheck();
            //Client特殊逻辑
            //对已验证的服务器发送同步消息
            foreach (var i in CookedUserList)
            {
                if (i.IfStateValid() && i.IfServer())
                {
                    TransformSyncRequest request = new TransformSyncRequest()
                    {
                        PositionX = 100,
                        PositionY = 100,
                        PositionZ = 100,
                        RotationX = 0,
                        RotationY = 0,
                        RotationZ = 0,
                        GameObjectName = CookedUserList[0].Send.GetLocalEndPoint()
                    };
                    string msg = request.GetType().Name + "|" + request.ToString();
                    Cmd_BroadCast cmd = new Cmd_BroadCast(CookedUserList[0], new BroadCastRequest() { Msg = msg });
                    //已弃用，标识001
                    //AddCmd(cmd);
                    CmdManagement.SingleTon.AddNewRequestCmdInCookedDicAndSend(cmd);
                }
            }
        }

        private List<User> tmpValidClients;

        public IEnumerable<User> GetValidClients(UserType client)
        {
            if (tmpValidClients == null)
            {
                tmpValidClients = new List<User>();
            }
            else
            {
                tmpValidClients.Clear();
            }

            foreach (var i in CookedUserList)
            {
                if (!i.IfStateDestroyed() && i.IfStateValid() && i.IfClient())
                {
                    tmpValidClients.Add(i);
                }
            }

            return tmpValidClients;
        }
    }
}