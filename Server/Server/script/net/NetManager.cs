using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Reflection;
using ENet;
using System.Linq;

class NetManager
{

    //客户端Peer及状态信息
    public static Dictionary<Peer, ClientState> clients = new Dictionary<Peer, ClientState>();

    //ping 间隔
    public static long pingInterval = 30;

    public static void StartLoop(int listenPort) {
        //启动ENet
        ENet.Library.Initialize();

        ushort port = Const.Port;
        int maxClients = 500;
        using (Host server = new Host()) {
            Address address = new Address();
            address.Port = port;
            server.Create(address, maxClients);
            Console.WriteLine("[服务器]启动成功");
            
            Event netEvent;
            
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

                        case EventType.Connect:
                            OnConnect(netEvent.Peer);
                            break;

                        case EventType.Disconnect:
            
                            Close(clients[netEvent.Peer]);
                            break;

                        case EventType.Timeout:
                            Console.WriteLine("Client timeout - ID: " + netEvent.Peer.ID + ", IP: " + netEvent.Peer.IP);
                            Timer();
                            break;

                        case EventType.Receive:
                            
                            OnReceive(netEvent);
                            
                            break;
                    }
                }
            }

            server.Flush();
        }
    }


    /// <summary>
    /// 客户端连接
    /// </summary>
    /// <param name="clientPeer"></param>
    public static void OnConnect(Peer clientPeer) {
        try {
            //加入列表
            ClientState state = new ClientState();
            state.peer = clientPeer;
            state.lastPingTime = GetTimeStamp();
            clients.Add(clientPeer, state);
            Console.WriteLine("Client connected - ID: " + clientPeer.ID + ", IP: " + clientPeer.IP);
        }
        catch (Exception ex) {
            Console.WriteLine("Accept 失败" + ex.ToString());
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    /// <param name="state"></param>
    public static void Close(ClientState state) {
        Console.WriteLine("Client disconnected - ID: " + state.peer.ID + ", IP: " + state.peer.IP);
        //消息分发
        MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
        object[] ob = { state };
        mei.Invoke(null, ob);
        //关闭
        clients.Remove(state.peer);

    }


    /// <summary>
    /// 接收客户端信息
    /// </summary>
    /// <param name="clientfd"></param>
    public static void OnReceive(Event netEvent) {

        ClientState state = clients[netEvent.Peer];
        ByteArray readBuff = state.readBuff;
        //接收
        int count = 0;
        //缓冲区不够，清除，若依旧不够，只能返回
        //当单条协议超过缓冲区长度时会发生
        if (readBuff.remain <= 0) {
            OnReceiveData(state);
            readBuff.MoveBytes();
        };
        if (readBuff.remain <= 0) {
            Console.WriteLine("Receive fail , maybe msg length > buff capacity");
            Close(state);
            return;
        }
        try {
            
            count = netEvent.Packet.Length;
            byte[] buffer = new byte[count];
            netEvent.Packet.CopyTo(buffer);
            buffer.CopyTo(readBuff.bytes, readBuff.writeIdx);
            netEvent.Packet.Dispose();
        }
        catch (Exception ex) {
            Console.WriteLine("Receive Exception " + ex.ToString());
            Close(state);
            return;
        }
        //客户端关闭
        if (count <= 0) {
            Close(state);
            return;
        }
        //消息处理
        readBuff.writeIdx += count;
        //处理二进制消息
        OnReceiveData(state);
        //移动缓冲区
        readBuff.CheckAndMoveBytes();
    }


    /// <summary>
    /// 数据处理
    /// </summary>
    /// <param name="state"></param>
    public static void OnReceiveData(ClientState state) {
        ByteArray readBuff = state.readBuff;
        //消息长度
        if (readBuff.length <= 2) {
            return;
        }
        //消息体长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if (readBuff.length < bodyLength) {
            return;
        }
        readBuff.readIdx += 2;
        //解析协议名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "") {
            Console.WriteLine("OnReceiveData MsgBase.DecodeName fail");
            Close(state);
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        if (bodyCount < 0) {
            Console.WriteLine("OnReceiveData fail, bodyCount <0 ");
            Close(state);
            return;
        }
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        //分发消息
        MethodInfo mi = typeof(MsgHandler).GetMethod(protoName);
        object[] o = { state, msgBase };
        Console.WriteLine("Receive " + protoName);
        if (mi != null) {
            mi.Invoke(null, o);
        }
        else {
            Console.WriteLine("OnReceiveData Invoke fail " + protoName);
        }
        //继续读取消息
        if (readBuff.length > 2) {
            OnReceiveData(state);
        }
    }




    //发送
    public static void Send(ClientState cs, MsgBase msg) {
        //状态判断
        if (cs == null) {
            return;
        }
        //数据编码
        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[2 + len];
        //组装长度
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        //组装名字
        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        //组装消息体
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
        //发送
        try {
            byte channelID = 0;
            Packet packet = default(Packet);
            byte[] data = sendBytes;
            packet.Create(data);
            cs.peer.Send(channelID, ref packet);
            
            
        }
        catch (Exception ex) {
            Console.WriteLine("Send failed" + ex.ToString());
        }

    }

    /// <summary>
    /// 定时器
    /// </summary>
    static void Timer() {
        //消息分发
        MethodInfo mei = typeof(EventHandler).GetMethod("OnTimer");
        object[] ob = { };
        mei.Invoke(null, ob);
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