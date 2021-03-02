using Google.Protobuf;
using Login;
using System;
using YT;

namespace YT

{
    public partial class MsgHandler
    {


        //注册协议处理
        public static void MsgRegister(Connection c, byte[] bytes) {

            MsgRegister msg = Login.MsgRegister.Parser.ParseFrom(bytes);
            //注册
            if (DBManager.Register(msg.Id, msg.Pw)) {
                DBManager.CreatePlayer(msg.Id);
                msg.Result = 0;
            }
            else {
                msg.Result = 1;
            }
            c.Send(msg);
            
        }


        //登陆协议处理
        public static void MsgLogin(Connection c, byte[] bytes) {

            MsgLogin msg = Login.MsgLogin.Parser.ParseFrom(bytes);
            //密码校验
            if (!DBManager.CheckPassword(msg.Id, msg.Pw)) {
                msg.Result = 1;
                c.Send(msg);
                return;
            }
            //不允许再次登陆
            if (Server.Players.ContainsKey(c.Conv)) {
                msg.Result = 1;
                c.Send(msg);
                return;
            }
            //如果已经登陆，踢下线
            if (PlayerManager.IsOnline(msg.Id)) {
                //发送踢下线协议
                Player other = PlayerManager.GetPlayer(msg.Id);
                MsgKick msgKick = new MsgKick();
                msgKick.Result = 0;
                other.Send(msgKick);
                //断开连接
                c.Send(msg);
            }
            //获取玩家数据
            PlayerData playerData = DBManager.GetPlayerData(msg.Id);
            if (playerData == null) {
                msg.Result = 1;
                c.Send(msg);
                return;
            }
            //构建Player
            Player player = new Player(c.Conv);
            player.id = msg.Id;
            player.data = playerData;
            PlayerManager.AddPlayer(msg.Id, player);
            Server.Players.Add(c.Conv,player);
            //返回协议
            msg.Result = 0;
            player.Send(msg);
        }
    }
}
