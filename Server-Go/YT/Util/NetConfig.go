package Util

import (
	"encoding/json"
	"fmt"
	io "io/ioutil"
	"sync"
)

//解析配置信息

// NetInfo 定义配置文件解析后的结构
type NetInfo struct {
	Name       string `json:Name`
	IP         string `json:IP`
	Port       int    `json:Port`
	MaxClients int    `json:MaxClients`
	DBName     string `json:DBName`
	DBURL      string `json:DBURL`
}

var fileLocker sync.Mutex //config file locker

// GetNetConfig 获取网络配置信息
func GetNetConfig(path string) NetInfo {
	conf, bl := loadConfig(path) //get config struct
	if !bl {
		panic("InitConfig failed")
	}
	return conf
}

//读取NetConfig.json文件并反序列化
func loadConfig(filename string) (NetInfo, bool) {
	var conf NetInfo
	fileLocker.Lock()
	data, err := io.ReadFile(filename) //read config file
	fileLocker.Unlock()
	if err != nil {
		fmt.Println("read json file error")
		return conf, false
	}
	dataset := data
	err = json.Unmarshal(dataset, &conf)
	if err != nil {
		fmt.Println("unmarshal json file error")
		return conf, false
	}
	return conf, true
}
