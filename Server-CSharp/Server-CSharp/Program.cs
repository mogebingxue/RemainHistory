using YT;
namespace Game
{
    class MainClass
    {
        public static void Main(string[] args) {
            Server server = Server.CreateServer();
            NetConfig netConfig = ConfigHelper.GetNetConfig();
            //连接数据库，如果数据库连接失败则直接结束程序
            if (!DBManager.Connect(netConfig.DBName, netConfig.DBURL)) {
                return;
            }
            //连接数据库成功则开启服务器
            server.Start();
        }
    }
}
