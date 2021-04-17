package TKcp

import (
	Log2 "ReaminHistory/YT/Log"
	"net"
	"strconv"
)

// Client TKCP客户端
type Client struct {
	//心跳间隔
	Interval int
	//客户端Peer
	Peer     *Peer
	//IP地址
	Ip             string
	Port           int
	//客户端UDP
	conn net.UDPConn
	//客户端本地地址
	localAddr   net.UDPAddr
	//服务端地址
	serverAddr  net.UDPAddr
	//连接时间
	connectTime int64
}

func NewClient() *Client {
	client := &Client{Interval: 1000,Ip: "127.0.0.1",Port: 8886}
	client.initClient()
	return client
}

//初始化客户端
func (client *Client) initClient() {
	client.Peer = NewPeer(net.UDPConn{}, net.UDPAddr{}, 0)
	addr, err1 := net.ResolveUDPAddr("udp", client.Ip+":"+strconv.Itoa(client.Port))
	if err1 != nil {
		Log2.Log.Panic("IP地址错误")
	}
	client.localAddr = *addr
	conn, err2 := net.ListenUDP("udp", addr)
	if err2 != nil {
		Log2.Log.Panic("监听UDP失败")
	}
	client.conn = *conn
}

// Connect 连接服务器
// server 要连接的服务端地址和端口号
func (client *Client) Connect(server string) {
	addr, err := net.ResolveUDPAddr("udp", server)
	if err != nil {
		panic("IP地址错误")
	}
	client.serverAddr = *addr
	//flag=0,第一次请求报文
	flag := 0
	sendBytes := make([]byte, 4)
	sendBytes[0] = uint8(flag)
	sendBytes[1] = uint8(flag >> 8)
	sendBytes[2] = uint8(flag >> 16)
	sendBytes[3] = uint8(flag >> 24)
	_, err = client.conn.WriteToUDP(sendBytes, addr)
	if err != nil {
		Log2.Log.Info("UDP发送报文失败",err)
	}
	client.connectTime = GetTimeStamp()
	go client.updateAccept()
}

// Send 客户端发送数据
func (client *Client) Send(sendBytes []byte) {
	if client.Peer.Conv == 0 {
		Log2.Log.Info("未与服务器建立连接")
		return
	}
	client.Peer.Send(sendBytes)
}

//接收同意连接的消息
func (client *Client) updateAccept() {
	for {
		recvBuffer := make([]byte, 1024)
		count, remote, err := client.conn.ReadFromUDP(recvBuffer)
		if count <= 0 {
			return
		}
		if err != nil {
			return
		}
		headBytes := recvBuffer[0:4]
		head := uint32(headBytes[0]) | uint32(headBytes[1])<<8 | uint32(headBytes[2])<<16 | uint32(headBytes[3])<<24

		if head == 1 {
			convBytes := recvBuffer[4:8]
			conv := uint32(convBytes[0]) | uint32(convBytes[1])<<8 | uint32(convBytes[2])<<16 | uint32(convBytes[3])<<24
			client.Peer.LocalSocket = client.conn
			client.Peer.Remote = *remote
			client.Peer.Conv = conv
			client.Peer.InitKcp()
			client.Peer.AcceptHandle.Call(convBytes, 4)
			go client.update()
			go client.updatePeer()
			//退出此go程
			return
		} else {
			timeNow := GetTimeStamp()
			//重连
			if timeNow-client.connectTime > int64(client.Interval) {
				flag := 0
				sendBytes := make([]byte, 4)
				sendBytes[0] = uint8(flag)
				sendBytes[1] = uint8(flag >> 8)
				sendBytes[2] = uint8(flag >> 16)
				sendBytes[3] = uint8(flag >> 24)
				_, err := client.conn.WriteToUDP(sendBytes, &client.serverAddr)
				if err != nil {
					Log2.Log.Info("UDP发送报文失败",err)
				}
				client.connectTime = GetTimeStamp()
				client.Peer.TimeoutTime++

			}
			//超时
			if client.Peer.TimeoutTime >= 4 {
				client.Peer.TimeoutHandle.Call()
			}
		}
	}

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
		convBytes := recvBuffer[:4]
		head := uint32(convBytes[0]) | uint32(convBytes[1])<<8 | uint32(convBytes[2])<<16 | uint32(convBytes[3])<<24
		if head != 1 {
			client.Peer.Kcp.Input(recvBuffer, true, true)
		}
	}
}

//更新Peer
func (client *Client) updatePeer() {
	for {
		if client.Peer.Conv == 0 {
			continue
		}
		client.Peer.PeerUpdate()
	}
}

//注册回调

// AddReceiveHandle 注册接收消息回调
func (client *Client) AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	client.Peer.ReceiveHandle.Add(name, handleFunc)
}

// AddAcceptHandle 注册第二次握手回调
func (client *Client) AddAcceptHandle(name string, handleFunc func(bytes []byte, len int)) {
	client.Peer.AcceptHandle.Add(name, handleFunc)
}

// AddTimeoutHandle 注册超时回调
func (client *Client) AddTimeoutHandle(name string, handleFunc func()) {
	client.Peer.TimeoutHandle.Add(name, handleFunc)
}
