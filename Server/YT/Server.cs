﻿using ENet;
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
        public Dictionary<Peer, ClientState> Clients = new Dictionary<Peer, ClientState>();
        /// <summary>注册的回调函数</summary>
        public Dictionary<string, MethodInfo> Routers;
        /// <summary>待处理的消息队列</summary>
        public Queue<Request> Requests;
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
            Console.WriteLine("[START]Server Name: {0}, IP: {1}, Port {2} ", Name, ip, this.port);
            //启动ENet
            Library.Initialize();
            using (server = new Host()) {
                Address address = new Address();
                address.SetIP(ip);
                address.Port = (ushort)port;
                server.Create(address, maxClients);
                Event netEvent;
                Thread FireMsgHandle = new Thread(StartMsgHandle);
                FireMsgHandle.Start();
                while (!Console.KeyAvailable) {
                    bool polled = false;

                    while (!polled) {
                        if (server.CheckEvents(out netEvent) <= 0) {
                            if (server.Service(15, out netEvent) <= 0)
                                break;

                            polled = true;
                        }
                        switch (netEvent.Type) {
                            case EventType.None:
                                break;
                            //当客户端连接
                            case EventType.Connect:
                                //加入列表
                                ClientState cs = new ClientState();
                                cs.Peer = netEvent.Peer;
                                cs.server = this;
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
                }
                server.Flush();
            }
        }

        /// <summary>
        /// 处理消息回调的线程
        /// </summary>
        private void StartMsgHandle() {
            Request request;
            while (Requests.Count>0) {
                request = Requests.Dequeue();
                //分发消息
                object[] o = { request.Peer, request.Msg };
                Console.WriteLine("Receive " + request.Name);
                if (Routers.ContainsKey(request.Name)) {
                    Routers[request.Name].Invoke(null, o);
                }
                else {
                    Console.WriteLine("Router 未注册 " + request.Name);
                }
            }
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Stop() {
            Console.WriteLine("[STOP]Server Name: {0}, IP: {1}, Port {2} ", Name, ip, port);
            //释放服务器
            server.Dispose();

            Library.Deinitialize();
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
                return;
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

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }
}
