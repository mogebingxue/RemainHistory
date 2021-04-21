package Base

// Request 请求-每一条请求
type Request struct {
	Conv uint32
	Name string
	Msg  []byte
}
