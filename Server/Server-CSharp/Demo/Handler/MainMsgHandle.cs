using Game;
using Google.Protobuf;
using System;
using YT;

namespace YT
{
    public partial class MsgHandler
    {

        //获取个人简介内容
        public static void MsgGetPlayerIntroduction(Connection c, byte[] bytes) {
            MsgGetPlayerIntroduction msg = Game.MsgGetPlayerIntroduction.Parser.ParseFrom(bytes);
            Player player = c.player;
            if (player == null) return;
            //获取text
            msg.PalyerIntroduction = player.data.palyerIntroduction;
            player.Send(msg);
        }

        //保存个人简介内容
        public static void MsgSavePlayerIntroduction(Connection c, byte[] bytes) {
            MsgSavePlayerIntroduction msg = Game.MsgSavePlayerIntroduction.Parser.ParseFrom(bytes);
            Player player = c.player;
            if (player == null) return;
            //获取头像
            player.data.palyerIntroduction = msg.PalyerIntroduction;
            //保存数据
            DBManager.UpdatePlayerData(c.player.id, c.player.data);
            player.Send(msg);
        }

        //获取头像
        public static void MsgGetHeadPhoto(Connection c, byte[] bytes) {
            MsgGetHeadPhoto msg = Game.MsgGetHeadPhoto.Parser.ParseFrom(bytes);
            Player player = c.player;
            if (player == null) return;
            //获取头像
            msg.HeadPhoto = player.data.headPhoto;
            player.Send(msg);
        }

        //保存头像
        public static void MsgSaveHeadPhoto(Connection c, byte[] bytes) {
            MsgSaveHeadPhoto msg = Game.MsgSaveHeadPhoto.Parser.ParseFrom(bytes);
            Player player = c.player;
            if (player == null) return;
            //获取头像
            player.data.headPhoto = msg.HeadPhoto;
            //保存数据
            DBManager.UpdatePlayerData(c.player.id, c.player.data);
            player.Send(msg);
        }

        //发送消息到世界
        public static void MsgSendMessageToWord(Connection c, byte[] bytes) {
            MsgSendMessageToWord msg = Game.MsgSendMessageToWord.Parser.ParseFrom(bytes);

            Player player = c.player;
            //返回协议
            msg.Result = 0;
            PlayerManager.Broadcast(msg);


        }

        ////发送消息到好友
        public static void MsgSendMessageToFriend(Connection c, byte[] bytes) {
            MsgSendMessageToFriend msg = Game.MsgSendMessageToFriend.Parser.ParseFrom(bytes);


        }

        //获取好友列表
        public static void MsgGetFriendList(Connection c, byte[] bytes) {
            MsgGetFriendList msg = Game.MsgGetFriendList.Parser.ParseFrom(bytes);
            Player player = c.player;
            if (player == null) return;
            //获取FriendList
            msg.FriendList = DBManager.GetFriendList(c.player.id);
            msg.Result = 0;
            player.Send(msg);
        }
        //添加好友
        public static void MsgAddFriend(Connection c, byte[] bytes) {
            MsgAddFriend msg = Game.MsgAddFriend.Parser.ParseFrom(bytes);
            Player player = c.player;
            if (player == null) return;

            if (DBManager.IsAccountExist(msg.FriendId)) {
                msg.Result = 1;
                player.Send(msg);
                return;
            }
            if (!DBManager.IsFriendExist(msg.Id, msg.FriendId)) {
                msg.Result = 3;
                player.Send(msg);
                return;
            }
            if (PlayerManager.GetPlayer(msg.FriendId) == null) {
                msg.Result = 2;
                player.Send(msg);
                return;
            }
            msg.Result = 0;
            player.Send(msg);
            PlayerManager.GetPlayer(msg.FriendId).Send(msg);
        }

        //删除好友
        public static void MsgDeleteFriend(Connection c, byte[] bytes) {
            MsgDeleteFriend msg = Game.MsgDeleteFriend.Parser.ParseFrom(bytes);
            Player player = c.player;
            if (player == null) return;
            if (DBManager.DeleteFriend(player.id, msg.FriendId)) {
                msg.Result = 0;
                player.Send(msg);
                if (PlayerManager.GetPlayer(msg.FriendId) != null) {
                    PlayerManager.GetPlayer(msg.FriendId).Send(msg);
                }

            }
            else {
                msg.Result = 1;
                player.Send(msg);
            }


        }
        //同意添加好友
        public static void MsgAcceptAddFriend(Connection c, byte[] bytes) {
            MsgAcceptAddFriend msg = Game.MsgAcceptAddFriend.Parser.ParseFrom(bytes);
            Player player = c.player;
            if (player == null) return;

            if (DBManager.AddFriend(msg.Id, msg.FriendId)) {
                msg.Result = 0;
            }
            else {
                msg.Result = 1;
            }
            player.Send(msg);
            PlayerManager.GetPlayer(msg.FriendId).Send(msg);
        }
    }
}