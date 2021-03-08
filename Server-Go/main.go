package main

import (
	"ReaminHistory/Demo/Helper"
	"ReaminHistory/Demo/MsgHandler"
	"ReaminHistory/Demo/Player/PlayerManager"
	"ReaminHistory/Demo/db"
	"ReaminHistory/YT"
)

func main() {
	netInfo := Helper.GetNetConfig()
	db.Connect(netInfo.DBName, netInfo.DBURL)
	server:=YT.NewServer(netInfo.Name,netInfo.IP,netInfo.Port,netInfo.MaxClients)
	server.AddRouter("MsgLogin", MsgHandler.MsgLogin)
	server.AddRouter("MsgRegister", MsgHandler.MsgRegister)
	server.AddRouter("MsgGetPlayerIntroduction", MsgHandler.MsgGetPlayerIntroduction)
	server.AddRouter("MsgSavePlayerIntroduction", MsgHandler.MsgSavePlayerIntroduction)
	server.AddRouter("MsgGetHeadPhoto", MsgHandler.MsgGetHeadPhoto)
	server.AddRouter("MsgSaveHeadPhoto", MsgHandler.MsgSaveHeadPhoto)
	server.AddRouter("MsgSendMessageToWord", MsgHandler.MsgSendMessageToWord)
	server.AddRouter("MsgSendMessageToFriend", MsgHandler.MsgSendMessageToFriend)
	server.AddRouter("MsgGetFriendList", MsgHandler.MsgGetFriendList)
	server.AddRouter("MsgAddFriend", MsgHandler.MsgAddFriend)
	server.AddRouter("MsgDeleteFriend", MsgHandler.MsgDeleteFriend)
	server.AddRouter("MsgAcceptAddFriend", MsgHandler.MsgAcceptAddFriend)
	server.AddRouter("MsgAddFriend", MsgHandler.MsgAddFriend)
	server.Start()
	server.AddDisconnectHandle("OnPlayerDisconnect",PlayerManager.OnPlayerDisconnect)
	for{
		
	}
}
