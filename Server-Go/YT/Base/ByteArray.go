package Base

type ByteArray struct {
	//缓冲区
	Buffer chan []byte
}

func NewByteArray() *ByteArray {
	byteArray := &ByteArray{}
	byteArray.Buffer = make(chan []byte, 100)
	return byteArray
}

//写数据
func (byteArray *ByteArray) Write(bs []byte) int {
	length := int16(bs[1])*256 + int16(bs[0])
	bytes := bs[2 : length+2]
	if byteArray.Buffer == nil {
		byteArray.Buffer = make(chan []byte, 100)
	}
	byteArray.Buffer <- bytes
	return len(byteArray.Buffer)
}

//读数据
func (byteArray *ByteArray) Read() []byte {
	select {
	case bytes := <-byteArray.Buffer:
		return bytes
	default:
		return nil
	}
}

//长度
func (byteArray *ByteArray) Count() int {
	return len(byteArray.Buffer)
}
