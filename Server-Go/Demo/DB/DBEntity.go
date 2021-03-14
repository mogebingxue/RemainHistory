package DB

//账户信息
type Account struct {
	UId string `bson:"uid,omitempty"`
	Pw  string `bson:"pw,omitempty"`
}

//好友信息
type Friend struct {
	UId      string `bson:"uid,omitempty"`
	FriendId string `bson:"friendId,omitempty"`
}

//玩家信息
type Player struct {
	UId                string `bson:"uid,omitempty"`
	HeadPhoto          int32  `bson:"headPhoto,omitempty"`
	PlayerIntroduction string `bson:"playerIntroduction,omitempty"`
}
