using System;
using YT;

namespace YT
{
    public partial class MsgHandler
    {

        //获取个人简介内容
        public static void MsgGetPlayerIntroduction(Connection c, MsgBase msgBase) {
            MsgGetPlayerIntroduction msg = (MsgGetPlayerIntroduction)msgBase;
            Player player = c.player;
            if (player == null) return;
            //获取text
            msg.palyerIntroduction = player.data.palyerIntroduction;
            player.Send(msg);
        }

        //保存个人简介内容
        public static void MsgSavePlayerIntroduction(Connection c, MsgBase msgBase) {
            MsgSavePlayerIntroduction msg = (MsgSavePlayerIntroduction)msgBase;
            Player player = c.player;
            if (player == null) return;
            //获取头像
            player.data.palyerIntroduction = msg.palyerIntroduction;
            //保存数据
            DBManager.UpdatePlayerData(c.player.id, c.player.data);
            player.Send(msg);
        }

        //获取头像
        public static void MsgGetHeadPhoto(Connection c, MsgBase msgBase) {
            MsgGetHeadPhoto msg = (MsgGetHeadPhoto)msgBase;
            Player player = c.player;
            if (player == null) return;
            //获取头像
            msg.headPhoto = player.data.headPhoto;
            player.Send(msg);
        }

        //保存头像
        public static void MsgSaveHeadPhoto(Connection c, MsgBase msgBase) {
            MsgSaveHeadPhoto msg = (MsgSaveHeadPhoto)msgBase;
            Player player = c.player;
            if (player == null) return;
            //获取头像
            player.data.headPhoto = msg.headPhoto;
            //保存数据
            DBManager.UpdatePlayerData(c.player.id, c.player.data);
            player.Send(msg);
        }

        //发送消息到世界
        public static void MsgSendMessageToWord(Connection c, MsgBase msgBase) {
            MsgSendMessageToWord msg = (MsgSendMessageToWord)msgBase;

            Player player = c.player;
            //返回协议
            msg.result = 0;
            PlayerManager.Broadcast(msg);


        }

        ////发送消息到好友
        public static void MsgSendMessageToFriend(Connection c, MsgBase msgBase) {
            MsgSendMessageToFriend msg = (MsgSendMessageToFriend)msgBase;


        }

        //获取好友列表
        public static void MsgGetFriendList(Connection c, MsgBase msgBase) {
            MsgGetFriendList msg = (MsgGetFriendList)msgBase;
            Player player = c.player;
            if (player == null) return;
            //获取FriendList
            msg.friendIdList = DBManager.GetFriendList(c.player.id);
            msg.result = 0;
            player.Send(msg);
        }
        //添加好友
        public static void MsgAddFriend(Connection c, MsgBase msgBase) {
            MsgAddFriend msg = (MsgAddFriend)msgBase;
            Player player = c.player;
            if (player == null) return;

            if (DBManager.IsAccountExist(msg.friendId)) {
                msg.result = 1;
                player.Send(msg);
                return;
            }
            if (!DBManager.IsFriendExist(msg.id, msg.friendId)) {
                msg.result = 3;
                player.Send(msg);
                return;
            }
            if (PlayerManager.GetPlayer(msg.friendId) == null) {
                msg.result = 2;
                player.Send(msg);
                return;
            }
            msg.result = 0;
            player.Send(msg);
            PlayerManager.GetPlayer(msg.friendId).Send(msg);
        }

        //删除好友
        public static void MsgDeleteFriend(Connection c, MsgBase msgBase) {
            MsgDeleteFriend msg = (MsgDeleteFriend)msgBase;
            Player player = c.player;
            if (player == null) return;
            if (DBManager.DeleteFriend(player.id, msg.friendId)) {
                msg.result = 0;
                player.Send(msg);
                if (PlayerManager.GetPlayer(msg.friendId) != null) {
                    PlayerManager.GetPlayer(msg.friendId).Send(msg);
                }

            }
            else {
                msg.result = 1;
                player.Send(msg);
            }


        }
        //同意添加好友
        public static void MsgAcceptAddFriend(Connection c, MsgBase msgBase) {
            MsgAcceptAddFriend msg = (MsgAcceptAddFriend)msgBase;
            Player player = c.player;
            if (player == null) return;

            if (DBManager.AddFriend(msg.id, msg.friendId)) {
                msg.result = 0;
            }
            else {
                msg.result = 1;
            }
            player.Send(msg);
            PlayerManager.GetPlayer(msg.friendId).Send(msg);
        }
    }
}