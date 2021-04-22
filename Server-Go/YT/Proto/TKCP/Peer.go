package TKCP

import (
	Log2 "ReaminHistory/YT/Log"
	"github.com/xtaci/kcp-go/v5"
	"net"
)

type peer struct {

	//本地的 socket
	LocalSocket *net.UDPConn
	//远端的地址
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
	ReceiveHandle receiveHandle
	//连接请求的回调，在这里要实现回传同意连接和连接号给客户端
	ConnectHandle connectHandle
	//接收连接请求回调，客户端收到服务端的接收连接请求，之后的回调
	AcceptHandle acceptHandle
	//断开连接请求回调，服务端断开一个连接，之后的回调
	DisconnectHandle disconnectHandle
	//客户端连接超时的回调
	TimeoutHandle timeoutHandle
}

//Model的枚举
const (
	fast   = 0
	normal = 1
)

// NewPeer 构造函数
func newPeer(localSocket *net.UDPConn, remote net.UDPAddr, conv uint32) *peer {
	peer := &peer{LocalSocket: localSocket, Remote: remote, Conv: conv, Model: fast, LastPingTime: getTimeStamp()}
	peer.initPeer()
	return peer
}

// InitKcp 初始化Kcp
func (peer *peer) InitKcp() {
	//当这个Peer曾被使用过时，先释放它的Kcp，在重设
	if peer.Kcp != nil {
		peer.Kcp.ReleaseTX()
		peer.Kcp = nil
	}
	//初始化Kcp
	peer.Kcp = kcp.NewKCP(peer.Conv, func(buf []byte, size int) {
		peer.handle(buf, size)
	})
	if peer.Model == fast {
		peer.Kcp.NoDelay(1, 10, 2, 1)
	} else {
		peer.Kcp.NoDelay(0, 40, 0, 0)
	}
	peer.Kcp.WndSize(64, 64)
	peer.Kcp.SetMtu(512)
}

//初始化Peer
func (peer *peer) initPeer() {
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
func (peer *peer) onConnect(conv []byte) {
	//注册接收回调
	peer.ReceiveHandle.Add("onReceive", func(conv uint32, bytes []byte, len int) {
		peer.onReceive(conv, bytes, len)
	})
	//注册断开连接回调
	peer.DisconnectHandle.Add("onDisconnect", func(conv uint32) {
		peer.onDisconnect(conv)
	})

}

func (peer *peer) onTimeout() {
	Log2.Log.Info("连接超时，请检查你的网络")
}

func (peer *peer) onDisconnect(conv uint32) {
	Log2.Log.Info("连接 ", conv, "断开")
}

func (peer *peer) onAccept(conv []byte, len int) {
	peer.ReceiveHandle.Add("onReceive", func(conv uint32, bytes []byte, len int) {
		peer.onReceive(conv, bytes, len)
	})
}

func (peer *peer) onReceive(conv uint32, bytes []byte, len int) {
	peer.LastPingTime = getTimeStamp()
	peer.TimeoutTime = 0
	if len == 4 {
		msg := uint32(bytes[0]) | uint32(bytes[1])<<8 | uint32(bytes[2])<<16 | uint32(bytes[3])<<24
		if msg == 2 {
			peer.Pong()
		}
	}
}

// Ping 发送心跳包
func (peer *peer) Ping() {
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
func (peer *peer) Pong() {
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
func (peer *peer) Send(bytes []byte) {
	peer.Kcp.Send(bytes)
	Log2.Log.Info("发送数据"+"TO", peer.Remote, peer.Conv, " ", string(bytes))
}

// PeerUpdate peer 的更新操作，负责接收来自udp的数据
func (peer *peer) PeerUpdate() {
	peer.Kcp.Update()
	receiveBytes := make([]byte, 1024)
	availedSize := peer.Kcp.Recv(receiveBytes)
	if availedSize > 0 {
		peer.ReceiveHandle.Call(peer.Conv, receiveBytes[:availedSize], availedSize)
	}
}

//output回调函数
func (peer *peer) handle(buf []byte, size int) {
	if size > 0 {
		_, err := peer.LocalSocket.WriteToUDP(buf, &peer.Remote)
		if err != nil {
			Log2.Log.Warn("UDP发送报文失败", err)
		}
	}
}
