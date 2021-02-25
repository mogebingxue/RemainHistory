package TKcp

import (
	"fmt"
	"math/rand"
	"net"
	"strconv"
	"time"
)

type Server struct {
	MaxConnection int
	Ip string
	Port int
	PingInterval int
	Peers map[uint32]*Peer

	clients map[uint32]net.UDPAddr
	peerpool []*Peer
	//服务端udp
	socket net.UDPConn
	localIpep net.UDPAddr
}

func NewServer() *Server {

	server :=&Server{MaxConnection: 999, Ip: "127.0.0.1", Port: 8888, PingInterval: 500}
    server.initServer()
	return server
}

func (server *Server) initServer()  {
	addr,err1 := net.ResolveUDPAddr("udp",server.Ip + ":" + strconv.Itoa(server.Port))
	if err1 != nil{
		panic("IP地址错误")
	}
	server.localIpep = *addr
	conn, err2 := net.ListenUDP("udp", addr)
	if err2 != nil {
		panic("监听UDP失败")
	}
	server.socket = *conn
	server.Peers = make(map[uint32]*Peer)
	server.clients = make(map[uint32]net.UDPAddr)
	//初始化缓存池
	server.peerpool = make([]*Peer,server.MaxConnection)
	for key,_:=range server.peerpool{
		server.peerpool[key] = NewPeer(server.socket,net.UDPAddr{},uint32(key+1000))
	}

	go server.update()
	go server.updatePeer()
}

//连接号生成器
func (server *Server) GenerateConv() uint32 {
	rand.Seed(time.Now().Unix())
	conv:=rand.Intn(server.MaxConnection)+1000
	return uint32(conv)
}


func (server *Server) CheckPing(peer Peer) {
	timeNow:=GetTimeStamp()
	if timeNow-peer.LastPingTime > int64(server.PingInterval) {
		peer.Ping()
		peer.LastPingTime = GetTimeStamp()
		peer.TimeoutTime++
	}
	if peer.TimeoutTime>4 {
		fmt.Println("超时删除")
		peer.DisconnectHandle.Call(peer.Conv)
		delete(server.Peers, peer.Conv)
	}
}

//更新接收信息
func (server *Server) update() {
	for{
		recvBuffer := make([]byte, 1472)
		count, remote, err := server.socket.ReadFromUDP(recvBuffer)
		if count<=0{
			return
		}
		if err!=nil{
			return
		}
		convBytes:=recvBuffer[0:4]
		head:=uint32(convBytes[0])|uint32(convBytes[1])<<8|uint32(convBytes[2])<<16|uint32(convBytes[3])<<24
		if head==0{
			if len(server.clients)>server.MaxConnection {
				fmt.Println("已达到最大连接数")
				continue
			}
			//判断是否map里存在这个remote
			isIn:=false
			for _,value:=range server.clients{
				if &value==remote {
					isIn = true
				}
			}
			if !isIn {
				//生成一个conv
				conv:=server.GenerateConv()
				server.peerpool[conv-1000].Remote = *remote
				server.peerpool[conv-1000].InitKcp()
				server.Peers[conv] = server.peerpool[conv-1000]
				server.clients[conv] = *remote
				sendBytes:=make([]byte,4)
				sendBytes[0] = uint8(conv)
				sendBytes[1] = uint8(conv>>8)
				sendBytes[2] = uint8(conv>>16)
				sendBytes[3] = uint8(conv>>24)
				server.peerpool[conv-1000].ConnectHandle.Call(sendBytes)
				fmt.Println("接受了一个连接请求",remote," ",conv)
			} else {
				//客户端已经连接，则不再连接
				fmt.Println("已经连接到服务器",remote)
			}
		}else {
			//如果是收到的消息
			if value,ok:=server.Peers[head]; ok {
				value.Kcp.Input(recvBuffer,true,true)
			}
		}
	}
}

//发送消息
func (server *Server) Send(conv uint32,bytes []byte) {
	server.Peers[conv].Send(bytes)
}

//更新Peer
func (server *Server) updatePeer() {
	for {
		if len(server.Peers)<=0 {
			continue
		}
		for _,peer:= range server.Peers{
			server.CheckPing(*peer)
			peer.PeerUpdate()
		}
	}
}

//注册回调

func (server *Server) AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	for _,peer:=range server.peerpool{
		peer.ReceiveHandle.Add(name,handleFunc)
	}
}

func (server *Server) AddConnectHandle(name string, handleFunc func(bytes []byte)) {
	for _,peer:=range server.peerpool{
		peer.ConnectHandle.Add(name,handleFunc)
	}
}

func (server *Server) AddDisconnectHandle(name string, handleFunc func(conv uint32)) {
	for _,peer:=range server.peerpool{
		peer.DisconnectHandle.Add(name,handleFunc)
	}
}