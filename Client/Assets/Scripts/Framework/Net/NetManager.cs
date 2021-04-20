﻿using Gate;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TKcp;
using UnityEngine;
public static class NetManager
{
    //定义客户端
    public static Client client;

    //接收缓冲区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;
    //是否正在连接
    static bool isConnecting = false;
    //是否正在关闭
    static bool isClosing = false;
    //消息列表
    static List<Request> msgList = new List<Request>();
    //消息列表长度
    static int msgCount = 0;
    //每一次Update处理的消息量
    readonly static int MAX_MESSAGE_FIRE = 10;

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
    public delegate void MsgListener(Request request);
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
    /// <param name="request"></param>
    private static void FireMsg(string msgName, Request request) {
        if (msgListeners.ContainsKey(msgName)) {
            msgListeners[msgName](request);

        }
    }


    //连接
    public static void Connect(string ip, int port) {

        //状态判断
        if (isConnecting) {
            Debug.Log("Connect fail, isConnecting");
            return;
        }
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        client = new Client();
        client.AddAcceptHandle(OnConnect);
        client.AddReceiveHandle(OnReceive);
        client.AddTimeoutHandle(OnTimeout);
        //初始化成员
        InitState();
        //参数设置
        //Connect
        isConnecting = true;
        client.Connect(iPEndPoint);

       
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
        msgList = new List<Request>();
        //消息列表长度
        msgCount = 0;
        
    }

    /// <summary>
    /// Connect 回调
    /// </summary>
    private static void OnConnect(byte[] bytes,int length) {

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
        //client.Dispose();
        //TODO

    }

    //发送数据
    public static void Send(IMessage msg) {
        //状态判断
        if (isConnecting) {
            return;
        }
        if (isClosing) {
            return;
        }

        byte[] sendBytes = MsgHelper.Encode(msg);
        
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
        if (writeQueue == null) return;
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
            byte[] data = ba.bytes;
            
            client.Send(data);
            
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
    public static void OnReceive(uint conv,byte[] bytes,int length) {
        if (length <= 4) {
            return;
        }
        try {
            //获取接收数据长度
            bytes.CopyTo(readBuff.bytes, readBuff.writeIdx);
            //Array.Copy(bytes, 0, readBuff.bytes, readBuff.writeIdx, length);
            readBuff.writeIdx += length;
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


        Request msg = MsgHelper.Decode(readBuff,client.Peer.Conv);
        
        //添加到消息队列
        lock (msgList) {
            msgList.Add(msg);
            msgCount++;
        }
        //继续读取消息
        if (readBuff.length > 2) {
            OnReceiveData();
        }
    }

    //Update
    public static void Update() {
        SendUpdata();
        MsgUpdate();
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
            Request request = null;
            lock (msgList) {
                if (msgList.Count > 0) {
                    request = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--;
                }
            }
            //分发消息
            if (request != null) {
                FireMsg(request.Name, request);
            }
            //没有消息了
            else {
                break;
            }
        }
    }
}