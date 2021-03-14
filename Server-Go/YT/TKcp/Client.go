package TKcp

import (
	Log2 "ReaminHistory/YT/Log"
	"net"
)

type Client struct {
	Interval int
	Peer     *Peer

	//客户端udp
	socket      net.UDPConn
	localIpep   net.UDPAddr
	serverIpep  net.UDPAddr
	connectTime int64
}

func NewClient() *Client {
	client := &Client{Interval: 1000}

	client.initClient()

	return client
}

//初始化客户端
func (client *Client) initClient() {
	client.Peer = NewPeer(net.UDPConn{}, net.UDPAddr{}, 0)
	addr, err1 := net.ResolveUDPAddr("udp", "127.0.0.1:8886")
	if err1 != nil {
		Log2.Log.Panic("IP地址错误")
	}
	client.localIpep = *addr
	conn, err2 := net.ListenUDP("udp", addr)
	if err2 != nil {
		Log2.Log.Panic("监听UDP失败")
	}
	client.socket = *conn
}

//连接服务器
func (client *Client) Connect(server string) {
	addr, err1 := net.ResolveUDPAddr("udp", server)
	if err1 != nil {
		panic("IP地址错误")
	}
	client.serverIpep = *addr
	flag := 0
	sendBytes := make([]byte, 4)
	sendBytes[0] = uint8(flag)
	sendBytes[1] = uint8(flag >> 8)
	sendBytes[2] = uint8(flag >> 16)
	sendBytes[3] = uint8(flag >> 24)
	_, err2 := client.socket.WriteToUDP(sendBytes, addr)
	if err2 != nil {
		Log2.Log.Info("udp发送失败")
	}
	client.connectTime = GetTimeStamp()
	go client.updateAccept()
}

//客户端发送数据
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
		count, remote, err := client.socket.ReadFromUDP(recvBuffer)
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
			client.Peer.LocalSocket = client.socket
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
				_, err := client.socket.WriteToUDP(sendBytes, &client.serverIpep)
				if err != nil {
					Log2.Log.Info("udp发送失败")
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
		count, _, err := client.socket.ReadFromUDP(recvBuffer)
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

func (client *Client) AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	client.Peer.ReceiveHandle.Add(name, handleFunc)
}
func (client *Client) AddAcceptHandle(name string, handleFunc func(bytes []byte, len int)) {
	client.Peer.AcceptHandle.Add(name, handleFunc)
}
func (client *Client) AddTimeoutHandle(name string, handleFunc func()) {
	client.Peer.TimeoutHandle.Add(name, handleFunc)
}
