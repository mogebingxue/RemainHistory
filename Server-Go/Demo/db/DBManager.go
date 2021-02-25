package db

import (
	"context"
	"fmt"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.mongodb.org/mongo-driver/mongo/readpref"
	"strings"
	"time"
)

//连接数据库
func Connect(dbname string,connStr string) {
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
	fmt.Println("连接数据库成功"+db.Name())
}

//判断安全字符
func IsSafeString(str string) bool {
	s:=[...]string{"[","-","|",",",".","!","@","#","$","%","^","&","*","(",")","_","=","+","]","{","}","'",":",";","?","/"}
	for _,v := range s {
		if strings.ContainsAny(str,v) {
			return false
		}
	}
	return true
}

