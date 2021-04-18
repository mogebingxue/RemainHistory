package TKcp

import (
	Log2 "ReaminHistory/YT/Log"
	"math/rand"
	"net"
	"strconv"
	"time"
)

// Server TKCP服务器端
type Server struct {
	//最大连接数量
	MaxConnections int
	//IP地址
	Ip           string
	Port         int
	PingInterval int
	Peers        map[uint32]*Peer

	clients map[uint32]net.UDPAddr
	//对象池
	peerpool []*Peer
	//服务端udp
	socket    *net.UDPConn
	localAddr net.UDPAddr
}

func NewServer() *Server {
	server := &Server{MaxConnections: 999, Ip: "127.0.0.1", Port: 8888, PingInterval: 500}
	server.initServer()
	return server
}

func (server *Server) initServer() {
	addr, err1 := net.ResolveUDPAddr("udp", server.Ip+":"+strconv.Itoa(server.Port))
	if err1 != nil {
		Log2.Log.Panic("IP地址错误")
	}
	server.localAddr = *addr
	conn, err2 := net.ListenUDP("udp", addr)
	if err2 != nil {
		Log2.Log.Panic("监听UDP失败")
	}

	server.socket = conn
	server.Peers = make(map[uint32]*Peer)
	server.clients = make(map[uint32]net.UDPAddr)
	//初始化缓存池
	server.peerpool = make([]*Peer, server.MaxConnections)
	for key, _ := range server.peerpool {
		server.peerpool[key] = NewPeer(server.socket, net.UDPAddr{}, uint32(key+1000))
	}
	go server.connectUpdate()
	go server.update()
	go server.updatePeer()
}

// GenerateConv 连接号生成器
func (server *Server) GenerateConv() uint32 {
	rand.Seed(time.Now().Unix())
	conv := rand.Intn(server.MaxConnections) + 1000
	return uint32(conv)
}

// CheckPing 检查PING
func (server *Server) CheckPing(peer Peer) {
	//获取当前时间戳
	timeNow := GetTimeStamp()
	//如果时间间隔大于规定间隔，则PING一下，并把超时次数加一
	if timeNow-peer.LastPingTime > int64(server.PingInterval) {
		peer.Ping()
		peer.LastPingTime = GetTimeStamp()
		peer.TimeoutTime++
	}
	//如果超时次数大于4，则移除客户端
	if peer.TimeoutTime > 4 {
		Log2.Log.Warn("超时删除")
		peer.DisconnectHandle.Call(peer.Conv)
		delete(server.Peers, peer.Conv)
	}
}

//TCPUpdate
func (server *Server) connectUpdate() {
	listen, err := net.Listen("tcp", server.Ip+":"+strconv.Itoa(server.Port))
	if err != nil {
		Log2.Log.Info("TCP监听错误", err)
		return
	}
	for {
		conn, err := listen.Accept() // 建立连接
		if err != nil {
			Log2.Log.Info("TCP接收连接失败", err)
			continue
		}
		go server.kcpConnect(conn)
	}
}

//建立KCP连接
func (server *Server) kcpConnect(conn net.Conn) {
	defer conn.Close() // 关闭连接
	//达到最大连接数则返回
	if len(server.clients) > server.MaxConnections {
		Log2.Log.Info("已达到最大连接数")
		result := 0
		sendBytes := make([]byte, 4)
		sendBytes[0] = uint8(result)
		sendBytes[1] = uint8(result >> 8)
		sendBytes[2] = uint8(result >> 16)
		sendBytes[3] = uint8(result >> 24)
		_, err := conn.Write(sendBytes)
		if err != nil {
			return
		}
		return
	}
	remote, err := net.ResolveUDPAddr("udp", conn.RemoteAddr().String())
	if err != nil {
		panic("IP地址错误")
	}
	//判断是否map里存在这个remote
	isIn := false
	for _, value := range server.clients {
		if &value == remote {
			isIn = true
		}
	}
	if !isIn {
		//生成一个conv
		conv := server.GenerateConv()
		server.peerpool[conv-1000].Remote = *remote
		server.peerpool[conv-1000].InitKcp()
		server.Peers[conv] = server.peerpool[conv-1000]
		server.clients[conv] = *remote
		sendBytes := make([]byte, 4)
		sendBytes[0] = uint8(conv)
		sendBytes[1] = uint8(conv >> 8)
		sendBytes[2] = uint8(conv >> 16)
		sendBytes[3] = uint8(conv >> 24)
		_, err2 := conn.Write(sendBytes)
		if err2 != nil {
			Log2.Log.Warn("TCP发送报文失败", err)
			return
		}
		Log2.Log.Info("接受了一个连接请求", remote, " ", conv)
	} else {
		//客户端已经连接，则不再连接
		Log2.Log.Info("已经连接到服务器", remote)
	}
}

//更新接收信息
func (server *Server) update() {
	defer server.socket.Close()
	for {
		recvBuffer := make([]byte, 1024)
		count, _, err := server.socket.ReadFromUDP(recvBuffer)
		if count <= 0 {
			return
		}
		if err != nil {
			return
		}
		convBytes := recvBuffer[0:4]
		head := uint32(convBytes[0]) | uint32(convBytes[1])<<8 | uint32(convBytes[2])<<16 | uint32(convBytes[3])<<24
		//如果是收到的消息,传给Kcp
		if value, ok := server.Peers[head]; ok {
			value.Kcp.Input(recvBuffer, true, true)
		}
	}
}

// Send 发送消息
func (server *Server) Send(conv uint32, bytes []byte) {
	server.Peers[conv].Send(bytes)
}

//更新Peer
func (server *Server) updatePeer() {
	for {
		if len(server.Peers) <= 0 {
			continue
		}
		for _, peer := range server.Peers {
			server.CheckPing(*peer)
			peer.PeerUpdate()
		}
	}
}

//注册回调

// AddReceiveHandle 添加一个收到消息的回调
func (server *Server) AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	for _, peer := range server.peerpool {
		peer.ReceiveHandle.Add(name, handleFunc)
	}
}

// AddConnectHandle 添加一个连接回调
func (server *Server) AddConnectHandle(name string, handleFunc func(bytes []byte)) {
	for _, peer := range server.peerpool {
		peer.ConnectHandle.Add(name, handleFunc)
	}
}

// AddDisconnectHandle 添加一个断开连接回调
func (server *Server) AddDisconnectHandle(name string, handleFunc func(conv uint32)) {
	for _, peer := range server.peerpool {
		peer.DisconnectHandle.Add(name, handleFunc)
	}
}
