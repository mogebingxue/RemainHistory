package TKcp

import Log2 "ReaminHistory/YT/Log"

//应用层接收消息之后的回调
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
		Log2.Log.Info("handle function <%s> does not exist\n", name)
	}
}

func (action *ReceiveHandle) Call(conv uint32, bytes []byte, len int) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(conv, bytes, len)
		}
	} else {
		Log2.Log.Info("no handle function was attached")
	}
}

//连接请求的回调，在这里要实现回传同意连接和连接号给客户端
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
		Log2.Log.Info("handle function <%s> does not exist\n", name)
	}
}

func (action *ConnectHandle) Call(bytes []byte) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(bytes)
		}
	} else {
		Log2.Log.Info("no handle function was attached")
	}
}

//接收连接请求回调，客户端收到服务端的接收连接请求，之后的回调
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
		Log2.Log.Info("handle function <%s> does not exist\n", name)
	}
}

func (action *AcceptHandle) Call(bytes []byte, len int) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(bytes, len)
		}
	} else {
		Log2.Log.Info("no handle function was attached")
	}
}

//断开连接请求回调，服务端断开一个连接，之后的回调
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
		Log2.Log.Info("handle function <%s> does not exist\n", name)
	}
}

func (action *DisconnectHandle) Call(conv uint32) {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc(conv)
		}
	} else {
		Log2.Log.Info("no handle function was attached")
	}
}

//客户端连接超时的回调
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
		Log2.Log.Info("handle function <%s> does not exist\n", name)
	}
}

func (action *TimeoutHandle) Call() {
	if action.handle != nil {
		for _, handleFunc := range action.handle {
			handleFunc()
		}
	} else {
		Log2.Log.Info("no handle function was attached")
	}
}
