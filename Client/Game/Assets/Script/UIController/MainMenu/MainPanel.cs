using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    //玩家头像
    private Image playerImage;
    //玩家名字
    private Text playerName;
    //玩家简介
    private Text playerIntroduction;
    //发送消息输入框
    private InputField input;
    //进入游戏按钮
    private Button enterGameBtn;
    //图鉴按钮
    private Button handbookBtn;
    //祖庙按钮
    private Button altarBtn;
    //好友按钮
    private Button friendBtn;
    //切换聊天模式按钮
    private Button switchBtn;
    //发送聊天消息按钮
    private Button sendBtn;
    //设置按钮
    private Button setBtn;
    //修改简介的按钮
    private Button modifyBtn;
    //修改简介的输入框
    private InputField modifyInput;
    //0 世界 1 好友
    private int sendStatus;

    private GameObject worldContent;

    //初始化
    public override void OnInit() {
        skinPath = "MainPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] para) {
        
        PlayerPrefs.SetFloat(Const.Volume, 0.5f);
        AudioListener.volume = PlayerPrefs.GetFloat(Const.Volume);
        //寻找组件
        setBtn = skin.transform.Find("SetBtn").GetComponent<Button>();
        playerImage = skin.transform.Find("PlayerInfo").Find("PlayerImage").GetComponent<Image>();
        playerName = skin.transform.Find("PlayerInfo").Find("PlayerName").GetComponent<Text>();
        playerIntroduction = skin.transform.Find("PlayerInfo").Find("PlayerIntroduction").GetComponent<Text>();
        enterGameBtn = skin.transform.Find("EnterGameBtn").GetComponent<Button>();
        handbookBtn = skin.transform.Find("HandbookBtn").GetComponent<Button>();
        altarBtn = skin.transform.Find("AltarBtn").GetComponent<Button>();
        friendBtn = skin.transform.Find("FriendBtn").GetComponent<Button>();
        sendBtn = skin.transform.Find("ChatWindow").Find("SendBtn").GetComponent<Button>();
        switchBtn = skin.transform.Find("ChatWindow").Find("SwitchBtn").GetComponent<Button>();
        input = skin.transform.Find("ChatWindow").Find("InputField").GetComponent<InputField>();
        modifyBtn = skin.transform.Find("PlayerInfo").Find("ModifyBtn").GetComponent<Button>();
        modifyInput = playerIntroduction.transform.Find("InputField").GetComponent<InputField>();
        worldContent = skin.transform.Find("ChatWindow").Find("WorldChat").Find("Scroll View").Find("Viewport").Find("Content").gameObject;


        //监听
        enterGameBtn.onClick.AddListener(OnEnterGameClick);
        handbookBtn.onClick.AddListener(OnHandbookClick);
        altarBtn.onClick.AddListener(OnAltarClick);
        friendBtn.onClick.AddListener(OnFriendClick);
        sendBtn.onClick.AddListener(OnSendClick);
        switchBtn.onClick.AddListener(OnSwitchClick);
        modifyBtn.onClick.AddListener(OnModifyClick);
        setBtn.onClick.AddListener(OnSetClick);

        //设置用户名
        playerName.text = GameMain.id;
        //设置频道
        sendStatus = 0;

        //网络协议监听
        NetManager.AddMsgListener("MsgGetPlayerIntroduction", OnMsgGetPlayerIntroduction);
        NetManager.AddMsgListener("MsgSavePlayerIntroduction", OnMsgSavePlayerIntroduction);
        NetManager.AddMsgListener("MsgSendMessageToWord", OnMsgSendMessageToWord);
        NetManager.AddMsgListener("MsgSendMessageToFriend", OnMsgSendMessageToFriend);

        //获取个人数据库中简介信息
        MsgGetPlayerIntroduction msgGetPlayerIntroduction = new MsgGetPlayerIntroduction();
        NetManager.Send(msgGetPlayerIntroduction);

    }

    


    //保存简介回调
    private void OnMsgSavePlayerIntroduction(MsgBase msg) {
        Debug.Log("保存个人简介成功");
    }
    //获得简介回调
    private void OnMsgGetPlayerIntroduction(MsgBase msg) {
        MsgGetPlayerIntroduction msgGetPlayerIntroduction = (MsgGetPlayerIntroduction)msg;
        playerIntroduction.text = msgGetPlayerIntroduction.palyerIntroduction;
    }
    //发送世界消息回调
    private void OnMsgSendMessageToWord(MsgBase msg) {
        MsgSendMessageToWord msgSendMessageToWord = (MsgSendMessageToWord)msg;
        GameObject messagePrefab;
        if (msgSendMessageToWord.id != GameMain.id) {
            messagePrefab = ResManager.LoadPrefab("OtherMessagePanel");
        }
        else {
            messagePrefab = ResManager.LoadPrefab("MyMessagePanel");
        }
        GameObject message = (GameObject)Instantiate(messagePrefab);
        message.transform.SetParent(worldContent.transform, false);
        message.transform.Find("Text").GetComponent<Text>().text = msgSendMessageToWord.message;
        
        Debug.Log(msgSendMessageToWord.id + msgSendMessageToWord.message);

    }
    //发送好友消息回调
    private void OnMsgSendMessageToFriend(MsgBase msgBase) {
        throw new NotImplementedException();
    }




    //编辑简介按钮按下
    private void OnModifyClick() {

        //编辑的时候
        if (modifyBtn.transform.Find("Text").GetComponent<Text>().text == "编辑") {

            modifyInput.gameObject.SetActive(true);
            modifyBtn.transform.Find("Text").GetComponent<Text>().text = "确认";
            modifyInput.transform.Find("Text").GetComponent<Text>().text = playerIntroduction.text;
            return;
        }
        //确认的时候
        if (modifyBtn.transform.Find("Text").GetComponent<Text>().text == "确认") {

            modifyBtn.transform.Find("Text").GetComponent<Text>().text = "编辑";
            playerIntroduction.text = modifyInput.transform.Find("Text").GetComponent<Text>().text;
            MsgSavePlayerIntroduction msgSavePlayerIntroduction = new MsgSavePlayerIntroduction();
            msgSavePlayerIntroduction.palyerIntroduction = playerIntroduction.text;
            NetManager.Send(msgSavePlayerIntroduction);
            modifyInput.gameObject.SetActive(false);
            return;
        }

    }
    //切换频道按钮按下
    private void OnSwitchClick() {
        if (sendStatus == 0) {
            sendStatus = 1;
            switchBtn.transform.Find("Text").GetComponent<Text>().text = "好友";
            return;
        }
        if (sendStatus == 1) {
            sendStatus = 0;
            switchBtn.transform.Find("Text").GetComponent<Text>().text = "世界";
            return;
        }
    }

    

    //发送消息按钮按下
    private void OnSendClick() {
        if (sendStatus == 0) {
            //世界频道
            MsgSendMessageToWord msgSendMessageToWord = new MsgSendMessageToWord();
            msgSendMessageToWord.message = input.transform.Find("Text").GetComponent<Text>().text;
            input.transform.Find("Text").GetComponent<Text>().text = "";
            input.GetComponent<InputField>().text = "";
            msgSendMessageToWord.id = GameMain.id;
            NetManager.Send(msgSendMessageToWord);

            return;
        }
        if (sendStatus == 1) {
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
