package Player

import (
	"ReaminHistory/YT"
	"google.golang.org/protobuf/runtime/protoiface"
)

type Player struct {
	//id
	Id string
	//指向ClientState
	Conv uint32
	//数据库数据
	Data *PlayerData
}

func NewPlayer(conv uint32 ) *Player {
	return &Player{Conv: conv}
}

//发送消息
func (player *Player) Send(msg protoiface.MessageV1) {
	YT.Clients[player.Conv].Send(msg)
}
