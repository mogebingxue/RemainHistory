package IProto

type Peer interface {
	// Send 发送数据
	Send(bytes []byte)
	// PeerUpdate 更新收到消息
	PeerUpdate()
	Ping()
	Pong()
}

type Server interface {
	Start()
	Send(conv uint32, bytes []byte)
	AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int))
	AddConnectHandle(name string, handleFunc func(bytes []byte))
	AddDisconnectHandle(name string, handleFunc func(conv uint32))
}

type Client interface {
	Connect(server string)
	Send(sendBytes []byte)
	AddReceiveHandle(name string, handleFunc func(conv uint32, bytes []byte, len int))
	AddAcceptHandle(name string, handleFunc func(bytes []byte, len int))
	AddTimeoutHandle(name string, handleFunc func())
}
