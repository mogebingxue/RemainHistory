package Net

import (
	"ReaminHistory/YT/Base"
	"ReaminHistory/YT/Log"
	"ReaminHistory/YT/TKcp"
	"ReaminHistory/YT/Util"
	"google.golang.org/protobuf/runtime/protoiface"
)

var Clients map[uint32]*Connection

type Server struct {
	Routers  map[string]func(connection *Connection, bytes []byte)
	Requests *Base.Queue
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

func NewServer(name string, ip string, port int, maxClients int) *Server {
	server := &Server{}
	server.Name = name
	server.ip = ip
	server.port = port
	server.maxClients = maxClients
	server.Requests = Base.NewQueue()
	server.Routers = make(map[string]func(connection *Connection, bytes []byte))
	return server
}

// Start 启动服务器
func (server *Server) Start() {
	Log.Log.Info("[START]Server Name: ", server.Name, ", IP: ", server.ip, ", Port: ", server.port)
	server.server = TKcp.NewServer()
	server.server.Ip = server.ip
	server.server.Port = server.port
	server.server.MaxConnections = server.maxClients
	//注册回到函数
	server.server.AddConnectHandle("OnConnect", func(bytes []byte) {
		server.OnConnect(bytes)
	})
	server.server.AddReceiveHandle("OnReceive", func(conv uint32, bytes []byte, len int) {
		server.OnReceive(conv, bytes, len)
	})
	server.server.AddDisconnectHandle("OnDisconnect", func(conv uint32) {
		server.OnDisconnect(conv)
	})
	go server.msgHandle()
}

//处理消息回调的线程
func (server *Server) msgHandle() {
	for {
		if v := server.Requests.Dequeue(); v != nil {
			if request, ok := v.(*Base.Request); ok {
				Log.Log.Info("Receive:", request.Name)
				if server.Routers == nil {
					server.Routers = make(map[string]func(connection *Connection, bytes []byte))
				}
				if router, ok := server.Routers[request.Name]; ok {
					router(Clients[request.Conv], request.Msg)
				}
			}
		}
	}
}

// Stop 关闭服务器
func (server *Server) Stop() {
	Log.Log.Info("[STOP]Server Name: ", server.Name, ", IP: ", server.ip, "Port: ", server.port)
}

// AddRouter 为当前服务添加一个路由
func (server *Server) AddRouter(name string, handleFunc func(connection *Connection, bytes []byte)) {
	if server.Routers == nil {
		server.Routers = make(map[string]func(connection *Connection, bytes []byte))
	}
	server.Routers[name] = handleFunc
}

// RemoveRouter 为当前服务移除一个路由
func (server *Server) RemoveRouter(name string) {
	if _, ok := server.Routers[name]; ok {
		delete(server.Routers, name)
	} else {
		Log.Log.Info("handle function <%s> does not exist\n", name)
	}
}

// OnConnect 客户端连接时，需要执行的方法
func (server *Server) OnConnect(bytes []byte) {
	conv := uint32(bytes[0]) | uint32(bytes[1])<<8 | uint32(bytes[2])<<16 | uint32(bytes[3])<<24

	connection := NewConnection(conv)
	connection.Server = server
	if Clients == nil {
		Clients = make(map[uint32]*Connection)
	}
	if _, ok := Clients[conv]; !ok {
		Clients[conv] = connection
	}
	Log.Log.Info("客户端连接:", "Conv:", conv)
}

// OnDisconnect 客户端断开连接时，需要执行的方法
func (server *Server) OnDisconnect(conv uint32) {
	Log.Log.Info("客户端断开连接: ", conv)
	if _, ok := Clients[conv]; ok {

		delete(Clients, conv)
	}
}

// OnReceive 客户端接受消息时，需要执行的方法
func (server *Server) OnReceive(conv uint32, bytes []byte, len int) {
	if len <= 4 {
		Log.Log.Info("收到了pong")
		return
	}
	if client, ok := Clients[conv]; ok {
		readBuf := client.readBuf
		readBuf.Write(bytes)
		server.onReceiveData(conv)
	} else {
		Log.Log.Info("客户端已断开连接: ", conv)
		return
	}

}

//数据处理
func (server *Server) onReceiveData(conv uint32) {
	readBuf := Clients[conv].readBuf
	bytes := readBuf.Read()
	for bytes != nil {
		request := Util.Decode(bytes, conv)
		server.Requests.Enqueue(request)
		bytes = readBuf.Read()
	}
}

//发送消息
func (server *Server) Send(conv uint32, message protoiface.MessageV1) {
	sendBytes := Util.Encode(message)
	server.server.Send(conv, sendBytes)
}

//广播消息
func (server *Server) Broadcast(message protoiface.MessageV1) {
	for _, connection := range Clients {
		connection.Send(message)
	}
}

func (server *Server) AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	server.server.AddReceiveHandle(name, handleFunc)
}
func (server *Server) AddConnectHandle(name string, handleFunc func(bytes []byte)) {
	server.server.AddConnectHandle(name, handleFunc)
}
func (server *Server) AddDisconnectHandle(name string, handleFunc func(conv uint32)) {
	server.server.AddDisconnectHandle(name, handleFunc)
}
