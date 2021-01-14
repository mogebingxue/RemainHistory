
using ProtoBuf;
using YT;
//获取个人简介内容
[ProtoContract]
public class MsgGetPlayerIntroduction : MsgBase {
	public MsgGetPlayerIntroduction() {protoName = "MsgGetPlayerIntroduction"; }
    //服务端回
    [ProtoMember(1)]
    public string palyerIntroduction = "";
}

//保存个人简介内容
[ProtoContract]
public class MsgSavePlayerIntroduction : MsgBase {
	public MsgSavePlayerIntroduction() {protoName = "MsgSavePlayerIntroduction"; }
    //客户端发
    [ProtoMember(1)]
    public string palyerIntroduction = "";
    //服务端回（0-成功 1-文字太长）
    [ProtoMember(2)]
    public int result = 0;
}

//获取头像
[ProtoContract]
public class MsgGetHeadPhoto : MsgBase
{
    public MsgGetHeadPhoto() { protoName = "MsgGetHeadPhoto"; }
    //服务端回
    [ProtoMember(1)]
    public int headPhoto = 0;
}

//保存头像
[ProtoContract]
public class MsgSaveHeadPhoto : MsgBase
{
    public MsgSaveHeadPhoto() { protoName = "MsgSaveHeadPhoto"; }
    //客户端发
    [ProtoMember(1)]
    public int headPhoto = 0;
    //服务端回
    [ProtoMember(2)]
    public int result = 0;
}
//发送消息到世界
[ProtoContract]
public class MsgSendMessageToWord : MsgBase
{
    public MsgSendMessageToWord() { protoName = "MsgSendMessageToWord"; }
    //客户端发 服务端回
    [ProtoMember(1)]
    public string message = "";
    [ProtoMember(2)]
    public string id = "";
    //服务端回（0-成功 1-文字太长）
    [ProtoMember(3)]
    public int result = 0;
    
}

//发送消息到好友
[ProtoContract]
public class MsgSendMessageToFriend : MsgBase
{
    public MsgSendMessageToFriend() { protoName = "MsgSendMessageToFriend"; }
    //客户端发 服务端回
    [ProtoMember(1)]
    public string message = "";
    [ProtoMember(2)]
    public string id = "";
    //服务端回（0-成功 1-文字太长）
    [ProtoMember(3)]
    public int result = 0;
}