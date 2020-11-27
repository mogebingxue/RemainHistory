using System;

public partial class EventHandler
{
	//客户端socket 断开连接时执行的操作
	public static void OnDisconnect(ClientState c){
		Console.WriteLine("Close");
		//Player 下线
		if(c.player != null){
			//保存数据
			DbManager.UpdatePlayerData(c.player.id, c.player.data);
			//移除
			PlayerManager.RemovePlayer(c.player.id);
		}
	}
		

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
				Console.WriteLine("Ping Close " + s.socket.RemoteEndPoint.ToString());
				NetManager.Close(s);
				return;
			}
		}
	}


}

