using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FriendPanel : BasePanel
{
    //关闭按钮
    private Button _closeBtn;
    //FriendListPanel
    private GameObject _friendListPanel;
    //SearchPanel
    private GameObject _searchPanel;
    //用户名输入框
    private InputField _idInput;
    //添加按钮
    private Button _addFriendBtn;
    //好友列表
    private List<string> _friendList = new List<string>();
    //初始化
    public override void OnInit() {
        skinPath = "FriendPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        _closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        _friendListPanel = skin.transform.Find("FriendListPanel").gameObject;
        _searchPanel = skin.transform.Find("SearchPanel").gameObject;
        _idInput = _searchPanel.transform.Find("IdInput").GetComponent<InputField>();
        _addFriendBtn = _searchPanel.transform.Find("AddFriendBtn").GetComponent<Button>();
        //监听
        _closeBtn.onClick.AddListener(OnCloseClick);
        _addFriendBtn.onClick.AddListener(OnAddFriendClick);
        //网络协议监听
        NetManager.AddMsgListener("MsgGetFriendList", OnMsgGetFriendList);
        NetManager.AddMsgListener("MsgDeleteFriend", OnMsgDeleteFriend);
        NetManager.AddMsgListener("MsgAddFriend", OnMsgAddFriend);
        NetManager.AddMsgListener("MsgAcceptAddFriend", OnMsgAcceptAddFriend);
        //发送获取好友列表协议
        MsgGetFriendList msgGetFriendList = new MsgGetFriendList();
        NetManager.Send(msgGetFriendList);
    }

    //添加好友按钮事件
    private void OnAddFriendClick() {
        MsgAddFriend msgAddFriend = new MsgAddFriend();
        msgAddFriend.id = GameMain.id;
        msgAddFriend.friendId = _idInput.transform.Find("Text").GetComponent<Text>().text;
        NetManager.Send(msgAddFriend);
    }

    //当按下关闭按钮
    public void OnCloseClick() {
        Close();
    }

    //接受好友申请回调
    private void OnMsgAcceptAddFriend(MsgBase msg) {
        MsgAcceptAddFriend msgAcceptAddFriend = (MsgAcceptAddFriend)msg;

        UpdataFriendList();

    }

    //收到添加好友协议
    private void OnMsgAddFriend(MsgBase msg) {
        MsgAddFriend msgAddFriend = (MsgAddFriend)msg;
        //是添加人显示结果
        if (msgAddFriend.id == GameMain.id) {
            if (msgAddFriend.result == 0) PanelManager.Open<TipPanel>("已发送添加请求");
            if (msgAddFriend.result == 1) PanelManager.Open<TipPanel>("玩家不存在");
            if (msgAddFriend.result == 2) PanelManager.Open<TipPanel>("玩家不在线");
            if (msgAddFriend.result == 3) PanelManager.Open<TipPanel>("此玩家已经是您的好友");
        }
    }

    //收到删除好友协议
    private void OnMsgDeleteFriend(MsgBase msg) {
        MsgDeleteFriend msgDeleteFriend = (MsgDeleteFriend)msg;
        if (msgDeleteFriend.id == GameMain.id) {
            if (msgDeleteFriend.result == 0) {
                PanelManager.Open<TipPanel>("删除好友成功！");
                UpdataFriendList();
            }
            else {
                PanelManager.Open<TipPanel>("删除好友失败！");
            }
            return;
        }
    }

    //收到获取好友列表协议
    private void OnMsgGetFriendList(MsgBase msg) {
        MsgGetFriendList msgGetFriendList = (MsgGetFriendList)msg;
        foreach (string friendId in msgGetFriendList.friendIdList) {
            if (!_friendList.Contains(friendId)) {
                _friendList.Add(friendId);
            }
        }


        UpdataFriendList();
    }

    //更新好友列表
    public void UpdataFriendList() {
        for (int i = 0; i < _friendListPanel.transform.childCount; i++) {
            var child = _friendListPanel.transform.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
        GameObject friensEnumPrefab = ResManager.LoadPrefab("Prefab/UI/FriendEnum");
        int j = 0;
        foreach (string friendId in _friendList) {
            int index = j;
            GameObject friensEnum = (GameObject)Instantiate(friensEnumPrefab);
            Debug.Log(friensEnum.GetInstanceID());
            friensEnum.transform.SetParent(_friendListPanel.transform, false);
            friensEnum.transform.Find("ID").GetComponent<Text>().text = friendId;
            Button deleteFriendBtn = friensEnum.transform.Find("Button").GetComponent<Button>();
            deleteFriendBtn.onClick.AddListener(() => { OnDeleteFriendClick(index); });
            j++;
        }
    }

    //删除好友按钮事件
    private void OnDeleteFriendClick(int index) {
        MsgDeleteFriend msgDeleteFriend = new MsgDeleteFriend();
        msgDeleteFriend.friendId = _friendList[index];
        msgDeleteFriend.id = GameMain.id;
        NetManager.Send(msgDeleteFriend);

    }



    //关闭
    public override void OnClose() {
        NetManager.RemoveMsgListener("MsgGetFriendList", OnMsgGetFriendList);
        NetManager.RemoveMsgListener("MsgDeleteFriend", OnMsgDeleteFriend);
        NetManager.RemoveMsgListener("MsgAddFriend", OnMsgAddFriend);
        NetManager.RemoveMsgListener("MsgAcceptAddFriend", OnMsgAcceptAddFriend);
    }
}
