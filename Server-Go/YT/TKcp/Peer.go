package TKcp

import (
	Log2 "ReaminHistory/YT/Log"
	"github.com/xtaci/kcp-go/v5"
	"net"
	"time"
)

type Peer struct {

	//本地的 socket
	LocalSocket net.UDPConn
	//远端的EndPoint
	Remote net.UDPAddr
	//连接号
	Conv uint32
	//上次Ping的时间
	LastPingTime int64
	//kcp
	Kcp *kcp.KCP
	//kcp模式
	Model int
	//超时时间
	TimeoutTime int

	//应用层接收消息之后的回调
	ReceiveHandle ReceiveHandle
	//连接请求的回调，在这里要实现回传同意连接和连接号给客户端
	ConnectHandle ConnectHandle
	//接收连接请求回调，客户端收到服务端的接收连接请求，之后的回调
	AcceptHandle AcceptHandle
	//断开连接请求回调，服务端断开一个连接，之后的回调
	DisconnectHandle DisconnectHandle
	//客户端连接超时的回调
	TimeoutHandle TimeoutHandle
}

//构造函数
func NewPeer(localSocket net.UDPConn, remote net.UDPAddr, conv uint32) *Peer {
	peer := &Peer{LocalSocket: localSocket, Remote: remote, Conv: conv, Model: FAST, LastPingTime: GetTimeStamp()}
	peer.initPeer()
	return peer
}

//Model的枚举
const (
	FAST   = 0
	NORMAL = 1
)

//初始化Kcp
func (peer *Peer) InitKcp() {
	//当这个Peer曾被使用过时，先释放它的Kcp，在重设
	if peer.Kcp != nil {
		peer.Kcp.ReleaseTX()
		peer.Kcp = nil
	}
	//初始化Kcp
	peer.Kcp = kcp.NewKCP(peer.Conv, func(buf []byte, size int) {
		peer.handle(buf, size)
	})
	if peer.Model == FAST {
		peer.Kcp.NoDelay(1, 10, 2, 1)
	} else {
		peer.Kcp.NoDelay(0, 40, 0, 0)
	}
	peer.Kcp.WndSize(64, 64)
	peer.Kcp.SetMtu(512)
}

//初始化Peer
func (peer *Peer) initPeer() {
	peer.ConnectHandle.Add("onConnect", func(bytes []byte) {
		peer.onConnect(bytes)
	})
	peer.TimeoutHandle.Add("onTimeout", func() {
		peer.onTimeout()
	})
	peer.AcceptHandle.Add("onAccept", func(bytes []byte, len int) {
		peer.onAccept(bytes, len)
	})
}

//给客户端发送连接号
func (peer *Peer) onConnect(conv []byte) {
	if len(conv) != 4 {
		return
	}
	sendBytes := make([]byte, 4)
	//1代表是同意连接的回调
	flag := 1
	sendBytes[0] = uint8(flag)
	sendBytes[1] = uint8(flag >> 8)
	sendBytes[2] = uint8(flag >> 16)
	sendBytes[3] = uint8(flag >> 24)
	sendBytes = append(sendBytes, conv...)
	_, err := peer.LocalSocket.WriteToUDP(sendBytes, &peer.Remote)
	if err != nil {
		Log2.Log.Info("udp发送失败")
	}
	peer.ReceiveHandle.Add("onReceive", func(conv uint32, bytes []byte, len int) {
		peer.onReceive(conv, bytes, len)
	})
	peer.DisconnectHandle.Add("onDisconnect", func(conv uint32) {
		peer.onDisconnect(conv)
	})

}

func (peer *Peer) onTimeout() {
	Log2.Log.Info("连接超时，请检查你的网络")
}

func (peer *Peer) onDisconnect(conv uint32) {
	Log2.Log.Info("客户端 ", conv, "断开")
}

func (peer *Peer) onAccept(conv []byte, len int) {
	Log2.Log.Info("客户端收到接受了连接请求", conv)
	peer.ReceiveHandle.Add("onReceive", func(conv uint32, bytes []byte, len int) {
		peer.onReceive(conv, bytes, len)
	})
}

func (peer *Peer) onReceive(conv uint32, bytes []byte, len int) {
	peer.LastPingTime = GetTimeStamp()
	peer.TimeoutTime = 0
	if len == 4 {
		msg := uint32(bytes[0]) | uint32(bytes[1])<<8 | uint32(bytes[2])<<16 | uint32(bytes[3])<<24
		if msg == 2 {
			peer.Pong()
		}
	}
}

func (peer *Peer) Ping() {
	sendBytes := make([]byte, 4)
	//2代表服务端发送的Ping
	flag := 2
	sendBytes[0] = uint8(flag)
	sendBytes[1] = uint8(flag >> 8)
	sendBytes[2] = uint8(flag >> 16)
	sendBytes[3] = uint8(flag >> 24)
	peer.Send(sendBytes)
}

func (peer *Peer) Pong() {
	sendBytes := make([]byte, 4)
	//3代表客户端发送的Pong
	flag := 3
	sendBytes[0] = uint8(flag)
	sendBytes[1] = uint8(flag >> 8)
	sendBytes[2] = uint8(flag >> 16)
	sendBytes[3] = uint8(flag >> 24)
	peer.Send(sendBytes)
}

//发送数据
func (peer *Peer) Send(bytes []byte) {
	peer.Kcp.Send(bytes)
	Log2.Log.Info("发送数据"+"TO", peer.Remote, peer.Conv, bytes)
}

//Peer 的更新操作，负责接收来自udp的数据
func (peer *Peer) PeerUpdate() {
	peer.Kcp.Update()
	receiveBytes := make([]byte, 1024)
	availedSize := peer.Kcp.Recv(receiveBytes)
	if availedSize > 0 {
		peer.ReceiveHandle.Call(peer.Conv, receiveBytes, availedSize)
	}
}

//获取当前时间戳
func GetTimeStamp() int64 {
	nowTimeUnix := time.Now().Unix()
	loc, _ := time.LoadLocation("Asia/Beijing")                                   //设置时区
	tt, _ := time.ParseInLocation("2006-01-02 15:04:05", "1970-01-01 0:0:0", loc) //2006-01-02 15:04:05是转换的格式如php的"Y-m-d H:i:s"
	return nowTimeUnix - tt.Unix()
}

//output回调函数
func (peer *Peer) handle(buf []byte, size int) {
	if size > 0 {
		_, err := peer.LocalSocket.WriteToUDP(buf, &peer.Remote)
		if err != nil {
			Log2.Log.Info("udp发送失败")
		}
	}
}
