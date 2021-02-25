package main

import (
	"ReaminHistory/Demo/db"
	"ReaminHistory/YT/Helper/ConfigHelper"
	"ReaminHistory/YT/TKcp"
	"fmt"
)

func main() {
	netInfo:=ConfigHelper.GetNetConfig()
	db.Connect(netInfo.DBName,netInfo.DBURL)
	/*
	a := &Proto.MsgGetPlayerIntroduction{}
	a.PalyerIntroduction = "aaasd"
	buffer, _ := proto.Marshal(a)
	fmt.Println("序列化之后的信息为：",buffer)
	data:= &Proto.MsgGetPlayerIntroduction{}
	proto.Unmarshal(buffer,data)
	fmt.Println("反序列化之后的信息为：",data)
	 */
	server:=TKcp.NewServer()
	server.AddReceiveHandle("test",test)
	client:=TKcp.NewClient()
	client.Connect("127.0.0.1:8888")
	var input string
	fmt.Scanln(&input)
	client.Send([]byte("姚姚姚"))
	for{

	}
}

func test(conv uint32, bytes []byte, l int) {
	fmt.Println("注册了一个回调")
	fmt.Println(string(bytes))
}
