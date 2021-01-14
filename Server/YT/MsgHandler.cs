using System;
using System.Collections.Generic;
using System.Text;

namespace YT
{
    partial class MsgHandler
    {
        public static void MsgPing(ClientState c, MsgBase msgBase) {
            Console.WriteLine("MsgPing");
            c.LastPingTime = c.GetTimeStamp();
            MsgPong msgPong = new MsgPong();
            c.Send(msgPong);
        }
    }
}
