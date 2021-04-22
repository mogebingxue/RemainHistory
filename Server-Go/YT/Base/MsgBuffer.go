package Base

// MsgBuffer 消息缓冲
type MsgBuffer struct {
	//缓冲区
	Buffer chan []byte
}

func NewMsgBuffer() *MsgBuffer {
	buffer := &MsgBuffer{}
	buffer.Buffer = make(chan []byte, 100)
	return buffer
}

//写数据
func (buffer *MsgBuffer) Write(bs []byte) int {
	length := int16(bs[1])<<8 | int16(bs[0])
	bytes := bs[2 : length+2]
	if buffer.Buffer == nil {
		buffer.Buffer = make(chan []byte, 100)
	}
	buffer.Buffer <- bytes
	return len(buffer.Buffer)
}

//读数据
func (buffer *MsgBuffer) Read() []byte {
	select {
	case bytes := <-buffer.Buffer:
		return bytes
	default:
		return nil
	}
}

// Count 长度
func (buffer *MsgBuffer) Count() int {
	return len(buffer.Buffer)
}
