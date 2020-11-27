using System;


public partial class MsgHandler
{

    //获取个人简介内容
    public static void MsgGetPlayerIntroduction(ClientState c, MsgBase msgBase) {
        MsgGetPlayerIntroduction msg = (MsgGetPlayerIntroduction)msgBase;
        Player player = c.player;
        if (player == null) return;
        //获取text
        msg.palyerIntroduction = player.data.palyerIntroduction;
        player.Send(msg);
    }

    //保存个人简介内容
    public static void MsgSavePlayerIntroduction(ClientState c, MsgBase msgBase) {
        MsgSavePlayerIntroduction msg = (MsgSavePlayerIntroduction)msgBase;
        Player player = c.player;
        if (player == null) return;
        //获取头像
        player.data.palyerIntroduction = msg.palyerIntroduction;
        player.Send(msg);
    }

    //获取头像
    public static void MsgGetHeadPhoto(ClientState c, MsgBase msgBase) {
        MsgGetHeadPhoto msg = (MsgGetHeadPhoto)msgBase;
        Player player = c.player;
        if (player == null) return;
        //获取头像
        msg.headPhoto = player.data.headPhoto;
        player.Send(msg);
    }

    //保存头像
    public static void MsgSaveHeadPhoto(ClientState c, MsgBase msgBase) {
        MsgSaveHeadPhoto msg = (MsgSaveHeadPhoto)msgBase;
        Player player = c.player;
        if (player == null) return;
        //获取头像
        player.data.headPhoto = msg.headPhoto;
        player.Send(msg);
    }

    //发送消息到世界
    public static void MsgSendMessageToWord(ClientState c, MsgBase msgBase) {
        MsgSendMessageToWord msg = (MsgSendMessageToWord)msgBase;

        Player player = c.player;
        //返回协议
        msg.result = 0;
        PlayerManager.Broadcast(msg);


    }

    ////发送消息到好友
    public static void MsgSendMessageToFriend(ClientState c, MsgBase msgBase) {
        MsgSendMessageToFriend msg = (MsgSendMessageToFriend)msgBase;


    }

    //获取好友列表
    public static void MsgGetFriendList(ClientState c, MsgBase msgBase) {
        MsgGetFriendList msg = (MsgGetFriendList)msgBase;
        Player player = c.player;
        if (player == null) return;
        //获取FriendList
        msg.friendIdList = DbManager.GetFriendList(c.player.id);
        msg.result = 0;
        player.Send(msg);
    }
    //添加好友
    public static void MsgAddFriend(ClientState c, MsgBase msgBase) {
        MsgAddFriend msg = (MsgAddFriend)msgBase;
        Player player = c.player;
        if (player == null) return;

        if (DbManager.IsAccountExist(msg.friendId)) {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        if (!DbManager.IsFriendExist(msg.id, msg.friendId)) {
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
    public static void MsgDeleteFriend(ClientState c, MsgBase msgBase) {
        MsgDeleteFriend msg = (MsgDeleteFriend)msgBase;
        Player player = c.player;
        if (player == null) return;
        if (DbManager.DeleteFriend(player.id, msg.friendId)) {
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
    public static void MsgAcceptAddFriend(ClientState c, MsgBase msgBase) {
        MsgAcceptAddFriend msg = (MsgAcceptAddFriend)msgBase;
        Player player = c.player;
        if (player == null) return;

        if (DbManager.AddFriend(msg.id, msg.friendId)) {
            msg.result = 0;
        }
        else {
            msg.result = 1;
        }
        player.Send(msg);
        PlayerManager.GetPlayer(msg.friendId).Send(msg);
    }
}