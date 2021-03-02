package YT

import (
	"google.golang.org/protobuf/runtime/protoiface"
)

type Connection struct {
	//客户端Peer
	Conv uint32
	//缓存区
	readBuf *ByteArray
	Server  *Server
}

func NewConnection(conv uint32) *Connection {
	return &Connection{Conv: conv}
}

func (connection *Connection) Send(message protoiface.MessageV1) {
	connection.Server.Send(connection.Conv, message)
}
