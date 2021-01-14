using ENet;
using System.Net.Sockets;
using YT;

public class ClientState
{
	public Peer peer;
	public ByteArray readBuff = new ByteArray(); 
	//Ping
	public long lastPingTime = 0;
	//玩家
	public Player player;
}