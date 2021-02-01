using ENet;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace YT
{
    public class Server
    {
        
        /// <summary>客户端Peer及状态信息</summary>
        public Dictionary<Peer, Connection> Clients = new Dictionary<Peer, Connection>();
        /// <summary>注册的回调函数</summary>
        public Dictionary<string, MethodInfo> Routers = new Dictionary<string, MethodInfo>();
        /// <summary>待处理的消息队列</summary>
        public Queue<Request> Requests = new Queue<Request>();
        /// <summary>ping 间隔</summary>
        public long pingInterval = 30;


        /// <summary>服务端名</summary>
        public string Name = "default";
        /// <summary>服务端IP地址</summary>
        private string ip = "127.0.0.1";
        /// <summary>服务端端口号</summary>
        private int port = 8888;
        /// <summary>最大连接数</summary>
        private int maxClients = 999;
        /// <summary>服务器是否被创建</summary>
        private bool isCreate = false;
        /// <summary>服务器</summary>
        private Host server;


        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <typeparam name="T">自定义的连接类型</typeparam>
        /// <returns></returns>
        public static Server CreateServer() {
            Server server = new Server();
            NetConfig netConfig = ConfigHelper.GetNetConfig();
            server.Name = netConfig.Name;
            server.ip = netConfig.IP;
            server.port = netConfig.Port;
            server.maxClients = netConfig.MaxClients;
            server.isCreate = true;
            return server;
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        public void Start() {
            if (isCreate == false) {
                return;
            }
            Console.WriteLine("[START]Server Name: {0}, IP: {1}, Port {2} ", Name, ip, port);
            using (server = new Host()) {
                //启动ENet
                Library.Initialize();
                Address address = new Address();
                address.SetIP(ip);
                address.Port = (ushort)port;
                server.Create(address, maxClients);
                //server.InitializeServer(port, maxClients);
                Event netEvent;
                Thread FireMsgHandle = new Thread(StartMsgHandle);
                FireMsgHandle.Start();
                while (!Console.KeyAvailable) {
                    if (server.Service(15000, out netEvent)>0) {

                        do {
                            switch (netEvent.Type) {
                                case EventType.None:
                                    break;
                                //当客户端连接
                                case EventType.Connect:
                                    //加入列表
                                    Connection cs = new Connection();
                                    cs.Peer = netEvent.Peer;
                                    cs.server = this;
                                    cs.ChannelID = netEvent.ChannelID;
                                    cs.OnConnect();
                                    break;
                                //当客户端断开连接
                                case EventType.Disconnect:
                                    Clients[netEvent.Peer].OnDisconnect();
                                    break;
                                //当超时
                                case EventType.Timeout:
                                    Clients[netEvent.Peer].OnTimer();
                                    break;
                                //当接收到来自客户端的消息
                                case EventType.Receive:
                                    
                                    Clients[netEvent.Peer].OnReceive(netEvent);
                                    break;
                            }
                        }
                        while (server.CheckEvents(out netEvent)>0);
                    }



                }
            }
            server.Flush();

        }

        /// <summary>
        /// 处理消息回调的线程
        /// </summary>
        private void StartMsgHandle() {

            Request request;

            while (true) {
                while (Requests.Count != 0) {
                    request = Requests.Dequeue();
                    //分发消息

                    object[] o = { Clients[request.Peer], request.Msg };
                    Console.WriteLine("Receive " + request.Name);
                    if (!Routers.ContainsKey(request.Name)) {
                        AddRouter(request.Name);
                    }
                    Routers[request.Name].Invoke(null, o);

                }
            }
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Stop() {
            Console.WriteLine("[STOP]Server Name: {0}, IP: {1}, Port {2} ", Name, ip, port);
            //释放服务器
            Library.Deinitialize();
            server.Dispose();
        }

        /// <summary>
        /// 为当前服务添加一个路由
        /// </summary>
        public void AddRouter(string name) {
            //如果已经注册了就不用了
            if (Routers.ContainsKey(name)) {
                return;
            }
            MethodInfo mi = typeof(MsgHandler).GetMethod(name);
            //如果得到的方法为空，则不加入
            if (mi == null) {
                throw new Exception("无此回调函数!");
            }
            Routers.Add(name, mi);
        }
        /// <summary>
        /// 为当前服务移除一个路由
        /// </summary>
        public void RemoveRouter(string name) {
            if (!Routers.ContainsKey(name)) {
                return;
            }
            Routers.Remove(name);
        }
    }
}
