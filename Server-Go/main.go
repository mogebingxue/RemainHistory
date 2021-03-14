package main

import (
	"ReaminHistory/Demo/DB"
	"ReaminHistory/Demo/MsgHandler"
	"ReaminHistory/Demo/Player/PlayerManager"
	"ReaminHistory/YT/Net"
	"ReaminHistory/YT/Util"
)

func main() {
	netInfo := Util.GetNetConfig("./Demo/Config/NetConfig.json")
	DB.Connect(netInfo.DBName, netInfo.DBURL)
	server := Net.NewServer(netInfo.Name, netInfo.IP, netInfo.Port, netInfo.MaxClients)
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
	server.AddDisconnectHandle("OnPlayerDisconnect", PlayerManager.OnPlayerDisconnect)
	select {}
}
