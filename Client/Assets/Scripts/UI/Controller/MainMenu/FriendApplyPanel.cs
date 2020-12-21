using UIFramework;
using UnityEngine.UI;

public class FriendApplyPanel : BasePanel
{
    //提示文本
    private Text _text;
    //确定按钮
    private Button _okBtn;
    //关闭按钮
    private Button _closeBtn;
    //信息
    public string id;
    public string friendId;
    //初始化
    public override void OnInit() {
        skinPath = "FriendApplyPanel";
        layer = PanelManager.Layer.Tip;
    }
    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        _text = skin.transform.Find("Text").GetComponent<Text>();
        _okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
        _closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        //监听
        _okBtn.onClick.AddListener(OnOkClick);
        _closeBtn.onClick.AddListener(OnCloseClick);
        //网络监听
        NetManager.AddMsgListener("MsgAcceptAddFriend", OnMsgAcceptAddFriend);
        //提示语
        if (args.Length == 1) {
            _text.text = (string)args[0];
        }
    }


    //接受好友申请回调
    private void OnMsgAcceptAddFriend(MsgBase msg) {
        MsgAcceptAddFriend msgAcceptAddFriend = (MsgAcceptAddFriend)msg;
        if (msgAcceptAddFriend.id == GameMain.id) {
            if (msgAcceptAddFriend.result == 0) {
                PanelManager.Open<TipPanel>("添加成功！");
            }
            else {
                PanelManager.Open<TipPanel>("添加失败！");
            }

        }
    }

    //当按下关闭按钮
    public void OnCloseClick() {
        Hide();
    }

    //关闭
    public override void OnClose() {
        NetManager.RemoveMsgListener("MsgAcceptAddFriend", OnMsgAcceptAddFriend);
    }

    //当按下确定按钮
    public void OnOkClick() {
        MsgAcceptAddFriend msgAcceptAddFriend = new MsgAcceptAddFriend();
        msgAcceptAddFriend.id = id;
        msgAcceptAddFriend.friendId = friendId;
        NetManager.Send(msgAcceptAddFriend);
    }
}
