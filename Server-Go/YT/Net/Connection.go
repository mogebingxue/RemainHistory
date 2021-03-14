package Net

import (
	"ReaminHistory/YT/Base"
	"google.golang.org/protobuf/runtime/protoiface"
)

type Connection struct {
	//客户端Peer
	Conv uint32
	//缓存区
	readBuf *Base.ByteArray
	Server  *Server
}

func NewConnection(conv uint32) *Connection {
	connection := new(Connection)
	connection.Conv = conv
	connection.readBuf = new(Base.ByteArray)
	return connection
}

func (connection *Connection) Send(message protoiface.MessageV1) {
	connection.Server.Send(connection.Conv, message)
}
