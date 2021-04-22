package TKCP

import (
	Log2 "ReaminHistory/YT/Log"
	"net"
)

// Client TKCP客户端
type Client struct {
	//心跳间隔
	Interval int
	//客户端Peer
	peer *peer
	//客户端UDP
	conn *net.UDPConn
	//客户端本地地址
	localAddr net.UDPAddr
	//服务端地址
	serverAddr net.UDPAddr
	//连接时间
	connectTime int64
}

func NewClient() *Client {
	client := &Client{Interval: 1000}
	client.peer = newPeer(&net.UDPConn{}, net.UDPAddr{}, 0)
	return client
}

// Connect 连接服务器
// server 要连接的服务端地址和端口号
func (client *Client) Connect(server string) {
	conn, err := net.Dial("tcp", server)
	if err != nil {
		Log2.Log.Info("err :", err)
		return
	}

	//设置客户端UDP信息
	addr, err := net.ResolveUDPAddr("udp", conn.LocalAddr().String())
	if err != nil {
		Log2.Log.Panic("IP地址错误")
	}
	client.localAddr = *addr
	client.conn, err = net.ListenUDP("udp", addr)
	if err != nil {
		Log2.Log.Panic("监听UDP失败")
	}

	defer conn.Close() // 关闭连接

	buf := [1024]byte{}
	n, err := conn.Read(buf[:])
	if err != nil {
		Log2.Log.Info("TCP接收消息失败", err)
		return
	}
	convBytes := buf[:n]
	head := uint32(convBytes[0]) | uint32(convBytes[1])<<8 | uint32(convBytes[2])<<16 | uint32(convBytes[3])<<24
	if head == 0 {
		Log2.Log.Info("连接已满，稍后重试!")
		return
	} else {
		remoteAddr, err := net.ResolveUDPAddr("udp", server)
		if err != nil {
			panic("IP地址错误")
		}

		client.serverAddr = *remoteAddr
		client.connectTime = getTimeStamp()
		client.peer.LocalSocket = client.conn
		client.peer.Remote = *remoteAddr
		client.peer.Conv = head
		client.peer.InitKcp()
		client.peer.AcceptHandle.Call(convBytes, 4)
		go client.update()
		go client.updatePeer()
		//退出此go程
		return

	}
}

// Send 客户端发送数据
func (client *Client) Send(sendBytes []byte) {
	if client.peer.Conv == 0 {
		Log2.Log.Info("未与服务器建立连接")
		return
	}
	client.peer.Send(sendBytes)
}

//更新接收信息
func (client *Client) update() {
	for {
		recvBuffer := make([]byte, 1024)
		count, _, err := client.conn.ReadFromUDP(recvBuffer)
		if count <= 0 {
			return
		}
		if err != nil {
			return
		}

		client.peer.Kcp.Input(recvBuffer, true, true)

	}
}

//更新Peer
func (client *Client) updatePeer() {
	for {
		if client.peer.Conv == 0 {
			continue
		}
		client.peer.PeerUpdate()
	}
}

//注册回调

// AddReceiveHandle 注册接收消息回调
func (client *Client) AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	client.peer.ReceiveHandle.Add(name, handleFunc)
}

// AddAcceptHandle 注册回调
func (client *Client) AddAcceptHandle(name string, handleFunc func(bytes []byte, len int)) {
	client.peer.AcceptHandle.Add(name, handleFunc)
}

// AddTimeoutHandle 注册超时回调
func (client *Client) AddTimeoutHandle(name string, handleFunc func()) {
	client.peer.TimeoutHandle.Add(name, handleFunc)
}
