using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using DBEntity;

public class DBManager
{


    public static IMongoDatabase database;


    /// <summary>
    /// 连接mongoDB
    /// </summary>
    /// <param name="dbname">数据库名称</param>
    /// <param name="connStr">连接URL</param>
    /// <returns>是否连接成功</returns>
    public static bool Connect(string dbname, string connStr) {
        //连接
        try {
            //获得连接
            var client = new MongoClient(connStr);
            //获得数据库
            database = client.GetDatabase(dbname);
            Console.WriteLine("[数据库]connect 成功 ");
            return true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库]connect 失败, " + e.Message);
            return false;
        }
    }



    /// <summary>
    /// 测试并重连
    /// </summary>
    private static void CheckAndReconnect() {
        try {
            if (database != null) {
                return;
            }
            Connect("game", "mongodb://127.0.0.1:27017");
            Console.WriteLine("[数据库] Reconnect!");
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] CheckAndReconnect 失败 " + e.Message);
        }

    }

    /// <summary>
    /// 判定安全字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static bool IsSafeString(string str) {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }


    /// <summary>
    /// 是否存在该用户 若为真，不存在该账户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsAccountExist(string uid) {
        Console.WriteLine(uid);
        CheckAndReconnect();
        //防注入
        if (!IsSafeString(uid)) {
            return false;
        }
        //创建约束生成器
        FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
        //约束条件
        FilterDefinition<BsonDocument> filter = builder.Eq("uid", uid);
        //查询
        try {
            //获取数据
            IMongoCollection<BsonDocument> coll = database.GetCollection<BsonDocument>("account");
            var result = coll.Find(filter).ToList();
            return result.Count == 0 ? false : true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] IsSafeString err, " + e.Message);
            return false;
        }
    }

    //注册
    public static bool Register(string uid, string pw) {
        CheckAndReconnect();
        //防sql注入
        if (!IsSafeString(uid)) {
            Console.WriteLine("[数据库] Register fail, id not safe");
            return false;
        }
        if (!IsSafeString(pw)) {
            Console.WriteLine("[数据库] Register fail, pw not safe");
            return false;
        }
        //能否注册
        if (IsAccountExist(uid)) {
            Console.WriteLine("[数据库] Register fail, id exist");
            return false;
        }
        //写入数据库User表
        Account account = new Account();
        account.uid = uid;
        account.pw = pw;
        try {
            IMongoCollection<MongoBaseEntity> coll = database.GetCollection<MongoBaseEntity>("account");
            coll.InsertOne(account);
            return true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] Register fail " + e.Message);
            return false;
        }
    }

    //创建角色
    public static bool CreatePlayer(string uid) {
        CheckAndReconnect();
        //防注入
        if (!IsSafeString(uid)) {
            Console.WriteLine("[数据库] CreatePlayer fail, id not safe");
            return false;
        }
        DBEntity.Player player = new DBEntity.Player();
        player.uid = uid;
        player.headPhoto = 0;
        player.palyerIntroduction = "这个人很懒，什么也没有写。";
        try {
            IMongoCollection<MongoBaseEntity> coll = database.GetCollection<MongoBaseEntity>("player");
            coll.InsertOne(player);
            return true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] CreatePlayer err, " + e.Message);
            return false;
        }
    }


    //检测用户名密码
    public static bool CheckPassword(string uid, string pw) {
        CheckAndReconnect();
        //防注入
        if (!IsSafeString(uid)) {
            Console.WriteLine("[数据库] CheckPassword fail, id not safe");
            return false;
        }
        if (!IsSafeString(pw)) {
            Console.WriteLine("[数据库] CheckPassword fail, pw not safe");
            return false;
        }
        //查询
        //创建约束生成器
        FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
        //约束条件
        FilterDefinition<BsonDocument> filter = builder.And(builder.Eq("uid", uid), builder.Eq("pw", pw));
        
        try {
            //获取数据
            IMongoCollection<BsonDocument> coll = database.GetCollection<BsonDocument>("account");
            var result = coll.Find(filter).ToList();
            return result.Count == 0 ? false : true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] CheckPassword err, " + e.Message);
            return false;
        }
    }


    //获取玩家数据
    public static PlayerData GetPlayerData(string uid) {
        CheckAndReconnect();
        //防注入
        if (!IsSafeString(uid)) {
            Console.WriteLine("[数据库] GetPlayerData fail, id not safe");
            return null;
        }
        //查询
        //创建约束生成器
        FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
        //约束条件
        FilterDefinition<BsonDocument> filter = builder.Eq("uid", uid);
        try {
            //查询
            //获取数据
            IMongoCollection<BsonDocument> coll = database.GetCollection<BsonDocument>("player");
            var result = coll.Find(filter).ToList();
            if (!(result.Count == 0 ? false : true)) {
                return null;
            }
            //反序列化
            DBEntity.Player player = BsonSerializer.Deserialize<DBEntity.Player>(result[0]);

            PlayerData playerData = new PlayerData();
            playerData.headPhoto = player.headPhoto;
            playerData.palyerIntroduction = player.palyerIntroduction;
            return playerData;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] GetPlayerData fail, " + e.Message);
            return null;
        }
    }


    //保存角色
    public static bool UpdatePlayerData(string uid, PlayerData playerData) {
        CheckAndReconnect();
        //创建约束生成器
        FilterDefinitionBuilder<BsonDocument> builder1 = Builders<BsonDocument>.Filter;
        //约束条件
        FilterDefinition<BsonDocument> filter = builder1.Eq("uid", uid);
        UpdateDefinitionBuilder<BsonDocument> builder2 = Builders<BsonDocument>.Update;
        UpdateDefinition<BsonDocument> update = builder2.Combine(builder2.Set("headPhoto", playerData.headPhoto), builder2.Set("palyerIntroduction", playerData.palyerIntroduction));
        //更新
        try {
            //获取数据
            IMongoCollection<BsonDocument> coll = database.GetCollection<BsonDocument>("player");
            coll.UpdateOne(filter,update);
            return true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] UpdatePlayerData err, " + e.Message);
            return false;
        }
    }

    //获取好友列表
    public static List<string> GetFriendList(string uid) {
        CheckAndReconnect();
        //防sql注入
        if (!IsSafeString(uid)) {
            Console.WriteLine("[数据库] GetPlayerData fail, id not safe");
            return null;
        }
        //创建约束生成器
        FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
        //约束条件
        FilterDefinition<BsonDocument> filter = builder.Eq("uid", uid);
        try {
            //查询
            //获取数据
            IMongoCollection<BsonDocument> coll = database.GetCollection<BsonDocument>("friend");
            var result = coll.Find(filter).ToList();
            if (!(result.Count == 0 ? false : true)) {
                return null;
            }
            //读取
            List<string> friendList = new List<string>();
            foreach (var item in result) {
                friendList.Add(BsonSerializer.Deserialize<Friend>(item).friendId);
            }
            return friendList;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] GetData fail, " + e.Message);
            return null;
        }
    }

    //删除好友
    public static bool DeleteFriend(string uid, string friendId) {
        CheckAndReconnect();
        //防注入
        if (!IsSafeString(uid)) {
            Console.WriteLine("[数据库] DeleteFriend fail, id not safe");
            return false;
        }
        if (!IsSafeString(friendId)) {
            Console.WriteLine("[数据库] DeleteFriend fail, friendId not safe");
            return false;
        }
        //创建约束生成器
        FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
        //约束条件
        FilterDefinition<BsonDocument> filter1 = builder.And(builder.Eq("uid", uid), builder.Eq("friendid", friendId));
        FilterDefinition<BsonDocument> filter2 = builder.And(builder.Eq("uid", friendId), builder.Eq("friendid", uid));
        try {
            //获取数据
            IMongoCollection<BsonDocument> coll = database.GetCollection<BsonDocument>("friend");
            coll.DeleteMany(filter1);
            coll.DeleteMany(filter2);
            return true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] DeleteFriend err, " + e.Message);
            return false;
        }
    }

    //是否是好友
    public static bool IsFriendExist(string uid, string friendId) {
        CheckAndReconnect();
        //防注入
        if (!IsSafeString(uid)) {
            return false;
        }
        if (!IsSafeString(friendId)) {
            return false;
        }
        //创建约束生成器
        FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
        //约束条件
        FilterDefinition<BsonDocument> filter = builder.And(builder.Eq("uid", uid), builder.Eq("friendid", friendId));
        //查询
        try {
            IMongoCollection<BsonDocument> coll = database.GetCollection<BsonDocument>("friend");
            var result = coll.Find(filter).ToList();
            return result.Count == 0 ? false : true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] IsSafeString err, " + e.Message);
            return false;
        }
    }

    //添加好友
    public static bool AddFriend(string uid, string friendId) {
        CheckAndReconnect();
        //防sql注入
        if (!IsSafeString(uid)) {
            Console.WriteLine("[数据库] AddFriend fail, id not safe");
            return false;
        }
        if (!IsSafeString(friendId)) {
            Console.WriteLine("[数据库] AddFriend fail, friendId not safe");
            return false;
        }

        Friend friend1 = new Friend();
        friend1.uid = uid;
        friend1.friendId = friendId;
        Friend friend2 = new Friend();
        friend2.uid = friendId;
        friend2.friendId = uid;

        try {
            IMongoCollection<MongoBaseEntity> coll = database.GetCollection<MongoBaseEntity>("friend");
            coll.InsertOne(friend1);
            coll.InsertOne(friend2);
            return true;
        }
        catch (Exception e) {
            Console.WriteLine("[数据库] AddFriend err, " + e.Message);
            return false;
        }
    }
}


