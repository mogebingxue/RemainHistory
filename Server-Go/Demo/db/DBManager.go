package db

import (
	"ReaminHistory/Demo/Helper"
	"ReaminHistory/Demo/Player/PlayerData"
	"context"
	"fmt"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.mongodb.org/mongo-driver/mongo/readpref"
	"strings"
	"time"
)

var database *mongo.Database

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
		fmt.Println("连接数据库成功失败")
		panic(err)
	}
	database = client.Database(dbname)
	fmt.Println("连接数据库成功", database.Name())
}

//测试并重连
func CheckAndReconnect() {
	if database == nil {
		netInfo := Helper.GetNetConfig()
		Connect(netInfo.DBName, netInfo.DBURL)
		fmt.Println("数据库重连")
	}
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
	CheckAndReconnect()
	if !IsSafeString(id) {
		return false
	}
	account := new(Account)
	account.UId = id
	query := database.Collection("account").FindOne(context.TODO(), account)
	res, err := query.DecodeBytes()
	if err == nil && res.String() != "" {
		return true
	} else {
		return false
	}
}

//注册
func Register(uid string, pw string) bool {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] Register fail, id not safe")
		return false
	}
	if !IsSafeString(pw) {
		fmt.Println("[数据库] Register fail, pw not safe")
		return false
	}
	//uid是否存在
	if IsAccountExist(uid) {
		fmt.Println("[数据库] Register fail, id exist")
		return false
	}
	//写入数据库
	account := new(Account)
	account.UId = uid
	account.Pw = pw
	_, err := database.Collection("account").InsertOne(context.TODO(), account)
	if err != nil {
		fmt.Println("[数据库] Register fail")
		return false
	} else {
		return true
	}
}

//创建角色
func CreatePlayer(uid string) bool {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] CreatePlayer fail, id not safe")
		return false
	}
	player := new(Player)
	player.UId = uid
	player.HeadPhoto = 0
	player.PlayerIntroduction = "这个人很懒，什么也没有写。"
	_, err := database.Collection("account").InsertOne(context.TODO(), player)
	if err != nil {
		fmt.Println("[数据库] CreatePlayer fail")
		return false
	} else {
		return true
	}
}

//检测用户名及密码
func CheckPassword(uid string, pw string) bool {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] CheckPassword fail, id not safe")
		return false
	}
	if !IsSafeString(pw) {
		fmt.Println("[数据库] CheckPassword fail, pw not safe")
		return false
	}
	account := new(Account)
	account.UId = uid
	account.Pw = pw
	query := database.Collection("account").FindOne(context.TODO(), account)
	res, err := query.DecodeBytes()
	if err == nil && res.String() != "" {
		return true
	} else {
		return false
	}
}

//获取玩家数据
func GetPlayerData(uid string) *PlayerData.PlayerData {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] GetPlayerData fail, id not safe")
		return nil
	}
	player := new(Player)
	player.UId = uid
	query := database.Collection("player").FindOne(context.TODO(), player)
	err := query.Decode(player)
	if err != nil {
		fmt.Println("[数据库] GetPlayerData fail")
		return nil
	}
	playerData := new(PlayerData.PlayerData)
	playerData.HeadPhoto = player.HeadPhoto
	playerData.PlayerIntroduction = player.PlayerIntroduction
	return playerData
}

//保存玩家角色数据
func UpdatePlayerData(uid string, data *PlayerData.PlayerData) bool {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] UpdatePlayerData fail, id not safe")
		return false
	}
	player := new(Player)
	player.UId = uid
	player.HeadPhoto = data.HeadPhoto
	player.PlayerIntroduction = data.PlayerIntroduction
	update := bson.D{{"$set", player}}
	res := database.Collection("player").FindOneAndUpdate(context.TODO(), bson.M{"uid": uid}, update)
	query, err := res.DecodeBytes()
	if err != nil || query.String() == "" {
		return false
	}
	return true
}

//获取好友列表
func GetFriendList(uid string) string {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] GetFriendList fail, id not safe")
		return ""
	}
	query, err := database.Collection("friend").Find(context.TODO(), bson.M{"uid": uid})
	if err != nil {
		fmt.Println("[数据库] GetFriendList fail")
		return ""
	}
	friends := make([]Friend, query.RemainingBatchLength())
	err1 := query.All(context.TODO(), &friends)
	if err1 != nil {
		fmt.Println("[数据库] GetFriendList fail")
		return ""
	}
	friendList := ""
	for _, s := range friends {
		friendList += s.FriendId + ","
	}
	return friendList
}

//删除好友
func DeleteFriend(uid string, friendId string) bool {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] DeleteFriend fail, id not safe")
		return false
	}
	if !IsSafeString(friendId) {
		fmt.Println("[数据库] DeleteFriend fail, friendId not safe")
		return false
	}
	friend1 := new(Friend)
	friend1.UId = uid
	friend1.FriendId = friendId
	friend2 := new(Friend)
	friend2.FriendId = uid
	friend2.UId = friendId
	_, err1 := database.Collection("friend").DeleteMany(context.TODO(), friend1)
	_, err2 := database.Collection("friend").DeleteMany(context.TODO(), friend2)
	if err1 != nil || err2 != nil {
		return false
	} else {
		return true
	}
}

//是否是好友
func IsFriendExist(uid string, friendId string) bool {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] IsFriendExist fail, id not safe")
		return false
	}
	if !IsSafeString(friendId) {
		fmt.Println("[数据库] IsFriendExist fail, id not safe")
		return false
	}
	friend := new(Friend)
	friend.UId = uid
	friend.FriendId = friendId
	query := database.Collection("friend").FindOne(context.TODO(), friend)
	res, err := query.DecodeBytes()
	if err == nil && res.String() != "" {
		return true
	} else {
		return false
	}
}

//添加好友
func AddFriend(uid string, friendId string) bool {
	CheckAndReconnect()
	//防止特殊字符
	if !IsSafeString(uid) {
		fmt.Println("[数据库] AddFriend fail, id not safe")
		return false
	}
	if !IsSafeString(friendId) {
		fmt.Println("[数据库] AddFriend fail, id not safe")
		return false
	}
	if IsFriendExist(uid, friendId) {
		fmt.Println("[数据库] AddFriend fail, id exit")
		return false
	}
	if IsFriendExist(friendId, uid) {
		fmt.Println("[数据库] AddFriend fail, id exit")
		return false
	}
	friend1 := new(Friend)
	friend1.UId = uid
	friend1.FriendId = friendId
	friend2 := new(Friend)
	friend2.FriendId = uid
	friend2.UId = friendId
	//写入数据库
	_, err1 := database.Collection("friend").InsertOne(context.TODO(), friend1)
	_, err2 := database.Collection("friend").InsertOne(context.TODO(), friend2)
	if err1 != nil || err2 != nil {
		fmt.Println("[数据库] AddFriend fail")
		return false
	} else {
		return true
	}
}
