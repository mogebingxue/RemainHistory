package MsgHandler

import (
	"ReaminHistory/Demo/DB"
	Player2 "ReaminHistory/Demo/Player/Player"
	"ReaminHistory/Demo/Player/PlayerManager"
	"ReaminHistory/Demo/Proto"
	"fmt"
	"github.com/golang/protobuf/proto"
	"github.com/mogebingxue/YTFramework/Net"
)

//注册协议处理
func MsgRegister(c *Net.Connection, bytes []byte) {
	msg := &Proto.MsgRegister{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if DB.Register(msg.Id, msg.Pw) {
		DB.CreatePlayer(msg.Id)
		msg.Result = 0
	} else {
		msg.Result = 1
	}
	fmt.Println()
	c.Send(msg)
}

//登陆协议处理
func MsgLogin(c *Net.Connection, bytes []byte) {
	msg := &Proto.MsgLogin{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	//密码校验
	if !DB.CheckPassword(msg.Id, msg.Pw) {
		msg.Result = 1
		c.Send(msg)
		return
	}
	//不允许再次登录
	if _, ok := PlayerManager.Players[c.Conv]; ok {
		msg.Result = 1
		c.Send(msg)
		return
	}
	//如果已经登陆，踢下线
	if PlayerManager.IsOnline(msg.Id) {
		msgKick := &Proto.MsgKick{}
		msgKick.Result = 0
		PlayerManager.GetPlayer(msg.Id).Send(msgKick)
	}
	playerData := DB.GetPlayerData(msg.Id)
	if playerData == nil {
		msg.Result = 1
		c.Send(msg)
		return
	}
	player := Player2.NewPlayer(c.Conv)
	player.Id = msg.Id
	player.Data = playerData
	PlayerManager.AddPlayer(msg.Id, player)
	if PlayerManager.Players == nil {
		PlayerManager.Players = make(map[uint32]*Player2.Player)
	}
	PlayerManager.Players[c.Conv] = player
	msg.Result = 0
	fmt.Println("id:", msg.Id, "pw:", msg.Pw, "result:", msg.Result)
	player.Send(msg)
}
