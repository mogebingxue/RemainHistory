using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    //玩家头像
    private Image _headPhoto;
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
    //切换聊天模式按钮
    private Button _switchBtn;
    //发送聊天消息按钮
    private Button _sendBtn;
    //设置按钮
    private Button _setBtn;
    //修改简介的按钮
    private Button _modifyBtn;
    //修改简介的输入框
    private InputField _modifyInput;
    //0 世界 1 好友
    private int _sendStatus;
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
        _headPhoto = skin.transform.Find("PlayerInfo").Find("HeadPhoto").GetComponent<Image>();
        _playerName = skin.transform.Find("PlayerInfo").Find("PlayerName").GetComponent<Text>();
        _playerIntroduction = skin.transform.Find("PlayerInfo").Find("PlayerIntroduction").GetComponent<Text>();
        _enterGameBtn = skin.transform.Find("EnterGameBtn").GetComponent<Button>();
        _handbookBtn = skin.transform.Find("HandbookBtn").GetComponent<Button>();
        _altarBtn = skin.transform.Find("AltarBtn").GetComponent<Button>();
        _friendBtn = skin.transform.Find("FriendBtn").GetComponent<Button>();
        _sendBtn = skin.transform.Find("ChatWindow").Find("SendBtn").GetComponent<Button>();
        _switchBtn = skin.transform.Find("ChatWindow").Find("SwitchBtn").GetComponent<Button>();
        _input = skin.transform.Find("ChatWindow").Find("InputField").GetComponent<InputField>();
        _modifyBtn = skin.transform.Find("PlayerInfo").Find("ModifyBtn").GetComponent<Button>();
        _modifyInput = _playerIntroduction.transform.Find("InputField").GetComponent<InputField>();
        _worldContent = skin.transform.Find("ChatWindow").Find("WorldChat").Find("Scroll View").Find("Viewport").Find("Content").gameObject;


        //监听
        _enterGameBtn.onClick.AddListener(OnEnterGameClick);
        _handbookBtn.onClick.AddListener(OnHandbookClick);
        _altarBtn.onClick.AddListener(OnAltarClick);
        _friendBtn.onClick.AddListener(OnFriendClick);
        _sendBtn.onClick.AddListener(OnSendClick);
        _switchBtn.onClick.AddListener(OnSwitchClick);
        _modifyBtn.onClick.AddListener(OnModifyClick);
        _setBtn.onClick.AddListener(OnSetClick);
        skin.transform.Find("PlayerInfo").Find("HeadPhoto").GetComponent<Button>().onClick.AddListener(OnHeadPhotoClick);
        //设置用户名
        _playerName.text = GameMain.id;
        //设置频道
        _sendStatus = 0;

        //网络协议监听
        NetManager.AddMsgListener("MsgGetPlayerIntroduction", OnMsgGetPlayerIntroduction);
        NetManager.AddMsgListener("MsgSavePlayerIntroduction", OnMsgSavePlayerIntroduction);
        NetManager.AddMsgListener("MsgSendMessageToWord", OnMsgSendMessageToWord);
        NetManager.AddMsgListener("MsgSendMessageToFriend", OnMsgSendMessageToFriend);
        NetManager.AddMsgListener("MsgGetHeadPhoto", OnMsgGetHeadPhoto);
        NetManager.AddMsgListener("MsgSaveHeadPhoto", OnMsgSaveHeadPhoto);
        //获取个人数据库中玩家信息
        MsgGetPlayerIntroduction msgGetPlayerIntroduction = new MsgGetPlayerIntroduction();
        NetManager.Send(msgGetPlayerIntroduction);
        MsgGetHeadPhoto msgGetHeadPhoto = new MsgGetHeadPhoto();
        NetManager.Send(msgGetHeadPhoto);

    }

    




    //保存简介回调
    private void OnMsgSavePlayerIntroduction(MsgBase msg) {
        Debug.Log("保存个人简介成功");
    }

    //获得简介回调
    private void OnMsgGetPlayerIntroduction(MsgBase msg) {
        MsgGetPlayerIntroduction msgGetPlayerIntroduction = (MsgGetPlayerIntroduction)msg;
        _playerIntroduction.text = msgGetPlayerIntroduction.palyerIntroduction;
    }

    //获取头像回调
    private void OnMsgGetHeadPhoto(MsgBase msg) {
        MsgGetHeadPhoto msgGetHeadPhoto = (MsgGetHeadPhoto)msg;
        _headPhoto.sprite = ResManager.LoadTexture("Texture/HeadPhoto" + msgGetHeadPhoto.headPhoto);

    }

    //保存头像回调
    private void OnMsgSaveHeadPhoto(MsgBase msg) {
        MsgSaveHeadPhoto msgSaveHeadPhoto = (MsgSaveHeadPhoto)msg;
        _headPhoto.sprite = ResManager.LoadTexture("Texture/HeadPhoto" + msgSaveHeadPhoto.headPhoto);
    }

    //发送世界消息回调
    private void OnMsgSendMessageToWord(MsgBase msg) {
        MsgSendMessageToWord msgSendMessageToWord = (MsgSendMessageToWord)msg;
        GameObject messagePrefab;
        if (msgSendMessageToWord.id != GameMain.id) {
            messagePrefab = ResManager.LoadPrefab("Prefab/UI/OtherMessagePanel");
        }
        else {
            messagePrefab = ResManager.LoadPrefab("Prefab/UI/MyMessagePanel");
        }
        GameObject message = (GameObject)Instantiate(messagePrefab);
        message.transform.SetParent(_worldContent.transform, false);
        message.transform.Find("Text").GetComponent<Text>().text = msgSendMessageToWord.message;
        
        Debug.Log(msgSendMessageToWord.id + msgSendMessageToWord.message);

    }
    //发送好友消息回调
    private void OnMsgSendMessageToFriend(MsgBase msgBase) {
        throw new NotImplementedException();
    }

    //点击头像事件
    private void OnHeadPhotoClick() {
        PanelManager.Open<HeadPhotoPanel>();
        MsgSaveHeadPhoto msgSaveHeadPhoto = new MsgSaveHeadPhoto();
        string[] headPhoto = _headPhoto.sprite.name.Split('o');
        msgSaveHeadPhoto.headPhoto = int.Parse(headPhoto[2]);
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
            msgSavePlayerIntroduction.palyerIntroduction = _playerIntroduction.text;
            NetManager.Send(msgSavePlayerIntroduction);
            _modifyInput.gameObject.SetActive(false);
            return;
        }

    }
    //切换频道按钮按下
    private void OnSwitchClick() {
        if (_sendStatus == 0) {
            _sendStatus = 1;
            _switchBtn.transform.Find("Text").GetComponent<Text>().text = "好友";
            return;
        }
        if (_sendStatus == 1) {
            _sendStatus = 0;
            _switchBtn.transform.Find("Text").GetComponent<Text>().text = "世界";
            return;
        }
    }

    

    //发送消息按钮按下
    private void OnSendClick() {
        if (_sendStatus == 0) {
            //世界频道
            MsgSendMessageToWord msgSendMessageToWord = new MsgSendMessageToWord();
            msgSendMessageToWord.message = _input.transform.Find("Text").GetComponent<Text>().text;
            _input.transform.Find("Text").GetComponent<Text>().text = "";
            _input.GetComponent<InputField>().text = "";
            msgSendMessageToWord.id = GameMain.id;
            NetManager.Send(msgSendMessageToWord);

            return;
        }
        if (_sendStatus == 1) {
            //好友频道
        }


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
        Close();
    }

    //关闭
    public override void OnClose() {
        NetManager.RemoveMsgListener("MsgGetPlayerIntroduction", OnMsgGetPlayerIntroduction);
        NetManager.RemoveMsgListener("MsgSavePlayerIntroduction", OnMsgSavePlayerIntroduction);
        NetManager.RemoveMsgListener("MsgSendMessageToWord", OnMsgSendMessageToWord);
        NetManager.RemoveMsgListener("MsgSendMessageToFriend", OnMsgSendMessageToFriend);
    }
}
