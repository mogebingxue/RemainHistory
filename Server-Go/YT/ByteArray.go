package YT

type ByteArray struct {
	//默认大小
	DEFAULT_SIZE int
	//初始大小
	initSize int
	//缓冲区
	Bytes []byte
	//读写位置
	ReadIdx  int
	WriteIdx int
	//容量
	capacity int
}

func NewByteArray() *ByteArray {
	byteArray := &ByteArray{DEFAULT_SIZE: 1024, initSize: 0, ReadIdx: 0, WriteIdx: 0, capacity: 0}
	byteArray.Bytes = make([]byte, byteArray.DEFAULT_SIZE)
	byteArray.capacity = byteArray.DEFAULT_SIZE
	byteArray.initSize = byteArray.DEFAULT_SIZE
	return byteArray
}

//剩余空间
func (byteArray *ByteArray) Remain() int {
	return byteArray.capacity - byteArray.WriteIdx
}

//数据长度
func (byteArray *ByteArray) Length() int {
	return byteArray.WriteIdx - byteArray.ReadIdx
}

//重设尺寸
func (byteArray *ByteArray) ReSize(size int) {

}

//写数据
func (byteArray *ByteArray) Write(bs []byte, offset int, count int) {

}

//读数据
func (byteArray *ByteArray) Read(bs []byte, offset int, count int) {

}

//移动并检查数据
func (byteArray *ByteArray) CheckAndMoveBytes() {

}

//移动数据
func (byteArray *ByteArray) MoveBytes() {

}

//打印缓冲区
func (byteArray *ByteArray) ToString() string {
	return string(byteArray.Bytes[byteArray.ReadIdx:])
}

//打印调试信息
func (byteArray *ByteArray) Debug() string {
	return string(rune(byteArray.ReadIdx)) + string(rune(byteArray.WriteIdx)) + string(byteArray.Bytes[:])
}
