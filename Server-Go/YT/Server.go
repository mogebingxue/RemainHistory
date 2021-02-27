package YT

import (
	"ReaminHistory/YT/Helper/ConfigHelper"
	"ReaminHistory/YT/TKcp"
	"google.golang.org/protobuf/runtime/protoiface"
	"gopkg.in/eapache/queue.v1"
)

type Server struct {
	Clients  map[uint32]Connection
	Routers  map[string]func()
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

}

//处理消息回调的线程
func (server *Server) StartMsgHandle() {

}

//关闭服务器
func (server *Server) Stop() {

}

//为当前服务添加一个路由
func (server *Server) AddRouter() {

}

//为当前服务移除一个路由
func (server *Server) RemoveRouter() {

}

//客户端连接时，需要执行的方法
func (server *Server) OnConnect() {

}

//客户端断开连接时，需要执行的方法
func (server *Server) OnDisconnect() {

}

//客户端接受消息时，需要执行的方法
func (server *Server) OnReceive() {

}

//数据处理
func (server *Server) onReceiveData() {

}

//发送消息
func (server *Server) Send(conv uint32, message protoiface.MessageV1) {

}
