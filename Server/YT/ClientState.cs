using ENet;
using System;
using System.Collections.Generic;
using System.Text;

namespace YT
{
    public class ClientState
    {
        //此客户端所连接的服务器
        public Server server;
        //客户端的Peer
        public Peer Peer;
        //player不必包含，应交给子类实现（或者包含它的一个类）
        private ByteArray readBuff = new ByteArray();
        //Ping
        public long LastPingTime = 0;

        public ClientState() {
            LastPingTime = GetTimeStamp();
        }
        /// <summary>
        /// 客户端连接时，需要执行的方法，交由子类重写
        /// </summary>
        public virtual void OnConnect() {
            Console.WriteLine("客户端连接 - ID: " + Peer.ID + ", IP: " + Peer.IP);
            server.Clients.Add(Peer, this);
        }
        /// <summary>
        /// 客户端断开连接时，需要执行的方法，交由子类重写
        /// </summary>
        public virtual void OnDisconnect() {
            Console.WriteLine("客户端断开连接 - ID: " + Peer.ID + ", IP: " + Peer.IP);
            server.Clients.Remove(Peer);

        }
        /// <summary>
        /// 客户端超时时，需要执行的方法，交由子类重写
        /// </summary>
        public virtual void OnTimer() {
            Console.WriteLine("客户端超时 - ID: " + Peer.ID + ", IP: " + Peer.IP);

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
                Console.WriteLine("Ping Close " + " ID " + Peer.ID + " IP " + Peer.IP);
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
                //Close(user);
                return;
            }
            readBuff.readIdx += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            if (bodyCount < 0) {
                Console.WriteLine("OnReceiveData fail, bodyCount <0 ");
                //Close(user);
                return;
            }
            MsgBase msg = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            Request request = new Request();
            request.Peer = Peer;
            request.Name = protoName;
            
            request.Msg = msg;
            server.Requests.Enqueue(request);
            //继续读取消息
            if (readBuff.length > 2) {
                OnReceiveData();
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void Send(MsgBase msg) {
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
                byte channelID = (byte)Peer.ID;
                Packet packet = default(Packet);
                byte[] data = sendBytes;
                packet.Create(data);
                Peer.Send(channelID, ref packet);
            }
            catch (Exception ex) {
                Console.WriteLine("Send failed" + ex.ToString());
            }
        }


    }
}
