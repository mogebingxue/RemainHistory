//注册
using ProtoBuf;

[ProtoContract]
public class MsgRegister : MsgBase
{
    public MsgRegister() { protoName = "MsgRegister"; }
    //客户端发
    [ProtoMember(1)]
    public string id = "";
    [ProtoMember(2)]
    public string pw = "";
    //服务端回（0-成功，1-失败）
    [ProtoMember(3)]
    public int result = 0;
}


//登陆
[ProtoContract]
public class MsgLogin : MsgBase
{
    public MsgLogin() { protoName = "MsgLogin"; }
    //客户端发
    [ProtoMember(1)]
    public string id = "";
    [ProtoMember(2)]
    public string pw = "";
    //服务端回（0-成功，1-失败）
    [ProtoMember(3)]
    public int result = 0;
}


//踢下线（服务端推送）
[ProtoContract]
public class MsgKick : MsgBase
{
    public MsgKick() { protoName = "MsgKick"; }
    //原因（0-其他人登陆同一账号）
    [ProtoMember(1)]
    public int reason = 0;
}