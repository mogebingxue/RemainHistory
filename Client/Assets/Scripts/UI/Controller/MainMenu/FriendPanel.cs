using Game;
using System;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
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
        _friendListPanel = skin.transform.Find("FriendListPanel/Scroll View/Viewport/Content").gameObject;
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
        msgAddFriend.Id = GameMain.id;
        msgAddFriend.FriendId = _idInput.transform.Find("Text").GetComponent<Text>().text;
        NetManager.Send(msgAddFriend);
    }

    //当按下关闭按钮
    public void OnCloseClick() {
        Hide();
    }

    //接受好友申请回调
    private void OnMsgAcceptAddFriend(Request request) {
        MsgAcceptAddFriend msgAcceptAddFriend = MsgAcceptAddFriend.Parser.ParseFrom(request.Msg);

        UpdataFriendList();

    }

    //收到添加好友协议
    private void OnMsgAddFriend(Request request) {
        MsgAddFriend msgAddFriend = MsgAddFriend.Parser.ParseFrom(request.Msg);
        //是添加人显示结果
        if (msgAddFriend.Id == GameMain.id) {
            if (msgAddFriend.Result == 0) PanelManager.Open<TipPanel>("已发送添加请求");
            if (msgAddFriend.Result == 1) PanelManager.Open<TipPanel>("玩家不存在");
            if (msgAddFriend.Result == 2) PanelManager.Open<TipPanel>("玩家不在线");
            if (msgAddFriend.Result == 3) PanelManager.Open<TipPanel>("此玩家已经是您的好友");
        }
    }

    //收到删除好友协议
    private void OnMsgDeleteFriend(Request request) {
        MsgDeleteFriend msgDeleteFriend = MsgDeleteFriend.Parser.ParseFrom(request.Msg);
        if (msgDeleteFriend.Id == GameMain.id) {
            if (msgDeleteFriend.Result == 0) {
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
    private void OnMsgGetFriendList(Request request) {
        MsgGetFriendList msgGetFriendList = MsgGetFriendList.Parser.ParseFrom(request.Msg);
        string[] fl = msgGetFriendList.FriendList.Split(',');
        foreach (string friendId in fl) {
            if ((!_friendList.Contains(friendId))&&friendId!="") {
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
        GameObject friensEnumPrefab = ABManager.Instance.LoadRes<GameObject>("prefab/ui", "FriendEnum");
        int j = 0;
        foreach (string friendId in _friendList) {
            int index = j;
            GameObject friensEnum = (GameObject)Instantiate(friensEnumPrefab);
            friensEnum.transform.SetParent(_friendListPanel.transform, false);
            friensEnum.transform.Find("ID").GetComponent<Text>().text = friendId;
            //获取并注册删除好友按钮事件
            Button deleteFriendBtn = friensEnum.transform.Find("DeleteButton").GetComponent<Button>();
            deleteFriendBtn.onClick.AddListener(() => { OnDeleteFriendClick(index); });
            //判断好友是否在线，然后呼出好友聊天的按钮是否禁用
            //TODO

            //获取并注册呼出好友按钮事件
            Button openFriendButton = friensEnum.transform.Find("OpenFriendButton").GetComponent<Button>();
            openFriendButton.onClick.AddListener(() => { onOpenFriendButtonClicj(index); });
            j++;
        }

    }

    //呼出好友按钮事件
    private void onOpenFriendButtonClicj(int index) {
        PanelManager.Open<ChatWindow>(_friendList[index]);
    }

    //删除好友按钮事件
    private void OnDeleteFriendClick(int index) {
        MsgDeleteFriend msgDeleteFriend = new MsgDeleteFriend();
        msgDeleteFriend.FriendId = _friendList[index];
        msgDeleteFriend.Id = GameMain.id;
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
