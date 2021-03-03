using Google.Protobuf;
using System;
using System.Collections.Generic;
using YT;

public class Player {
    
    //id
    public string id = "";
	public uint Conv;
	//构造函数
	public Player(uint conv){
		this.Conv = conv;
	}
	
	//数据库数据
	public PlayerData data;

	//发送信息
	public void Send(IMessage msgBase){
		Server.Clients[Conv].Send(msgBase);
	}

    


}


