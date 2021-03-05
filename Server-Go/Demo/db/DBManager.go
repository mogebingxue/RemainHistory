package db

import (
	"ReaminHistory/Demo/Player/PlayerData"
	"context"
	"fmt"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.mongodb.org/mongo-driver/mongo/readpref"
	"strings"
	"time"
)

//连接数据库
func Connect(dbname string, connStr string) {
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()
	client, err := mongo.Connect(ctx, options.Client().ApplyURI(connStr))
	if err != nil {
		panic(err)
	}
	// Ping the primary
	if err := client.Ping(ctx, readpref.Primary()); err != nil {
		panic(err)
	}
	db := client.Database(dbname)
	fmt.Println("连接数据库成功" + db.Name())
}

//测试并重连
func CheckAndReconnect() {

}

//判断安全字符
func IsSafeString(str string) bool {
	s := [...]string{"[", "-", "|", ",", ".", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "=", "+", "]", "{", "}", "'", ":", ";", "?", "/"}
	for _, v := range s {
		if strings.ContainsAny(str, v) {
			return false
		}
	}
	return true
}

//是否存在该用户
func IsAccountExist(id string) bool {
	return true
}

//注册
func Register(uid string, pw string) bool {
	return true
}

//创建角色
func CreatePlayer(uid string) {

}

//检测用户名及密码
func CheckPassword(uid string, pw string) bool {
	return true
}

//获取玩家数据
func GetPlayerData(uid string) *PlayerData.PlayerData {
	return &PlayerData.PlayerData{}
}

//保存玩家角色数据
func UpdatePlayerData(id string, data *PlayerData.PlayerData) {

}

//获取好友列表
func GetFriendList(uid string) {

}

//删除好友
func DeleteFriend(uid string, pw string) {

}

//是否是好友
func IsFriendExist(uid string, pw string) bool {
	return true
}

//添加好友
func AddFriend(uid string, pw string) {

}
