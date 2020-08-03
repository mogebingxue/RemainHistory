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
        //获取text
        player.data.palyerIntroduction = msg.palyerIntroduction;
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
}