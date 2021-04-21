package TCP

import (
	Log2 "ReaminHistory/YT/Log"
	"net"
	"strconv"
)

// Client TCP客户端
type Client struct {
	//心跳间隔
	Interval int
	//客户端Peer
	Peer *Peer
	//客户端本地地址
	Ip   string
	Port int
	//连接时间
	connectTime int64
}

func NewClient() *Client {
	client := &Client{Interval: 1000, Ip: "127.0.0.1", Port: 8880}
	client.Peer = NewPeer(net.TCPAddr{}, 0)
	return client
}

// Connect 连接服务器
// server 要连接的服务端地址和端口号
func (client *Client) Connect(server string) {
	localAddr, err := net.ResolveTCPAddr("tcp", client.Ip+":"+strconv.Itoa(client.Port))
	if err != nil {
		Log2.Log.Info("解析地址失败")
	}
	serverAddr, err := net.ResolveTCPAddr("tcp", server)
	if err != nil {
		Log2.Log.Info("解析地址失败")
	}
	conn, err := net.DialTCP("tcp", localAddr, serverAddr)
	if err != nil {
		Log2.Log.Info("连接tcp失败:", err)
		return
	}

	client.Peer.conn = conn
	remote, err := net.ResolveTCPAddr("tcp", conn.RemoteAddr().String())
	if err != nil {
		Log2.Log.Info("解析地址失败")
	}
	client.Peer.Remote = *remote
	client.connectTime = GetTimeStamp()
	client.Peer.AcceptHandle.Call([]byte{}, 0)
	return
}

// Send 客户端发送数据
func (client *Client) Send(sendBytes []byte) {
	client.Peer.Send(sendBytes)
}

//注册回调

// AddReceiveHandle 注册接收消息回调
func (client *Client) AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	client.Peer.ReceiveHandle.Add(name, handleFunc)
}

// AddAcceptHandle 注册回调
func (client *Client) AddAcceptHandle(name string, handleFunc func(bytes []byte, len int)) {
	client.Peer.AcceptHandle.Add(name, handleFunc)
}

// AddTimeoutHandle 注册超时回调
func (client *Client) AddTimeoutHandle(name string, handleFunc func()) {
	client.Peer.TimeoutHandle.Add(name, handleFunc)
}
