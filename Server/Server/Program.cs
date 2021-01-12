using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;

namespace Game
{
    class MainClass
    {
        public static void Main(string[] args) {
            NetConfig netConfig = ConfigHelper.GetNetConfig();
            //连接数据库，如果数据库连接失败则直接结束程序
            if (!DBManager.Connect(netConfig.DBName, netConfig.DBURL)) {
                return;
            }

            //连接数据库成功则进入服务器主循环
            NetManager.StartLoop(netConfig.Port);
        }
    }
}
