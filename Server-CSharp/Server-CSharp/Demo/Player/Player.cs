using Google.Protobuf;
using System;
using System.Collections.Generic;
using YT;

public class Player {
    /// <summary>玩家信息</summary>
    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();
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

    public static void OnPlayerDisconnect(uint conv) {


        Console.WriteLine("客户端断开连接 - " + conv);

        //Player 下线
        if (Players.ContainsKey(conv)) {
            Player player = Players[conv];
            //保存数据
            DBManager.UpdatePlayerData(player.id, player.data);
            //移除
            PlayerManager.RemovePlayer(player.id);
            Players.Remove(conv);
        }


    }


}


