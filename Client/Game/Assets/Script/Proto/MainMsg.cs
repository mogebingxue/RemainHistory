//获取个人简介内容
public class MsgGetPlayerIntroduction : MsgBase {
	public MsgGetPlayerIntroduction() {protoName = "MsgGetPlayerIntroduction"; }
	//服务端回
	public string palyerIntroduction = "";
}

//保存个人简介内容
public class MsgSavePlayerIntroduction : MsgBase {
	public MsgSavePlayerIntroduction() {protoName = "MsgSavePlayerIntroduction"; }
	//客户端发
	public string palyerIntroduction = "";
	//服务端回（0-成功 1-文字太长）
	public int result = 0;
}

//发送消息到世界
public class MsgSendMessageToWord : MsgBase
{
	public MsgSendMessageToWord() { protoName = "MsgSendMessageToWord"; }
	//客户端发 服务端回
	public string message = "";
	public string id = "";
	//服务端回（0-成功 1-文字太长）
	public int result = 0;
}
//发送消息到好友
public class MsgSendMessageToFriend : MsgBase
{
    public MsgSendMessageToFriend() { protoName = "MsgSendMessageToFriend"; }
	//客户端发 服务端回
	public string message = "";
	public string id = "";
	//服务端回（0-成功 1-文字太长）
	public int result = 0;
}
