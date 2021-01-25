using Gate;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace YT
{
    partial class MsgHandler
    {
        public static void MsgPing(Connection c, IMessage msgBase) {
            Console.WriteLine("MsgPing");
            c.LastPingTime = c.GetTimeStamp();
            MsgPong msgPong = new MsgPong();
            c.Send(msgPong);
        }
    }
}
