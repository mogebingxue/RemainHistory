package main

import (
	"ReaminHistory/YT/TKcp"
	"bufio"
	"fmt"
	"os"
	"time"
)

func main() {
	go testServer()
	time.Sleep(2*time.Second)
	go testClient()
	select {}
}

func testServer()  {
	server := TKcp.NewServer()
	server.AddReceiveHandle("onServerReceive", func(conv uint32, bytes []byte, len int) {
		fmt.Println("服务端收到了: ", string(bytes))
		server.Send(conv,bytes)
	})
	select {}
}

func testClient()  {
	client := TKcp.NewClient()
	client.AddReceiveHandle("onClientReceive", func(conv uint32, bytes []byte, len int) {
		fmt.Println("客户端收到了: ", string(bytes))
	})
	client.Connect("127.0.0.1:8888")
	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan(){
		bytes := []byte(scanner.Text())
		client.Send(bytes)
	}
}
