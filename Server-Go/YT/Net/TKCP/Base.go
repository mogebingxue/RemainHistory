package TKCP

import (
	Log2 "ReaminHistory/YT/Log"
	"time"
)

// GetTimeStamp 获取当前时间戳
func GetTimeStamp() int64 {
	nowTimeUnix := time.Now().Unix()
	loc, _ := time.LoadLocation("Asia/Beijing")                                   //设置时区
	tt, _ := time.ParseInLocation("2006-01-02 15:04:05", "1970-01-01 0:0:0", loc) //2006-01-02 15:04:05是转换的格式如php的"Y-m-d H:i:s"
	return nowTimeUnix - tt.Unix()
}

// ReceiveHandle 应用层接收消息之后的回调
type ReceiveHandle struct {
	handle map[string]func(conv uint32, bytes []byte, len int)
}

func (action *ReceiveHandle) Add(name string, handleFunc func(conv uint32, bytes []byte, len int)) {
	if action.handle == nil {
		action.handle = make(map[string]func(conv uint32, bytes []byte, len int))
	}
	action.handle[name] = handleFunc
}

func (action *ReceiveHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *ReceiveHandle) Call(conv uint32, bytes []byte, len int) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(conv, bytes, len)
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}

// ConnectHandle 服务器接收连接请求的回调
type ConnectHandle struct {
	handle map[string]func(bytes []byte)
}

func (action *ConnectHandle) Add(name string, handleFunc func(bytes []byte)) {
	if action.handle == nil {
		action.handle = make(map[string]func(bytes []byte))
	}
	action.handle[name] = handleFunc
}

func (action *ConnectHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *ConnectHandle) Call(bytes []byte) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(bytes)
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}

// AcceptHandle 客户端接收连接请求回调
type AcceptHandle struct {
	handle map[string]func(bytes []byte, len int)
}

func (action *AcceptHandle) Add(name string, handleFunc func(bytes []byte, len int)) {
	if action.handle == nil {
		action.handle = make(map[string]func(bytes []byte, len int))
	}
	action.handle[name] = handleFunc
}

func (action *AcceptHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *AcceptHandle) Call(bytes []byte, len int) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(bytes, len)
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}

// DisconnectHandle 断开连接请求回调
type DisconnectHandle struct {
	handle map[string]func(conv uint32)
}

func (action *DisconnectHandle) Add(name string, handleFunc func(conv uint32)) {
	if action.handle == nil {
		action.handle = make(map[string]func(conv uint32))
	}
	action.handle[name] = handleFunc
}

func (action *DisconnectHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *DisconnectHandle) Call(conv uint32) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(conv)
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}

// TimeoutHandle 客户端连接超时的回调
type TimeoutHandle struct {
	handle map[string]func()
}

func (action *TimeoutHandle) Add(name string, handleFunc func()) {
	if action.handle == nil {
		action.handle = make(map[string]func())
	}
	action.handle[name] = handleFunc
}

func (action *TimeoutHandle) Remove(name string) {
	if _, ok := action.handle[name]; ok {
		delete(action.handle, name)
	} else {
		Log2.Log.Info("回调函数", name, "不存在")
	}
}

func (action *TimeoutHandle) Call() {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc()
		}
	} else {
		Log2.Log.Info("没有回调函数被注册")
	}
}
