package Handler

import (
	"ReaminHistory/Demo/Player/PlayerManager"
	"ReaminHistory/Demo/Proto"
	"ReaminHistory/Demo/db"
	"ReaminHistory/YT"
	"fmt"
	"github.com/golang/protobuf/proto"
)

//获取个人简介内容
func MsgGetPlayerIntroduction(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgGetPlayerIntroduction{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if player, ok := PlayerManager.Players[c.Conv]; ok {
		msg.PalyerIntroduction = player.Data.PlayerIntroduction
		player.Send(msg)
	} else {
		return
	}
}

//保存个人简介内容
func MsgSavePlayerIntroduction(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgSavePlayerIntroduction{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if player, ok := PlayerManager.Players[c.Conv]; ok {
		player.Data.PlayerIntroduction = msg.PalyerIntroduction
		db.UpdatePlayerData(player.Id, player.Data)
		player.Send(msg)
	} else {
		return
	}
}

//获取头像
func MsgGetHeadPhoto(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgGetHeadPhoto{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if player, ok := PlayerManager.Players[c.Conv]; ok {
		msg.HeadPhoto = player.Data.HeadPhoto
		player.Send(msg)
	} else {
		return
	}
}

//保存头像
func MsgSaveHeadPhoto(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgSaveHeadPhoto{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if player, ok := PlayerManager.Players[c.Conv]; ok {
		player.Data.HeadPhoto = msg.HeadPhoto
		db.UpdatePlayerData(player.Id, player.Data)
		player.Send(msg)
	} else {
		return
	}
}

//发送消息到世界
func MsgSendMessageToWord(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgSendMessageToWord{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if _, ok := PlayerManager.Players[c.Conv]; ok {
		msg.Result = 0
		PlayerManager.Broadcast(msg)
	} else {
		return
	}
}

//发送消息到好友
func MsgSendMessageToFriend(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgSendMessageToFriend{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	//TODO
}

//获取好友列表
func MsgGetFriendList(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgGetFriendList{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if player, ok := PlayerManager.Players[c.Conv]; ok {
		msg.FriendList = db.GetFriendList(player.Id)
		msg.Result = 0
		player.Send(msg)
	} else {
		return
	}
}

//添加好友
func MsgAddFriend(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgAddFriend{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if player, ok := PlayerManager.Players[c.Conv]; ok {
		if db.IsAccountExist(msg.FriendId) {
			msg.Result = 1
			player.Send(msg)
			return
		}
		if db.IsFriendExist(msg.Id, msg.FriendId) {
			msg.Result = 2
			player.Send(msg)
			return
		}
		if PlayerManager.GetPlayer(msg.FriendId) == nil {
			msg.Result = 3
			player.Send(msg)
			return
		}
		msg.Result = 0
		player.Send(msg)
		PlayerManager.GetPlayer(msg.FriendId).Send(msg)
	} else {
		return
	}
}

//删除好友
func MsgDeleteFriend(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgDeleteFriend{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if player, ok := PlayerManager.Players[c.Conv]; ok {
		if db.DeleteFriend(msg.Id, msg.FriendId) {
			msg.Result = 0
			player.Send(msg)
			if PlayerManager.GetPlayer(msg.FriendId) != nil {
				PlayerManager.GetPlayer(msg.FriendId).Send(msg)
			}
		} else {
			msg.Result = 1
			player.Send(msg)
		}
	} else {
		return
	}
}

//同意添加好友
func MsgAcceptAddFriend(c *YT.Connection, bytes []byte) {
	msg := &Proto.MsgAcceptAddFriend{}
	err := proto.Unmarshal(bytes, msg)
	if err != nil {
		fmt.Println("解析失败")
		return
	}
	if player, ok := PlayerManager.Players[c.Conv]; ok {
		if db.AddFriend(msg.Id, msg.FriendId) {
			msg.Result = 0
		} else {
			msg.Result = 1
		}
		player.Send(msg)
		PlayerManager.GetPlayer(msg.FriendId).Send(msg)
	} else {
		return
	}
}
