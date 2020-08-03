using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendPanel : BasePanel
{
    //关闭按钮
    private Button closeBtn;
    //FriendListPanel
    private GameObject friendListPanel;

    //初始化
    public override void OnInit() {
        skinPath = "FriendPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        friendListPanel = skin.transform.Find("FriendListPanel").gameObject;
        //监听
        closeBtn.onClick.AddListener(OnCloseClick);
        //网络协议监听
        NetManager.AddMsgListener("MsgGetFriendList", OnMsgGetFriendList);
        NetManager.AddMsgListener("MsgDeleteFriend", OnMsgDeleteFriend);
        NetManager.AddMsgListener("MsgAddFriend", OnMsgAddFriend);
        //发送获取好友列表协议
        MsgGetFriendList msgGetFriendList = new MsgGetFriendList();
        NetManager.Send(msgGetFriendList);
    }

    //收到添加好友协议
    private void OnMsgAddFriend(MsgBase msg) {
        throw new NotImplementedException();
    }
    //收到删除好友协议
    private void OnMsgDeleteFriend(MsgBase msg) {
        throw new NotImplementedException();
    }
    //收到获取好友列表协议
    private void OnMsgGetFriendList(MsgBase msg) {
        MsgGetFriendList msgGetFriendList = (MsgGetFriendList)msg;
        GameObject friensEnumPrefab = ResManager.LoadPrefab("FriendEnum");
        foreach(string friendId in msgGetFriendList.friendIdList) {
            GameObject friensEnum = (GameObject)Instantiate(friensEnumPrefab);
            friensEnum.transform.SetParent(friendListPanel.transform, false);
            friensEnum.transform.Find("ID").GetComponent<Text>().text = friendId;
        }
        
    }

    //当按下关闭按钮
    public void OnCloseClick() {
        Close();
    }

    //关闭
    public override void OnClose() {
    }
}
