
using ProtoBuf;
using System.Collections.Generic;

using ProtoBuf;
//获取个人简介内容
[ProtoContract]
public class MsgGetPlayerIntroduction : MsgBase
{
    public MsgGetPlayerIntroduction() { protoName = "MsgGetPlayerIntroduction"; }
    //服务端回
    [ProtoMember(1)]
    public string palyerIntroduction = "";
}

//保存个人简介内容
[ProtoContract]
public class MsgSavePlayerIntroduction : MsgBase
{
    public MsgSavePlayerIntroduction() { protoName = "MsgSavePlayerIntroduction"; }
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

//获取好友列表
[ProtoContract]
public class MsgGetFriendList : MsgBase
{
    public MsgGetFriendList() { protoName = "MsgGetFriendList"; }
    //服务端回
    //好友列表
    [ProtoMember(1)]
    public List<string> friendIdList = new List<string>();
    //0 成功 1 失败
    [ProtoMember(2)]
    public int result = 0;
}

//删除好友
[ProtoContract]
public class MsgDeleteFriend : MsgBase
{
    public MsgDeleteFriend() { protoName = "MsgDeleteFriend"; }
    //客户端发
    [ProtoMember(1)]
    public string friendId = "";
    [ProtoMember(2)]
    public string id = "";
    //服务端回
    //0 成功 1 失败
    [ProtoMember(3)]
    public int result = 0;
}

//添加好友
[ProtoContract]
public class MsgAddFriend : MsgBase
{
    public MsgAddFriend() { protoName = "MsgAddFriend"; }

    //客户端发 服务端回
    [ProtoMember(1)]
    public string friendId = "";
    [ProtoMember(2)]
    public string id = "";

    //服务端回
    //0 成功 1 玩家不存在 2 玩家不在线 3 已经是好友
    [ProtoMember(3)]
    public int result = 0;
}

//同意添加好友
[ProtoContract]
public class MsgAcceptAddFriend : MsgBase
{
    public MsgAcceptAddFriend() { protoName = "MsgAcceptAddFriend"; }

    //客户端发 服务端回
    [ProtoMember(1)]
    public string friendId = "";
    [ProtoMember(2)]
    public string id = "";

    //服务端回
    [ProtoMember(3)]
    public int result = 0;
}


