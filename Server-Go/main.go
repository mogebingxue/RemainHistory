package main

import (
	"ReaminHistory/Demo/db"
	"ReaminHistory/YT/Helper/ConfigHelper"
	"fmt"
	"gopkg.in/eapache/queue.v1"
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
	queue:= queue.New()
	queue.Add("姚姚姚")
	i:=queue.Remove()
	fmt.Println(i)

}
