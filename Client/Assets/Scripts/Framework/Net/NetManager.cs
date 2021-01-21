using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class NetManager
{
    //定义ENet客户端
    public static Host client;
    public static Peer peer;
    public static ENet.Event netEvent;
    //接收缓冲区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;
    //是否正在连接
    static bool isConnecting = false;
    //是否正在关闭
    static bool isClosing = false;
    //消息列表
    static List<MsgBase> msgList = new List<MsgBase>();
    //消息列表长度
    static int msgCount = 0;
    //每一次Update处理的消息量
    readonly static int MAX_MESSAGE_FIRE = 10;
    //是否启用心跳
    public static bool isUsePing = true;
    //心跳间隔时间
    public static int pingInterval = 30;
    //上一次发送PING的时间
    static float lastPingTime = 0;
    //上一次收到PONG的时间
    static float lastPongTime = 0;

    //事件
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }
    //事件委托类型
    public delegate void EventListener(String err);
    //事件监听列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="listener"></param>
    public static void AddEventListener(NetEvent netEvent, EventListener listener) {
        //添加事件
        if (eventListeners.ContainsKey(netEvent)) {
            eventListeners[netEvent] += listener;
        }
        //新增事件
        else {
            eventListeners[netEvent] = listener;
        }
    }
    /// <summary>
    /// 删除事件监听
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="listener"></param>
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener) {
        if (eventListeners.ContainsKey(netEvent)) {
            eventListeners[netEvent] -= listener;
        }
    }
    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="err"></param>
    private static void FireEvent(NetEvent netEvent, String err) {
        if (eventListeners.ContainsKey(netEvent)) {
            eventListeners[netEvent](err);
        }
    }


    //消息委托类型
    public delegate void MsgListener(MsgBase msgBase);
    //消息监听列表
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
    /// <summary>
    /// 添加消息监听
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="listener"></param>
    public static void AddMsgListener(string msgName, MsgListener listener) {
        //添加
        if (msgListeners.ContainsKey(msgName)) {
            msgListeners[msgName] += listener;
        }
        //新增
        else {
            msgListeners[msgName] = listener;
        }
    }
    /// <summary>
    /// 删除消息监听
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="listener"></param>
    public static void RemoveMsgListener(string msgName, MsgListener listener) {
        if (msgListeners.ContainsKey(msgName)) {
            msgListeners[msgName] -= listener;
        }
    }
    /// <summary>
    /// 分发消息
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="msgBase"></param>
    private static void FireMsg(string msgName, MsgBase msgBase) {
        if (msgListeners.ContainsKey(msgName)) {
            msgListeners[msgName](msgBase);

        }
    }


    //连接
    public static void Connect(string ip, int port) {

        ENet.Library.Initialize();
        //状态判断
        if (peer.IsSet) {
            Debug.Log("Connect fail, already connected!");
            return;
        }
        if (isConnecting) {
            Debug.Log("Connect fail, isConnecting");
            return;
        }

        client = new Host();
        Address address = new Address();
        address.SetHost(ip);
        address.Port = (ushort)port;
        client.Create();

        peer = client.Connect(address);

        //初始化成员
        InitState();
        //参数设置
        //Connect
        isConnecting = true;
    }

    private static void ENetUpdata() {
        if (client == null) {
            return;
        }
        if (!client.IsSet) {
            return;
        }

        bool polled = false;

        while (!polled) {
            if (client.CheckEvents(out netEvent) <= 0) {
                if (client.Service(15, out netEvent) <= 0)
                    break;

                polled = true;
            }

            switch (netEvent.Type) {
                case ENet.EventType.None:
                    break;

                case ENet.EventType.Connect:
                    OnConnect();
                    break;

                case ENet.EventType.Disconnect:
                    //Console.WriteLine("Client disconnected from server");
                    Close();
                    break;

                case ENet.EventType.Timeout:
                    OnTimeout();
                    break;

                case ENet.EventType.Receive:
                    //Console.WriteLine("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                    OnReceive(netEvent);
                    break;
            }
        }


        client.Flush();
    }

    //初始化状态
    private static void InitState() {

        //接收缓冲区
        readBuff = new ByteArray();
        //写入队列
        writeQueue = new Queue<ByteArray>();
        //是否正在连接
        isConnecting = false;
        //是否正在关闭
        isClosing = false;
        //消息列表
        msgList = new List<MsgBase>();
        //消息列表长度
        msgCount = 0;
        //上一次发送PING的时间
        lastPingTime = Time.time;
        //上一次收到PONG的时间
        lastPongTime = Time.time;
        //监听PONG协议
        if (!msgListeners.ContainsKey("MsgPong")) {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }

    /// <summary>
    /// Connect 回调
    /// </summary>
    private static void OnConnect() {

        Debug.Log("Client connected to server");
        FireEvent(NetEvent.ConnectSucc, "");
        isConnecting = false;

    }
    /// <summary>
    /// 连接超时
    /// </summary>
    private static void OnTimeout() {
        Debug.Log("Client connection timeout");
        FireEvent(NetEvent.ConnectFail, "Client connection timeout");
        isConnecting = false;
    }
    /// <summary>
    /// 关闭连接
    /// </summary>
    public static void Close() {

        //状态判断
        if (!peer.IsSet) {
            return;
        }
        if (isConnecting) {
            return;
        }
        //还有数据在发送
        if (writeQueue.Count > 0) {
            isClosing = true;
        }
        //没有数据在发送
        else {

            FireEvent(NetEvent.Close, "");
        }
        client.Dispose();
        ENet.Library.Deinitialize();

    }

    //发送数据
    public static void Send(MsgBase msg) {
        //状态判断
        if (!peer.IsSet) {
            return;
        }
        if (isConnecting) {
            return;
        }
        if (isClosing) {
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
        //写入队列
        ByteArray ba = new ByteArray(sendBytes);
        lock (writeQueue) {
            writeQueue.Enqueue(ba);
        }
    }

    /// <summary>
    /// 发送发送队列的数据
    /// </summary>
    public static void SendUpdata() {

        if (writeQueue.Count == 0) {
            return;
        }
        //获取写入队列第一条数据            
        ByteArray ba;
        lock (writeQueue) {
            ba = writeQueue.First();
        }

        //继续发送
        if (ba != null) {
            byte channelID = 0;
            Packet packet = default(Packet);
            byte[] data = ba.bytes;
            packet.Create(data);
            peer.Send(channelID, ref packet);
            lock (writeQueue) {
                writeQueue.Dequeue();
            }
        }
        //正在关闭
        else if (isClosing) {
            Close();
        }
    }



    /// <summary>
    /// 接受消息
    /// </summary>
    /// <param name="netEvent"></param>
    public static void OnReceive(ENet.Event netEvent) {
        try {
            //获取接收数据长度
            int count = netEvent.Packet.Length;
            byte[] buffer = new byte[count];
            netEvent.Packet.CopyTo(buffer);
            buffer.CopyTo(readBuff.bytes, readBuff.writeIdx);
            netEvent.Packet.Dispose();

            readBuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData();
            //继续接收数据
            if (readBuff.remain < 8) {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.length * 2);
            }
        }
        catch (Exception ex) {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    /// <summary>
    /// 数据处理
    /// </summary>
    public static void OnReceiveData() {
        //消息长度
        if (readBuff.length <= 2) {
            return;
        }
        //获取消息体长度
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if (readBuff.length < bodyLength)
            return;
        readBuff.readIdx += 2;
        //解析协议名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "") {
            Debug.Log("OnReceiveData MsgBase.DecodeName fail");
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        //添加到消息队列
        lock (msgList) {
            msgList.Add(msgBase);
            msgCount++;
        }
        //继续读取消息
        if (readBuff.length > 2) {
            OnReceiveData();
        }
    }

    //Update
    public static void Update() {
        ENetUpdata();
        SendUpdata();
        MsgUpdate();
        PingUpdate();
    }

    /// <summary>
    /// 更新消息
    /// </summary>
    public static void MsgUpdate() {
        //初步判断，提升效率
        if (msgCount == 0) {
            return;
        }
        //重复处理消息
        for (int i = 0; i < MAX_MESSAGE_FIRE; i++) {
            //获取第一条消息
            MsgBase msgBase = null;
            lock (msgList) {
                if (msgList.Count > 0) {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            //分发消息
            if (msgBase != null) {
                FireMsg(msgBase.protoName, msgBase);
            }
            //没有消息了
            else {
                break;
            }
        }
    }

    //发送PING协议
    private static void PingUpdate() {
        //是否启用
        if (!isUsePing) {
            return;
        }
        //发送PING
        if (Time.time - lastPingTime > pingInterval) {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }
        //检测PONG时间
        if (Time.time - lastPongTime > pingInterval * 4) {
            Close();
        }
    }

    //监听PONG协议
    private static void OnMsgPong(MsgBase msgBase) {
        lastPongTime = Time.time;
    }
}