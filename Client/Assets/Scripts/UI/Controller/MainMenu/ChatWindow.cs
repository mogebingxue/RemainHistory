using Game;
using System;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindow : BasePanel
{
    //名字
    private Text nameText;
    //发送聊天消息按钮
    private Button sendBtn;
    //关闭按钮
    private Button closeBtn;
    //世界聊天框
    private GameObject worldContent;
    //发送消息输入框
    private InputField input;
    //初始化
    public override void OnInit() {
        skinPath = "ChatWindow";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] args) {

        sendBtn = skin.transform.Find("SendBtn").GetComponent<Button>();
        closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        worldContent = skin.transform.Find("WorldChat/Scroll View/Viewport/Content").gameObject;
        input = skin.transform.Find("InputField").GetComponent<InputField>();
        nameText = skin.transform.Find("NameText").GetComponent<Text>();
        if (args.Length >= 1) {
            nameText.text = (string)args[0];
        }


        sendBtn.onClick.AddListener(OnSendClick);
        closeBtn.onClick.AddListener(OnCloseClick);


        NetManager.AddMsgListener("MsgSendMessageToFriend", OnMsgSendMessageToFriend);
    }

    private void OnCloseClick() {
        //关闭界面
        Close();
    }

    private void OnSendClick() {
        //世界频道
        MsgSendMessageToFriend msgSendMessageToFriend = new MsgSendMessageToFriend();
        msgSendMessageToFriend.Message = input.transform.Find("Text").GetComponent<Text>().text;
        input.transform.Find("Text").GetComponent<Text>().text = "";
        input.GetComponent<InputField>().text = "";
        msgSendMessageToFriend.Id = GameMain.id;
        msgSendMessageToFriend.FriendId = nameText.text;
        NetManager.Send(msgSendMessageToFriend);
    }

    //发送好友消息回调
    private void OnMsgSendMessageToFriend(Request request) {
        MsgSendMessageToFriend msgSendMessageToFriend = MsgSendMessageToFriend.Parser.ParseFrom(request.Msg);
        GameObject messagePrefab;
        if (msgSendMessageToFriend.Result == 1) {
            PanelManager.Open<TipPanel>("发送失败");
            return;
        }
        if (msgSendMessageToFriend.Id != GameMain.id) {
            messagePrefab = ABManager.Instance.LoadRes<GameObject>("prefab/ui", "OtherMessagePanel");
        }
        else {
            messagePrefab = ABManager.Instance.LoadRes<GameObject>("prefab/ui", "MyMessagePanel");
        }
        GameObject message = (GameObject)Instantiate(messagePrefab);
        message.transform.SetParent(worldContent.transform, false);
        message.transform.Find("Text").GetComponent<Text>().text = msgSendMessageToFriend.Message;

        Debug.Log(msgSendMessageToFriend.Id + msgSendMessageToFriend.Message);
    }

    //关闭
    public override void OnClose() {
        NetManager.RemoveMsgListener("MsgSendMessageToFriend", OnMsgSendMessageToFriend);
    }
}
