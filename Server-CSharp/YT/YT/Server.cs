using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using TKcp;

namespace YT
{
    public class Server
    {
        /// <summary>客户端Peer及状态信息</summary>
        public static Dictionary<uint, Connection> Clients = new Dictionary<uint, Connection>();
        /// <summary>注册的回调函数</summary>
        public Dictionary<string, Action<Connection,byte[]>> Routers = new Dictionary<string, Action<Connection, byte[]>> ();
        /// <summary>待处理的消息队列</summary>
        public Queue<Request> Requests = new Queue<Request>();



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
        private TKcp.Server server;


        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <typeparam name="T">自定义的连接类型</typeparam>
        /// <returns></returns>
        public static Server CreateServer(string ip,int port, string name = "default", int maxClients = 999) {
            Server server = new Server();
            server.Name = name;
            server.ip =ip;
            server.port = port;
            server.maxClients = maxClients;
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
            server = new TKcp.Server();
            server.Ip = ip;
            server.Port = port;
            server.MaxConnection = maxClients;
            //注册回调函数
            server.AddConnectHandle(OnConnect);
            server.AddDisconnectHandle(OnDisconnect);
            server.AddReceiveHandle(OnReceive);
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
                    Console.WriteLine("Receive " + request.Name);
                    if (!Routers.ContainsKey(request.Name)) {
                        Console.WriteLine("此回调未注册");
                        continue;
                    }
                    Routers[request.Name](Clients[request.Conv], request.Msg);

                }
            }
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Stop() {
            Console.WriteLine("[STOP]Server Name: {0}, IP: {1}, Port {2} ", Name, ip, port);
            //释放服务器
            //TODO
        }

        /// <summary>
        /// 为当前服务添加一个路由
        /// </summary>
        public void AddRouter(string name, Action<Connection, byte[]> handle) {
            //如果已经注册了就不用了
            if (Routers.ContainsKey(name)) {
                return;
            }
            Routers.Add(name, handle);
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
        /// 客户端连接时，需要执行的方法
        /// </summary>
        public void OnConnect(byte[] bytes) {
            uint conv = System.BitConverter.ToUInt32(bytes);
            Console.WriteLine("客户端连接 - "+ conv);
            Connection connection = new Connection(conv);
            connection.Server = this;
            if (!Clients.ContainsKey(conv)) {
                Clients.Add(conv, connection);
            }
            Thread thread = new Thread(StartMsgHandle);
            thread.Start();

        }

        /// <summary>
        /// 客户端断开连接时，需要执行的方法
        /// </summary>
        public void OnDisconnect(uint conv) {
            Console.WriteLine("客户端断开连接 - "+ conv);
            
            //Player 下线
            if (Clients.ContainsKey(conv)) {
                Clients.Remove(conv);
            }
        }

        /// <summary>
        /// 客户端接受消息时，需要执行的方法
        /// </summary>
        public void OnReceive(uint conv,byte[] bytes, int length) {
            if (length <= 4) {
                Console.WriteLine("收到了pong");
                return;
            }
            //缓冲区不够，清除，若依旧不够，只能返回
            //当单条协议超过缓冲区长度时会发生
            ByteArray readBuff;
            if (Clients.ContainsKey(conv)) {
                readBuff = Clients[conv].readBuff;
            }
            else {
                Console.WriteLine("此客户端已断开连接");
                return;
            }
            if (readBuff.remain <= 0) {
                OnReceiveData(conv);
                readBuff.MoveBytes();
            }
            if (readBuff.remain <= 0) {
                Console.WriteLine("Receive fail , maybe msg length > buff capacity");
                server.Peers[conv].DisconnectHandle(conv);
                return;
            }
            bytes.CopyTo(readBuff.bytes, readBuff.writeIdx);
            //消息处理
            readBuff.writeIdx += length;
            //处理二进制消息
            OnReceiveData(conv);
            //移动缓冲区
            readBuff.CheckAndMoveBytes();
        }
        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="user"></param>
        private void OnReceiveData(uint conv) {

            ByteArray readBuff;
            if (Clients.ContainsKey(conv)) {
                readBuff = Clients[conv].readBuff;
            }
            else {
                Console.WriteLine("此客户端已断开连接");
                return;
            }

            Request request = MsgHelper.Decode(readBuff, conv);
            Requests.Enqueue(request);
            //继续读取消息
            if (readBuff.length > 2) {
                OnReceiveData(conv);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void Send(uint conv, IMessage msg) {
            byte[] sendBytes = MsgHelper.Encode(msg);
            //发送
            try {
                server.Send(conv,sendBytes);
            }
            catch (Exception ex) {
                Console.WriteLine("Send failed" + ex.ToString());
            }
        }
        /// <summary>
        /// 广播
        /// </summary>
        public void Broadcast(IMessage msg) {
            foreach(Connection connection in Clients.Values) {
                connection.Send(msg);
            }
        }
        public void AddDisconnectHandle(Action<uint> method) {
            server.AddDisconnectHandle(method);
        }
        public void AddConnectHandle(Action<byte[]> method) {
            server.AddConnectHandle(method);
        }
        public void AddReceiveHandle(Action<uint, byte[], int> method) {
            server.AddReceiveHandle(method);
        }

    }

}
