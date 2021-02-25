package MsgHelper

import (
	"github.com/golang/protobuf/proto"
	"google.golang.org/protobuf/runtime/protoiface"
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
	return nil
}

func Decode() {

}

func decodeName() {

}

func decodeBody() {

}
