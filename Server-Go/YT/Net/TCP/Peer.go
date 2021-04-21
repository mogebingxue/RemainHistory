package TCP

import (
	Log2 "ReaminHistory/YT/Log"
	"net"
)

type Peer struct {

	//远端的地址
	Remote net.TCPAddr
	//连接号
	Conv uint32
	//上次Ping的时间
	LastPingTime int64
	//tcp conn
	conn *net.TCPConn
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

// NewPeer 构造函数
func NewPeer(remote net.TCPAddr, conv uint32) *Peer {
	peer := &Peer{Remote: remote, Conv: conv, LastPingTime: GetTimeStamp()}
	peer.initPeer()
	return peer
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

//连接后
func (peer *Peer) onConnect(conv []byte) {
	//注册接收回调
	peer.ReceiveHandle.Add("onReceive", func(conv uint32, bytes []byte, len int) {
		peer.onReceive(conv, bytes, len)
	})
	//注册断开连接回调
	peer.DisconnectHandle.Add("onDisconnect", func(conv uint32) {
		peer.onDisconnect(conv)
	})
	go peer.PeerUpdate()
}

func (peer *Peer) onTimeout() {
	Log2.Log.Info("连接超时，请检查你的网络")
}

func (peer *Peer) onDisconnect(conv uint32) {
	err := peer.conn.Close()
	if err != nil {
		return
	}
	Log2.Log.Info("连接 ", conv, "断开")
}

func (peer *Peer) onAccept(conv []byte, len int) {
	peer.ReceiveHandle.Add("onReceive", func(conv uint32, bytes []byte, len int) {
		peer.onReceive(conv, bytes, len)
	})
	go peer.PeerUpdate()
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

// Ping 发送心跳包
func (peer *Peer) Ping() {
	sendBytes := make([]byte, 4)
	//9代表服务端发送的Ping
	flag := 2
	sendBytes[0] = uint8(flag)
	sendBytes[1] = uint8(flag >> 8)
	sendBytes[2] = uint8(flag >> 16)
	sendBytes[3] = uint8(flag >> 24)
	peer.Send(sendBytes)
}

// Pong 回送心跳包
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

// Send 发送数据
func (peer *Peer) Send(bytes []byte) {
	_, err := peer.conn.Write(bytes)
	if err != nil {
		return
	}
	Log2.Log.Info("发送数据"+"TO", peer.Remote, peer.Conv, " ", string(bytes))
}

// PeerUpdate Peer 的更新操作，负责接收来自tcp的数据
func (peer *Peer) PeerUpdate() {
	for {
		receiveBytes := make([]byte, 1024)
		n, err := peer.conn.Read(receiveBytes)
		if err != nil {
			Log2.Log.Info("TCP接收消息失败", err)
		}
		peer.ReceiveHandle.Call(peer.Conv, receiveBytes, n)
	}
}
