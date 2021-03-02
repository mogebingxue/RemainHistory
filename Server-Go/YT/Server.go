package YT

import (
	"ReaminHistory/Demo/Player"
	"ReaminHistory/Demo/db"
	MsgHelper "ReaminHistory/YT/Helper"
	"ReaminHistory/YT/Helper/ConfigHelper"
	"ReaminHistory/YT/TKcp"
	"fmt"
	"google.golang.org/protobuf/runtime/protoiface"
	"gopkg.in/eapache/queue.v1"
)

type Server struct {
	Clients  map[uint32]*Connection
	Routers  map[string]func(connection *Connection,bytes []byte)
	Requests queue.Queue
	//服务端名
	Name string
	//服务端ip地址
	ip string
	//服务端端口号
	port int
	//最大连接数
	maxClients int
	//服务器
	server *TKcp.Server
}

func NewServer() *Server {
	server := &Server{}
	netConfig := ConfigHelper.GetNetConfig()
	server.Name = netConfig.Name
	server.ip = netConfig.IP
	server.port = netConfig.Port
	server.maxClients = netConfig.MaxClients
	return server
}

//启动服务器
func (server *Server) Start() {
	fmt.Println("[START]Server Name: ",server.Name,", IP: ",server.ip,"Port: ",server.port)
	server.server = TKcp.NewServer()
	server.server.Ip = server.ip
	server.server.Port = server.port
	server.server.MaxConnections = server.maxClients
	//注册回到函数
	server.server.AddConnectHandle("OnConnect", func(bytes []byte) {
		server.OnConnect(bytes)
	})
	server.server.AddReceiveHandle("OnReceive", func(conv uint32, bytes []byte, len int) {
		server.OnReceive(conv,bytes,len)
	})
	server.server.AddDisconnectHandle("OnDisconnect", func(conv uint32) {
		server.OnDisconnect(conv)
	})
}

//处理消息回调的线程
func (server *Server) StartMsgHandle() {

	for{
		for server.Requests.Length()>0{
			request:=&Request{}

			//request:=Request(server.Requests.Remove())
			fmt.Println("Receive: ",request.Name)
			//注册回调应手动进行
			//TODO
			server.Routers[request.Name](server.Clients[request.Conv],request.Msg)

		}
	}
}

//关闭服务器
func (server *Server) Stop() {
	fmt.Println("[STOP]Server Name: ",server.Name,", IP: ",server.ip,"Port: ",server.port)
}

//为当前服务添加一个路由
func (server *Server) AddRouter(name string,handleFunc func(connection *Connection,bytes []byte)) {
	if server.Routers == nil{
		server.Routers = make(map[string]func(connection *Connection,bytes []byte))
	}
	server.Routers[name] = handleFunc
}

//为当前服务移除一个路由
func (server *Server) RemoveRouter(name string) {
	if _, ok := server.Routers[name]; ok {
		delete(server.Routers, name)
	}else {
		fmt.Printf("handle function <%s> does not exist\n", name)
	}
}

//客户端连接时，需要执行的方法
func (server *Server) OnConnect(bytes []byte) {
	conv:=uint32(bytes[0])|uint32(bytes[1])<<8|uint32(bytes[2])<<16|uint32(bytes[3])<<24
	fmt.Println("客户端连接: ",conv)
	connection:=&Connection{Conv: conv,Server: server}
	if _,ok:=server.Clients[conv];!ok{
		server.Clients[conv]=connection
	}
	go server.StartMsgHandle()
}

//客户端断开连接时，需要执行的方法
func (server *Server) OnDisconnect(conv uint32) {
	fmt.Println("客户端连接: ",conv)
	if client,ok:=server.Clients[conv];ok{
		//Player下线
		db.UpdatePlayerData(client.Player.Id,client.Player.Data)
		Player.RemovePlayer(client.Player.Id)
		delete(server.Clients,conv)
	}
}

//客户端接受消息时，需要执行的方法
func (server *Server) OnReceive(conv uint32, bytes []byte, len int) {
	if len<=4{
		fmt.Println("收到了pong")
		return
	}
	if client,ok:=server.Clients[conv];ok{
		readBuf:=client.readBuf
		readBuf.Write(bytes)
		server.onReceiveData(conv)
	}else {
		fmt.Println("客户端已断开连接: ",conv)
		return
	}

}

//数据处理
func (server *Server) onReceiveData(conv uint32) {
	readBuf:=server.Clients[conv].readBuf
	request:=MsgHelper.Decode(readBuf,conv)
	server.Requests.Add(request)
	if len(readBuf.Bytes)>2{
		server.onReceiveData(conv)
	}
}

//发送消息
func (server *Server) Send(conv uint32, message protoiface.MessageV1) {
	sendBytes:=MsgHelper.Encode(message)
	server.server.Send(conv,sendBytes)
}
