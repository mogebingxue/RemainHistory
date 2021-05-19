using Game;
using UIFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameMain : MonoSingleton<GameMain>
{
    public static string id = "";


    //加载场景时不销毁的物体
    //public static List<GameObject> DontDestroyObjects;

    void Awake() {
        //初始化
        DontDestroyOnLoad(gameObject);

    }

    // Use this for initialization
    void Start() {
        //网络监听
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        NetManager.AddMsgListener("MsgAddFriend", OnMsgAddFriend);
        NetManager.AddMsgListener("MsgAcceptAddFriend", OnMsgAcceptAddFriend);
        NetManager.AddMsgListener("MsgDeleteFriend", OnMsgDeleteFriend);

        PanelManager.Init();

        //打开登陆面板
        SceneManager.LoadSceneAsync("LoginMenu");

        //打开登陆面板
        PanelManager.Open<LoginPanel>();
    }
    //收到添加好友协议
    private void OnMsgAddFriend(Request request) {
        MsgAddFriend msgAddFriend = MsgAddFriend.Parser.ParseFrom(request.Msg);
        //是被添加人显示是否同意添加
        if (msgAddFriend.FriendId == id) {
            PanelManager.Open<FriendApplyPanel>("是否同意" + msgAddFriend.Id + "的好友申请？");
            FriendApplyPanel friendApplyPanel = (FriendApplyPanel)PanelManager.panels["FriendApplyPanel"];
            friendApplyPanel.id = msgAddFriend.FriendId;
            friendApplyPanel.friendId = msgAddFriend.Id;
        }
    }

    //接受好友申请回调
    private void OnMsgAcceptAddFriend(Request request) {
        MsgAcceptAddFriend msgAcceptAddFriend = MsgAcceptAddFriend.Parser.ParseFrom(request.Msg);
        if (msgAcceptAddFriend.FriendId == GameMain.id) {
            PanelManager.Open<TipPanel>("添加" + msgAcceptAddFriend.Id + "成功！");
        }
        if (PanelManager.panels.ContainsKey("FriendPanel")) {
            FriendPanel friendPanel = (FriendPanel)PanelManager.panels["FriendPanel"];
            friendPanel.UpdataFriendList();
        }
    }

    //收到删除好友协议
    private void OnMsgDeleteFriend(Request request) {
        MsgDeleteFriend msgDeleteFriend = MsgDeleteFriend.Parser.ParseFrom(request.Msg);
        if (msgDeleteFriend.FriendId == GameMain.id) {
            if (msgDeleteFriend.Result == 0) {
                PanelManager.Open<TipPanel>("你已被" + msgDeleteFriend.Id + "删除好友！");
                return;
            }
        }
        if (PanelManager.panels.ContainsKey("FriendPanel")) {
            FriendPanel friendPanel = (FriendPanel)PanelManager.panels["FriendPanel"];
            friendPanel.UpdataFriendList();
        }
    }

    // Update is called once per frame
    void Update() {
        NetManager.Update();
    }

    //关闭连接
    void OnConnectClose(string err) {
        Debug.Log("断开连接");
    }

    //被踢下线
    void OnMsgKick(Request request) {
        NetManager.Close();
        Transform root = GameObject.Find("Root").transform;
        Transform canvas = root.Find("Canvas");
        Transform panel = canvas.Find("Panel");
        Transform tip = canvas.Find("Tip");

        for (int i = 0; i < panel.childCount; i++) {
            var child = panel.GetChild(i);

            PanelManager.Close(child.name);
            string[] name = child.name.Split('(');
            PanelManager.Close(name[0]);
            Debug.Log(name[0]);
        }
        for (int i = 0; i < tip.childCount; i++) {
            var child = tip.GetChild(i);
            string[] name = child.name.Split('(');
            PanelManager.Close(name[0]);
            Debug.Log(name[0]);
        }
        SceneManager.LoadSceneAsync("LoginMenu");
        PanelManager.Open<LoginPanel>();
        PanelManager.Open<TipPanel>("被踢下线");

    }
}

