using Google.Protobuf;
using System;
using System.Collections.Generic;
using YT;

public class PlayerManager
{

    /// <summary>玩家信息列表</summary>
    public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();
    public static void OnPlayerDisconnect(uint conv) {
        Console.WriteLine("客户端断开连接 - " + conv);
        //Player 下线
        if (Players.ContainsKey(conv)) {
            Player player = Players[conv];
            //保存数据
            DBManager.UpdatePlayerData(player.id, player.data);
            //移除
            RemovePlayer(player.id);
            Players.Remove(conv);
        }
    }
    //玩家列表
    static Dictionary<string, Player> players = new Dictionary<string, Player>();
	//玩家是否在线
	public static bool IsOnline(string id){
		return players.ContainsKey(id);
	}
	//获取玩家
	public static Player GetPlayer(string id){
		if(players.ContainsKey(id)){
			return players[id];
		}
		return null;
	}
	//添加玩家
	public static void AddPlayer(string id, Player player){
		players.Add(id, player);
	}
	//删除玩家
	public static void RemovePlayer(string id){
		players.Remove(id);
	}
	//广播
    public static void Broadcast(IMessage msgBase) {
        foreach (Player player in players.Values) {
			player.Send(msgBase);
        }
    }
}