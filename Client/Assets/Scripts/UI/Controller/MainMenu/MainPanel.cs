﻿using Game;
using System;
using UIFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    //玩家头像
    private Image _headPhoto;
    //玩家头像名称
    private string _headPhotoName;
    //玩家名字
    private Text _playerName;
    //玩家简介
    private Text _playerIntroduction;
    //发送消息输入框
    private InputField _input;
    //进入游戏按钮
    private Button _enterGameBtn;
    //图鉴按钮
    private Button _handbookBtn;
    //祖庙按钮
    private Button _altarBtn;
    //好友按钮
    private Button _friendBtn;
    //发送聊天消息按钮
    private Button _sendBtn;
    //设置按钮
    private Button _setBtn;
    //修改简介的按钮
    private Button _modifyBtn;
    //修改简介的输入框
    private InputField _modifyInput;
    //世界聊天框
    private GameObject _worldContent;
    //初始化
    public override void OnInit() {
        skinPath = "MainPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] para) {
        //设置声音
        if (PlayerPrefs.GetString(Const.IsOnVolume) == "") {
            PlayerPrefs.SetString(Const.IsOnVolume, "true");
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetString(Const.IsOnVolume) == "true") {
            AudioListener.volume = PlayerPrefs.GetFloat(Const.Volume);
        }
        if (PlayerPrefs.GetString(Const.IsOnVolume) == "false") {
            AudioListener.volume = 0f;
        }

        //寻找组件
        _setBtn = skin.transform.Find("SetBtn").GetComponent<Button>();
        _headPhoto = skin.transform.Find("PlayerInfo/HeadPhoto").GetComponent<Image>();
        _playerName = skin.transform.Find("PlayerInfo/PlayerName").GetComponent<Text>();
        _playerIntroduction = skin.transform.Find("PlayerInfo/PlayerIntroduction").GetComponent<Text>();
        _enterGameBtn = skin.transform.Find("EnterGameBtn").GetComponent<Button>();
        _handbookBtn = skin.transform.Find("HandbookBtn").GetComponent<Button>();
        _altarBtn = skin.transform.Find("AltarBtn").GetComponent<Button>();
        _friendBtn = skin.transform.Find("FriendBtn").GetComponent<Button>();
        _sendBtn = skin.transform.Find("ChatWindow/SendBtn").GetComponent<Button>();
        _input = skin.transform.Find("ChatWindow/InputField").GetComponent<InputField>();
        _modifyBtn = skin.transform.Find("PlayerInfo/ModifyBtn").GetComponent<Button>();
        _modifyInput = _playerIntroduction.transform.Find("InputField").GetComponent<InputField>();
        _worldContent = skin.transform.Find("ChatWindow/WorldChat/Scroll View/Viewport/Content").gameObject;


        //监听
        _enterGameBtn.onClick.AddListener(OnEnterGameClick);
        _handbookBtn.onClick.AddListener(OnHandbookClick);
        _altarBtn.onClick.AddListener(OnAltarClick);
        _friendBtn.onClick.AddListener(OnFriendClick);
        _sendBtn.onClick.AddListener(OnSendClick);
        _modifyBtn.onClick.AddListener(OnModifyClick);
        _setBtn.onClick.AddListener(OnSetClick);
        skin.transform.Find("PlayerInfo/HeadPhoto").GetComponent<Button>().onClick.AddListener(OnHeadPhotoClick);
        //设置用户名
        _playerName.text = GameMain.id;

        //网络协议监听
        NetManager.AddMsgListener("MsgGetPlayerIntroduction", OnMsgGetPlayerIntroduction);
        NetManager.AddMsgListener("MsgSavePlayerIntroduction", OnMsgSavePlayerIntroduction);
        NetManager.AddMsgListener("MsgSendMessageToWord", OnMsgSendMessageToWord);
        NetManager.AddMsgListener("MsgGetHeadPhoto", OnMsgGetHeadPhoto);
        NetManager.AddMsgListener("MsgSaveHeadPhoto", OnMsgSaveHeadPhoto);
        //获取个人数据库中玩家信息
        MsgGetPlayerIntroduction msgGetPlayerIntroduction = new MsgGetPlayerIntroduction();
        NetManager.Send(msgGetPlayerIntroduction);
        MsgGetHeadPhoto msgGetHeadPhoto = new MsgGetHeadPhoto();
        NetManager.Send(msgGetHeadPhoto);

    }






    //保存简介回调
    private void OnMsgSavePlayerIntroduction(Request request) {
        Debug.Log("保存个人简介成功");
    }

    //获得简介回调
    private void OnMsgGetPlayerIntroduction(Request request) {
        MsgGetPlayerIntroduction msgGetPlayerIntroduction = MsgGetPlayerIntroduction.Parser.ParseFrom(request.Msg);
        _playerIntroduction.text = msgGetPlayerIntroduction.PlayerIntroduction;
    }

    //获取头像回调
    private void OnMsgGetHeadPhoto(Request request) {
        MsgGetHeadPhoto msgGetHeadPhoto = MsgGetHeadPhoto.Parser.ParseFrom(request.Msg);
        _headPhotoName = "HeadPhoto" + msgGetHeadPhoto.HeadPhoto;
        Texture2D texture = ABManager.Instance.LoadRes<Texture2D>("texture", _headPhotoName);
        _headPhoto.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    //保存头像回调
    private void OnMsgSaveHeadPhoto(Request request) {
        MsgSaveHeadPhoto msgSaveHeadPhoto = MsgSaveHeadPhoto.Parser.ParseFrom(request.Msg);
        _headPhotoName = "HeadPhoto" + msgSaveHeadPhoto.HeadPhoto;
        Texture2D texture = ABManager.Instance.LoadRes<Texture2D>("texture", _headPhotoName);
        _headPhoto.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    //发送世界消息回调
    private void OnMsgSendMessageToWord(Request request) {
        MsgSendMessageToWord msgSendMessageToWord = MsgSendMessageToWord.Parser.ParseFrom(request.Msg);
        GameObject messagePrefab;
        if (msgSendMessageToWord.Id != GameMain.id) {
            messagePrefab = ABManager.Instance.LoadRes<GameObject>("prefab/ui", "OtherMessagePanel");
        }
        else {
            messagePrefab = ABManager.Instance.LoadRes<GameObject>("prefab/ui", "MyMessagePanel");
        }
        GameObject message = (GameObject)Instantiate(messagePrefab);
        message.transform.SetParent(_worldContent.transform, false);
        message.transform.Find("Text").GetComponent<Text>().text = msgSendMessageToWord.Message;

        Debug.Log(msgSendMessageToWord.Id + msgSendMessageToWord.Message);

    }
   

    //点击头像事件
    private void OnHeadPhotoClick() {
        PanelManager.Open<HeadPhotoPanel>();
        MsgSaveHeadPhoto msgSaveHeadPhoto = new MsgSaveHeadPhoto();
        Debug.Log(_headPhotoName);
        string[] headPhoto = _headPhotoName.Split('o');
        msgSaveHeadPhoto.HeadPhoto = int.Parse(headPhoto[2]);
        NetManager.Send(msgSaveHeadPhoto);
    }


    //编辑简介按钮按下
    private void OnModifyClick() {

        //编辑的时候
        if (_modifyBtn.transform.Find("Text").GetComponent<Text>().text == "编辑") {

            _modifyInput.gameObject.SetActive(true);
            _modifyBtn.transform.Find("Text").GetComponent<Text>().text = "确认";
            _modifyInput.transform.Find("Text").GetComponent<Text>().text = _playerIntroduction.text;
            return;
        }
        //确认的时候
        if (_modifyBtn.transform.Find("Text").GetComponent<Text>().text == "确认") {

            _modifyBtn.transform.Find("Text").GetComponent<Text>().text = "编辑";
            _playerIntroduction.text = _modifyInput.transform.Find("Text").GetComponent<Text>().text;
            MsgSavePlayerIntroduction msgSavePlayerIntroduction = new MsgSavePlayerIntroduction();
            msgSavePlayerIntroduction.PalyerIntroduction = _playerIntroduction.text;
            NetManager.Send(msgSavePlayerIntroduction);
            _modifyInput.gameObject.SetActive(false);
            return;
        }

    }



    //发送消息按钮按下
    private void OnSendClick() {
        //世界频道
        MsgSendMessageToWord msgSendMessageToWord = new MsgSendMessageToWord();
        msgSendMessageToWord.Message = _input.transform.Find("Text").GetComponent<Text>().text;
        _input.transform.Find("Text").GetComponent<Text>().text = "";
        _input.GetComponent<InputField>().text = "";
        msgSendMessageToWord.Id = GameMain.id;
        NetManager.Send(msgSendMessageToWord);
    }

    //好友按钮
    private void OnFriendClick() {
        PanelManager.Open<FriendPanel>();
    }
    //祖庙按钮
    private void OnAltarClick() {
        PanelManager.Open<AltarPanel>();
    }
    //图鉴按钮
    private void OnHandbookClick() {
        PanelManager.Open<HandbookPanel>();
    }
    //点击设置按钮
    private void OnSetClick() {
        PanelManager.Open<SetPanel>();

    }
    //进入游戏按钮
    private void OnEnterGameClick() {
        //进入游戏场景
        SceneManager.LoadSceneAsync("Game");


        //关闭界面
        Hide();
    }

    //关闭
    public override void OnClose() {
        NetManager.RemoveMsgListener("MsgGetPlayerIntroduction", OnMsgGetPlayerIntroduction);
        NetManager.RemoveMsgListener("MsgSavePlayerIntroduction", OnMsgSavePlayerIntroduction);
        NetManager.RemoveMsgListener("MsgSendMessageToWord", OnMsgSendMessageToWord);
    }
}
