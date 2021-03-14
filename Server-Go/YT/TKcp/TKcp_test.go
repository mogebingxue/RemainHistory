package TKcp

import (
	"fmt"
	"testing"
	"time"
)

func TestTKcp(t *testing.T) {
	client := NewClient()
	client.AddReceiveHandle("b", b)
	server := NewServer()
	server.AddReceiveHandle("a", a)
	time.Sleep(1 * time.Second)
	client.Connect("127.0.0.1:8888")
	time.Sleep(1 * time.Second)
	bytes := []byte("姚姚姚")
	client.Send(bytes)
	time.Sleep(1 * time.Second)
	server.Send(client.Peer.Conv, bytes)
	select {}

}

func b(conv uint32, bytes []byte, l int) {
	fmt.Println("客户端收到了: ", string(bytes))
}

func a(conv uint32, bytes []byte, l int) {
	fmt.Println("服务端收到了: ", string(bytes))
}
