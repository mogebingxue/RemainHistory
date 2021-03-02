using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;
using TKcp;

namespace YT
{
    public class Connection
    {
        //客户端的Peer
        public uint Conv;
        //缓存区
        public ByteArray readBuff = new ByteArray();

        public Server Server;

        public Connection(uint conv) {
            Conv = conv;
        }

        public void Send(IMessage message) {
            Server.Send(Conv, message);
        }
    }
}
