using ProtoBuf;

[ProtoContract]
public class MsgPing:MsgBase {
    public MsgPing() {protoName = "MsgPing";}
}

[ProtoContract]
public class MsgPong:MsgBase {
    public MsgPong() {protoName = "MsgPong";}
} 
