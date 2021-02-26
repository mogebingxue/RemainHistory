package MsgHelper

import (
	"ReaminHistory/YT"
	"fmt"
	"github.com/golang/protobuf/proto"
	"google.golang.org/protobuf/runtime/protoiface"
	"reflect"
)

//编码
func Encode(msg protoiface.MessageV1) []byte {
	nameBytes := encodeName(msg)
	bodyBytes := encodeBody(msg)
	//组装长度
	length := len(nameBytes) + len(bodyBytes)
	sendBytes := make([]byte, 2)
	sendBytes[0] = byte(length % 256)
	sendBytes[1] = byte(length / 256)
	//组装协议名
	sendBytes = append(sendBytes, nameBytes...)
	//组装协议体
	sendBytes = append(sendBytes, bodyBytes...)
	return sendBytes
}

func encodeBody(msg protoiface.MessageV1) []byte {
	buffer, _ := proto.Marshal(msg)
	return buffer
}

func encodeName(msg protoiface.MessageV1) []byte {
	name := reflect.TypeOf(msg).Name()
	sendBytes := make([]byte, 2)
	sendBytes[0] = byte(len(name) % 256)
	sendBytes[1] = byte(len(name) / 256)
	sendBytes = append(sendBytes, []byte(name)...)
	return sendBytes
}

func Decode(readBuf YT.ByteArray, conv uint32) *YT.Request {
	bytes := readBuf.Read()
	//解析协议名
	protoName, nameCount := decodeName(bytes)
	if protoName == "" {
		fmt.Println("OnReceiveData MsgBase.DecodeName fail")
		return &YT.Request{}
	}
	//解析协议体
	if len(bytes) < nameCount {
		fmt.Println("OnReceiveData fail, bodyCount <0")
		return &YT.Request{}
	}
	bytes = bytes[nameCount:]
	msg := decodeBody(bytes)
	request := &YT.Request{Conv: conv, Name: protoName, Msg: msg}
	return request
}

func decodeName(bytes []byte) (name string, count int) {
	//必须大于两字节
	if len(bytes) < 2 {
		return "", 0
	}
	//读取长度
	length := int16((bytes[1] << 8) | bytes[0])
	if length < 0 {
		return "", 0
	}
	//长度必须足够
	if int(length)+2 > len(bytes) {
		return "", 0
	}
	name = string(bytes[2:])
	return name, int(length)
}

func decodeBody(bytes []byte) []byte {
	return bytes
}
