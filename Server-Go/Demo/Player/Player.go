package Player

import "ReaminHistory/YT"

type Player struct {
	//id
	Id string
	//指向ClientState
	State *YT.Connection
	//数据库数据
	Data PlayerData
}

func NewPlayer(state *YT.Connection) *Player {
	return &Player{State: state}
}

//发送消息
func (player *Player) Send() {
	//TODO
}
