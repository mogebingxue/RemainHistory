
using ProtoBuf;
using System.Collections.Generic;
using YT;
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


