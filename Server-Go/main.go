package main

import (
	"ReaminHistory/Demo/Helper/ConfigHelper"
	"ReaminHistory/Demo/db"
)

func main() {
	netInfo := ConfigHelper.GetNetConfig()
	db.Connect(netInfo.DBName, netInfo.DBURL)

	//a := &Proto.MsgGetPlayerIntroduction{}
	//a.PlayerIntroduction = "aaasd"
	//buffer, _ := proto.Marshal(a)
	//fmt.Println("序列化之后的信息为：", buffer)
	//data := &Proto.MsgGetPlayerIntroduction{}
	//_ = proto.Unmarshal(buffer, data)
	//fmt.Println("反序列化之后的信息为：", data)
	//fmt.Println(reflect.TypeOf(data))

}
