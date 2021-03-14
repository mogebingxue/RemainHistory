package Player

import (
	"ReaminHistory/Demo/Player/PlayerData"
	"ReaminHistory/YT/Net"
	"google.golang.org/protobuf/runtime/protoiface"
)

type Player struct {
	//id
	Id string
	//连接号
	Conv uint32
	//数据库数据
	Data *PlayerData.PlayerData
}

func NewPlayer(conv uint32) *Player {
	return &Player{Conv: conv}
}

//发送消息
func (player *Player) Send(msg protoiface.MessageV1) {
	Net.Clients[player.Conv].Send(msg)
}
