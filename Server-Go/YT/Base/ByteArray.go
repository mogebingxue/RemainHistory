package Base

type ByteArray struct {
	//缓冲区
	Bytes []byte
	//读写位置
	ReadIdx  int
	WriteIdx int
}

func NewByteArray() *ByteArray {
	byteArray := &ByteArray{ReadIdx: 0, WriteIdx: 1}
	byteArray.Bytes = make([]byte, 0)
	return byteArray
}

//写数据
func (byteArray *ByteArray) Write(bs []byte) int {
	length := int16(bs[1])*256 + int16(bs[0])
	bytes := bs[:length+2]
	if byteArray.Bytes == nil {
		byteArray.Bytes = make([]byte, 0)
	}
	byteArray.Bytes = append(byteArray.Bytes, bytes...)
	byteArray.WriteIdx += len(bytes)

	return len(byteArray.Bytes)
}

//读数据
func (byteArray *ByteArray) Read() []byte {
	if len(byteArray.Bytes) <= 2 {
		return nil
	}
	length := int16(byteArray.Bytes[byteArray.ReadIdx+1])*256 + int16(byteArray.Bytes[byteArray.ReadIdx])
	if len(byteArray.Bytes) < int(length) {

		return nil
	}
	byteArray.ReadIdx += 2
	bytes := byteArray.Bytes[byteArray.ReadIdx : byteArray.ReadIdx+int(length)]
	byteArray.ReadIdx += int(length)
	byteArray.WriteIdx = byteArray.WriteIdx - 2 - int(length)
	byteArray.Bytes = byteArray.Bytes[byteArray.ReadIdx:]
	byteArray.ReadIdx = 0

	return bytes
}

//打印缓冲区
func (byteArray *ByteArray) ToString() string {
	return string(byteArray.Bytes[byteArray.ReadIdx:])
}

//打印调试信息
func (byteArray *ByteArray) Debug() string {
	return string(rune(byteArray.ReadIdx)) + string(rune(byteArray.WriteIdx)) + string(byteArray.Bytes[:])
}
