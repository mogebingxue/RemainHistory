package PlayerManager

import (
	"ReaminHistory/Demo/DB"
	Player2 "ReaminHistory/Demo/Player/Player"
	"google.golang.org/protobuf/runtime/protoiface"
)

var Players map[uint32]*Player2.Player

func OnPlayerDisconnect(conv uint32) {
	if player, ok := Players[conv]; ok {
		//Player下线
		DB.UpdatePlayerData(player.Id, player.Data)
		RemovePlayer(player.Id)
		delete(Players, conv)
	}
}

//玩家列表
var players map[string]*Player2.Player

//玩家是否在线
func IsOnline(id string) bool {
	_, ok := players[id]
	return ok
}

//获取玩家
func GetPlayer(id string) *Player2.Player {
	if player, ok := players[id]; ok {
		return player
	} else {
		return nil
	}
}

//添加玩家
func AddPlayer(id string, player *Player2.Player) {
	if players == nil {
		players = make(map[string]*Player2.Player)
	}
	players[id] = player
}

//移除玩家
func RemovePlayer(id string) {
	delete(players, id)
}

//广播给所有玩家
func Broadcast(message protoiface.MessageV1) {
	for _, player := range Players {
		player.Send(message)
	}
}
