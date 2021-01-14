using System;
using YT;

public class Player {
	//id
	public string id = "";
	//指向ClientState
	public Connection state;
	//构造函数
	public Player(Connection state){
		this.state = state;
	}
	
	//数据库数据
	public PlayerData data;

	//发送信息
	public void Send(MsgBase msgBase){
		state.Send(msgBase);
	}

	

}


