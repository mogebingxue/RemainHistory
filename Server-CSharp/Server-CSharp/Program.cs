using System;
using YT;
namespace Game
{
    class MainClass
    {
        public static void Main(string[] args) {
           
            NetConfig netConfig = ConfigHelper.GetNetConfig();
            Server server = Server.CreateServer(netConfig.IP,netConfig.Port,netConfig.Name,netConfig.MaxClients);
            server.AddRouter("MsgLogin", MsgHandler.MsgLogin);
            server.AddRouter("MsgRegister", MsgHandler.MsgRegister);
            server.AddRouter("MsgGetPlayerIntroduction", MsgHandler.MsgGetPlayerIntroduction);
            server.AddRouter("MsgSavePlayerIntroduction", MsgHandler.MsgSavePlayerIntroduction);
            server.AddRouter("MsgGetHeadPhoto", MsgHandler.MsgGetHeadPhoto);
            server.AddRouter("MsgSaveHeadPhoto", MsgHandler.MsgSaveHeadPhoto);
            server.AddRouter("MsgSendMessageToWord", MsgHandler.MsgSendMessageToWord);
            server.AddRouter("MsgSendMessageToFriend", MsgHandler.MsgSendMessageToFriend);
            server.AddRouter("MsgGetFriendList", MsgHandler.MsgGetFriendList);
            server.AddRouter("MsgAddFriend", MsgHandler.MsgAddFriend);
            server.AddRouter("MsgDeleteFriend", MsgHandler.MsgDeleteFriend);
            server.AddRouter("MsgAcceptAddFriend", MsgHandler.MsgAcceptAddFriend);
            server.AddRouter("MsgAddFriend", MsgHandler.MsgAddFriend);
            //连接数据库，如果数据库连接失败则直接结束程序
            if (!DBManager.Connect(netConfig.DBName, netConfig.DBURL)) {
                return;
            }
            //连接数据库成功则开启服务器
            server.Start();
            server.AddDisconnectHandle(PlayerManager.OnPlayerDisconnect);
            
        }

    }
}
