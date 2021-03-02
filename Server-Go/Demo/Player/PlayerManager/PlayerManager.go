package PlayerManager

import (
	"ReaminHistory/Demo/Player"
	"ReaminHistory/Demo/db"
)

var Players map[uint32]*Player.Player

func RemovePlayer(id string)  {
	
}

func OnPlayerDisconnect(conv uint32)  {
	if player,ok:= Players[conv];ok{
		//Player下线
		db.UpdatePlayerData(player.Id,player.Data)
		RemovePlayer(player.Id)
		delete(Players,conv)
	}
}