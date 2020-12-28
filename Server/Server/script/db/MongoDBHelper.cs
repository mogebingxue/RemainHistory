using DBEntity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

public class MongoDBHelper
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
    /// 创建集合对象
    /// </summary>
    /// <param name="collName">集合名称</param>
    ///<returns>集合对象</returns>
    private static IMongoCollection<MongoBaseEntity> GetColletion(string collName) {
        return database.GetCollection<MongoBaseEntity>(collName);
    }
    #region 增加
    /// <summary>
    /// 插入对象
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="t">插入的对象</param>
    public static void Insert(string collName, MongoBaseEntity t) {
        var coll = GetColletion(collName);
        coll.InsertOne(t);
    }

    /// <summary>
    /// 批量插入
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="ts">要插入的对象集合</param>
    public static void InsertBath(string collName, IEnumerable<MongoBaseEntity> ts) {
        var coll = GetColletion(collName);
        coll.InsertMany(ts);
    }
    #endregion
    #region 删除
    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public long Delete(string collName, QueryDocument document) {
        var coll = GetColletion(collName);
        var result = coll.DeleteMany(document);
        return result.DeletedCount;
    }
    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public long Delete(string collName, string json) {
        var coll = GetColletion(collName);
        var result = coll.DeleteMany(json);
        return result.DeletedCount;
    }
    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public long Delete(string collName, Expression<Func<MongoBaseEntity, bool>> predicate) {
        var coll = GetColletion(collName);
        var result = coll.DeleteMany(predicate);
        return result.DeletedCount;
    }
    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders<T>构建复杂的查询条件
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public long Delete(string collName, FilterDefinition<MongoBaseEntity> filter) {
        var coll = GetColletion(collName);
        var result = coll.DeleteMany(filter);
        return result.DeletedCount;
    }
    #endregion
    #region 修改
    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public long Update(string collName, Expression<Func<MongoBaseEntity, bool>> filter, UpdateDefinition<MongoBaseEntity> update, bool upsert = false) {
        var coll = GetColletion(collName);
        var result = coll.UpdateMany(filter, update, new UpdateOptions { IsUpsert = upsert });
        return result.ModifiedCount;
    }
    /// <summary>
    /// 用新对象替换新文档-更新批量
    /// </summary>
    /// <param name = "collName" > 集合名称 </ param >
    /// < param name="filter">修改条件</param>
    /// <param name = "t" > 新对象 </ param >
    /// < param name="upsert">是否插入新文档（filter条件满足就更新，否则true插入新文档）</param>
    /// <returns>修改影响文档数</returns>
    public Int64 UpdateT(String collName, FilterDefinition<MongoBaseEntity> filter, MongoBaseEntity t, Boolean upsert = false) {
        var coll = GetColletion(collName);
        BsonDocument document = t.ToBsonDocument<MongoBaseEntity>();
        document.Remove("_id");
        UpdateDocument update = new UpdateDocument("$set", document);
        var result = coll.UpdateMany(filter, update, new UpdateOptions { IsUpsert = upsert });
        return result.ModifiedCount;
    }
    /// <summary>
    /// 用新对象替换新文档-更新单个
    /// </summary>
    /// <param name = "collName" > 集合名称 </ param >
    /// < param name="filter">修改条件</param>
    /// <param name = "t" > 新对象 </ param >
    /// < param name="upsert">是否插入新文档（filter条件满足就更新，否则true插入新文档）</param>
    /// <returns>修改影响文档数</returns>
    public long UpdateOne(string collName, FilterDefinition<MongoBaseEntity> filter, MongoBaseEntity t, bool upsert = false) {
        var coll = GetColletion(collName);
        BsonDocument document = t.ToBsonDocument<MongoBaseEntity>();
        document.Remove("_id");
        UpdateDocument update = new UpdateDocument("$set", document);
        var result = coll.UpdateOne(filter, update, new UpdateOptions { IsUpsert = upsert });
        return result.ModifiedCount;
    }
    #endregion


    #region 查询
    /// <summary>
    /// 查询，复杂查询直接用Linq处理
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <returns>要查询的对象</returns>
    private static IQueryable<MongoBaseEntity> GetQueryable(string collName) {
        var coll = GetColletion(collName);
        return coll.AsQueryable();
    }
    /// <summary>
    /// 根据条件表达式返回可查询的记录源
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <param name="sortPropertyName">排序表达式</param>
    /// <param name="isDescending">如果为true则为降序，否则为升序</param>
    /// <returns></returns>
    private static IFindFluent<MongoBaseEntity, MongoBaseEntity> GetQueryable(string collName, FilterDefinition<MongoBaseEntity> query, string sortPropertyName, bool isDescending = true) {
        IMongoCollection<MongoBaseEntity> collection = GetColletion(collName);
        IFindFluent<MongoBaseEntity, MongoBaseEntity> queryable = collection.Find(query);
        var sort = isDescending ? Builders<MongoBaseEntity>.Sort.Descending(sortPropertyName) : Builders<MongoBaseEntity>.Sort.Ascending(sortPropertyName);
        return queryable.Sort(sort);
    }
    /// <summary>
    /// 根据条件表达式返回可查询的记录源
    /// </summary>
    /// <param name="match">查询条件</param>
    /// <param name="orderByProperty">排序表达式</param>
    /// <param name="isDescending">如果为true则为降序，否则为升序</param>
    /// <returns></returns>
    private static IQueryable<MongoBaseEntity> GetQueryable<TKey>(string collName, Expression<Func<MongoBaseEntity, bool>> match, Expression<Func<MongoBaseEntity, TKey>> orderByProperty, bool isDescending = true) {
        IMongoCollection<MongoBaseEntity> collection = GetColletion(collName);
        IQueryable<MongoBaseEntity> query = collection.AsQueryable();
        if (match != null) {
            query = query.Where(match);
        }
        if (orderByProperty != null) {
            query = isDescending ? query.OrderByDescending(orderByProperty) : query.OrderBy(orderByProperty);
        }
        else {
            // query = query.OrderBy(sortPropertyName, isDescending);
        }
        return query;
    }
    /// <summary>
    /// 根据条件查询数据库,并返回对象集合
    /// </summary>
    /// <param name="match">条件表达式</param>
    /// <param name="sortPropertyName">排序字段</param>
    /// <param name="isDescending">如果为true则为降序，否则为升序</param>
    /// <returns></returns>
    public static IList<MongoBaseEntity> Find(string collName, Expression<Func<MongoBaseEntity, bool>> match, string sortPropertyName, bool isDescending = true) {
        return GetQueryable(collName, match, sortPropertyName, isDescending).ToList();
    }
    /// <summary>
    /// 根据条件查询数据库,并返回对象集合
    /// </summary>
    /// <param name="query">条件表达式</param>
    /// <param name="sortPropertyName">排序字段</param>
    /// <param name="isDescending">如果为true则为降序，否则为升序</param>
    /// <returns></returns>
    public static IList<MongoBaseEntity> Find(string collName, FilterDefinition<MongoBaseEntity> query, string sortPropertyName, bool isDescending = true) {
        return GetQueryable(collName, query, sortPropertyName, isDescending).ToList();
    }
    /// <summary>
    /// 根据条件查询数据库,并返回对象集合
    /// </summary>
    /// <param name="match">条件表达式</param>
    /// <param name="orderByProperty">排序表达式</param>
    /// <param name="isDescending">如果为true则为降序，否则为升序</param>
    /// <returns></returns>
    public IList<MongoBaseEntity> Find<TKey>(string collName, Expression<Func<MongoBaseEntity, bool>> match, Expression<Func<MongoBaseEntity, TKey>> orderByProperty, bool isDescending = true) {
        return GetQueryable(collName, match, orderByProperty, isDescending).ToList();
    }
    /// <summary>
    /// 根据条件查询数据库,如果存在返回第一个对象
    /// </summary>
    /// <param name="filter">条件表达式</param>
    /// <returns>存在则返回指定的第一个对象,否则返回默认值</returns>
    public MongoBaseEntity FindSingle(string collName, FilterDefinition<MongoBaseEntity> filter) {
        var coll = GetColletion(collName);
        return coll.Find(filter).FirstOrDefault();
    }
    #endregion
}
