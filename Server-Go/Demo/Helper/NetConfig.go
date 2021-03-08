package Helper

import (
	"encoding/json"
	"fmt"
	io "io/ioutil"
	"sync"
)

//定义配置文件解析后的结构
type NetInfo struct {
	Name  string `json:Name`
	IP  string `json:IP`
	Port      int `json:Port`
	MaxClients  	  int `json:MaxClients`
	DBName    string `json:DBName`
	DBURL string `json:DBURL`
}


var fileLocker sync.Mutex //config file locker

//获取网络配置信息
func GetNetConfig() NetInfo {
	conf, bl := loadConfig("./NetConfig.json") //get config struct
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
	datajson := []byte(data)
	err = json.Unmarshal(datajson, &conf)
	if err != nil {
		fmt.Println("unmarshal json file error")
		return conf, false
	}
	return conf, true
}