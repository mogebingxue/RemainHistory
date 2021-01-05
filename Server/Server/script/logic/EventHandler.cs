using System;

public partial class EventHandler
{
	//客户端断开连接时执行的操作
	public static void OnDisconnect(ClientState c){
		
		//Player 下线
		if (c.player != null){
			//保存数据
			DBManager.UpdatePlayerData(c.player.id, c.player.data);
			//移除
			PlayerManager.RemovePlayer(c.player.id);
		}
	}
		
	/// <summary>
	/// 超时
	/// </summary>
	public static void OnTimer(){
		CheckPing();
	}

	//Ping 检查
	public static void CheckPing(){
		//现在的时间戳
		long timeNow = NetManager.GetTimeStamp();
		//遍历，删除
		foreach(ClientState s in NetManager.clients.Values){
			if(timeNow - s.lastPingTime > NetManager.pingInterval*4){
				Console.WriteLine("Ping Close " +" ID "+ s.peer.ID + " IP " + s.peer.IP);
				NetManager.Close(s);
				return;
			}
		}
	}


}

