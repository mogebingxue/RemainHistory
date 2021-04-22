package TCP

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
	//当前连接的Peer
	Peers map[uint32]*peer
	//对象池
	peerpool []*peer
	//服务端tcp监听
	listen *net.TCPListener
}

func NewServer() *Server {
	server := &Server{MaxConnections: 999, Ip: "127.0.0.1", Port: 8888, PingInterval: 500}
	//初始化缓存池
	server.peerpool = make([]*peer, server.MaxConnections)
	for key, _ := range server.peerpool {
		server.peerpool[key] = newPeer(net.TCPAddr{}, uint32(key+1000))
	}
	server.Peers = make(map[uint32]*peer)
	return server
}

// Start 开启服务器
func (server *Server) Start() {

	localAddr, err := net.ResolveTCPAddr("tcp", server.Ip+":"+strconv.Itoa(server.Port))
	if err != nil {
		Log2.Log.Info("解析地址失败")
	}
	//监听TCP连接
	listen, err := net.ListenTCP("tcp", localAddr)
	if err != nil {
		Log2.Log.Info("TCP监听错误", err)
		return
	}
	server.listen = listen
	for {
		conn, err := listen.AcceptTCP() // 建立连接
		if err != nil {
			Log2.Log.Info("TCP接收连接失败", err)
			continue
		}

		go server.onConnect(conn)
	}
}

func (server *Server) onConnect(conn *net.TCPConn) {
	//达到最大连接数则返回
	if len(server.Peers) > server.MaxConnections {
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
		_ = conn.Close()
		return
	}
	remote, err := net.ResolveTCPAddr("tcp", conn.RemoteAddr().String())
	if err != nil {
		Log2.Log.Info("解析地址失败")
	}
	//生成一个conv
	conv := server.generateConv()
	server.Peers[conv] = server.peerpool[conv-1000]
	server.Peers[conv].Remote = *remote
	server.Peers[conv].conn = conn
	server.Peers[conv].TimeoutTime = server.PingInterval
	convBytes := make([]byte, 4)
	convBytes[0] = uint8(conv)
	convBytes[1] = uint8(conv >> 8)
	convBytes[2] = uint8(conv >> 16)
	convBytes[3] = uint8(conv >> 24)
	server.Peers[conv].ConnectHandle.Call(convBytes)
	Log2.Log.Info("接受了一个连接请求", remote, " ", conv)
}

// GenerateConv 连接号生成器
func (server *Server) generateConv() uint32 {
	rand.Seed(time.Now().Unix())
	conv := rand.Intn(server.MaxConnections) + 1000
	return uint32(conv)
}

// CheckPing 检查PING
func (server *Server) CheckPing(peer peer) {
	//获取当前时间戳
	timeNow := getTimeStamp()
	//如果时间间隔大于规定间隔，则PING一下，并把超时次数加一
	if timeNow-peer.LastPingTime > int64(server.PingInterval) {
		peer.Ping()
		peer.LastPingTime = getTimeStamp()
		peer.TimeoutTime++
	}
	//如果超时次数大于4，则移除客户端
	if peer.TimeoutTime > 4 {
		Log2.Log.Warn("超时删除")
		peer.DisconnectHandle.Call(peer.Conv)
		delete(server.Peers, peer.Conv)
	}
}

// Send 发送消息
func (server *Server) Send(conv uint32, bytes []byte) {
	server.Peers[conv].Send(bytes)
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
