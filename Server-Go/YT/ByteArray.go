package YT

type ByteArray struct {
	//缓冲区
	Bytes []byte
	//读写位置
	ReadIdx  int
	WriteIdx int
}

func NewByteArray() *ByteArray {
	byteArray := &ByteArray{ReadIdx: 0, WriteIdx: 0}
	byteArray.Bytes = make([]byte, 0)
	return byteArray
}

//写数据
func (byteArray *ByteArray) Write(bs []byte) int {
	byteArray.Bytes = append(byteArray.Bytes, bs...)
	return len(byteArray.Bytes)
}

//读数据
func (byteArray *ByteArray) Read() []byte {
	if len(byteArray.Bytes) <= 2 {
		return nil
	}
	length := int16((byteArray.Bytes[byteArray.ReadIdx+1] << 8) | byteArray.Bytes[byteArray.ReadIdx])
	if len(byteArray.Bytes) < int(length) {
		return nil
	}
	bytes := byteArray.Bytes[byteArray.ReadIdx+2 : byteArray.ReadIdx+2+int(length)]
	byteArray.checkAndMoveBytes()
	return bytes
}

//移动并检查数据
func (byteArray *ByteArray) checkAndMoveBytes() {
	if len(byteArray.Bytes) < 8 {
		byteArray.moveBytes()
	}
}

//移动数据
func (byteArray *ByteArray) moveBytes() {
	byteArray.Bytes = byteArray.Bytes[byteArray.ReadIdx:]
}

//打印缓冲区
func (byteArray *ByteArray) ToString() string {
	return string(byteArray.Bytes[byteArray.ReadIdx:])
}

//打印调试信息
func (byteArray *ByteArray) Debug() string {
	return string(rune(byteArray.ReadIdx)) + string(rune(byteArray.WriteIdx)) + string(byteArray.Bytes[:])
}
