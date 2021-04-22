package TCP

import (
	Log2 "ReaminHistory/YT/Log"
	"time"
)

// getTimeStamp 获取当前时间戳
func getTimeStamp() int64 {
	nowTimeUnix := time.Now().Unix()
	loc, _ := time.LoadLocation("Asia/Beijing")                                   //设置时区
	tt, _ := time.ParseInLocation("2006-01-02 15:04:05", "1970-01-01 0:0:0", loc) //2006-01-02 15:04:05是转换的格式如php的"Y-m-d H:i:s"
	return nowTimeUnix - tt.Unix()
}

// receiveHandle 应用层接收消息之后的回调
type receiveHandle struct {
	handle map[string]func(conv uint32, bytes []byte, len int)
}

func (action *receiveHandle) Add(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	if action.handle == nil {
		action.handle = make(map[string]func(conv uint32, bytes []byte, len int))
	}
	action.handle[name] = handleFunc
}

func (action *receiveHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *receiveHandle) Call(conv uint32, bytes []byte, len int) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(conv, bytes, len)
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}

// connectHandle 服务器接收连接请求的回调
type connectHandle struct {
	handle map[string]func(bytes []byte)
}

func (action *connectHandle) Add(name string, handleFunc func(bytes []byte)) {
	if action.handle == nil {
		action.handle = make(map[string]func(bytes []byte))
	}
	action.handle[name] = handleFunc
}

func (action *connectHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *connectHandle) Call(bytes []byte) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(bytes)
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}

// acceptHandle 客户端接收连接请求回调
type acceptHandle struct {
	handle map[string]func(bytes []byte, len int)
}

func (action *acceptHandle) Add(name string, handleFunc func(bytes []byte, len int)) {
	if action.handle == nil {
		action.handle = make(map[string]func(bytes []byte, len int))
	}
	action.handle[name] = handleFunc
}

func (action *acceptHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *acceptHandle) Call(bytes []byte, len int) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(bytes, len)
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}

// disconnectHandle 断开连接请求回调
type disconnectHandle struct {
	handle map[string]func(conv uint32)
}

func (action *disconnectHandle) Add(name string, handleFunc func(conv uint32)) {
	if action.handle == nil {
		action.handle = make(map[string]func(conv uint32))
	}
	action.handle[name] = handleFunc
}

func (action *disconnectHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *disconnectHandle) Call(conv uint32) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(conv)
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}

// timeoutHandle 客户端连接超时的回调
type timeoutHandle struct {
	handle map[string]func()
}

func (action *timeoutHandle) Add(name string, handleFunc func()) {
	if action.handle == nil {
		action.handle = make(map[string]func())
	}
	action.handle[name] = handleFunc
}

func (action *timeoutHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *timeoutHandle) Call() {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc()
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}
