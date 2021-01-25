using ENet;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace YT
{
    public class Connection
    {
        //玩家
        public Player player;

        //
        public int ChannelID;
        //此客户端所连接的服务器
        public Server server;
        //客户端的Peer
        public Peer Peer;
        //player不必包含，应交给子类实现（或者包含它的一个类）
        private ByteArray readBuff = new ByteArray();
        //Ping
        public long LastPingTime = 0;

        public Connection() {
            LastPingTime = GetTimeStamp();
        }
        /// <summary>
        /// 客户端连接时，需要执行的方法，交由子类重写
        /// </summary>
        public virtual void OnConnect() {
            Console.WriteLine("客户端连接 - "+ "IP: " + Peer.IP);
            


            //Peer.SetPingInterval(1000);
            //Peer.SetTimeouts(8, 5000, 60000);
            server.Clients.Add(Peer, this);
        }
        /// <summary>
        /// 客户端断开连接时，需要执行的方法，交由子类重写
        /// </summary>
        public virtual void OnDisconnect() {
            Console.WriteLine("客户端断开连接 - " + "IP: " + Peer.IP);
            server.Clients.Remove(Peer);

            //Player 下线
            if (player != null) {
                //保存数据
                DBManager.UpdatePlayerData(player.id, player.data);
                //移除
                PlayerManager.RemovePlayer(player.id);
            }

        }
        /// <summary>
        /// 客户端超时时，需要执行的方法，交由子类重写
        /// </summary>
        public virtual void OnTimer() {
            Console.WriteLine("客户端超时 - " + "IP: " + Peer.IP);

        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public long GetTimeStamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        //Ping 检查
        public void CheckPing() {
            //现在的时间戳
            long timeNow = GetTimeStamp();
            //遍历，删除
            if (timeNow - LastPingTime > server.pingInterval * 4) {
                Console.WriteLine("Ping Close - "  +  "IP: " + Peer.IP);
                OnDisconnect();
                return;
            }

        }
        /// <summary>
        /// 客户端接受消息时，需要执行的方法
        /// </summary>
        public void OnReceive(Event netEvent) {

            //接收
            int count = 0;
            //缓冲区不够，清除，若依旧不够，只能返回
            //当单条协议超过缓冲区长度时会发生
            if (readBuff.remain <= 0) {
                OnReceiveData();
                readBuff.MoveBytes();
            }
            if (readBuff.remain <= 0) {
                Console.WriteLine("Receive fail , maybe msg length > buff capacity");
                Peer.Disconnect(0);
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
                Peer.Disconnect(0);
                return;
            }
            //客户端关闭
            if (count <= 0) {
                Peer.Disconnect(0);
                return;
            }
            //消息处理
            readBuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData();
            //移动缓冲区
            readBuff.CheckAndMoveBytes();
        }
        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="user"></param>
        private void OnReceiveData() {
            Request request = MsgHelper.Decode(readBuff, Peer);
            server.Requests.Enqueue(request);
            //继续读取消息
            if (readBuff.length > 2) {
                OnReceiveData();
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void Send(IMessage msg) {
            byte[] sendBytes = MsgHelper.Encode(msg);
            //发送
            try {
                byte channelID = (byte)ChannelID;
                Packet packet = default(Packet);
                byte[] data = sendBytes;
                //packet.SetUserData(data);
                packet.Create(data);
                Peer.Send(channelID, ref packet);
            }
            catch (Exception ex) {
                Console.WriteLine("Send failed" + ex.ToString());
            }
        }


    }
}
