using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMain : MonoBehaviour
{
    public static string id = "";


    //加载场景时不销毁的物体
    public static List<GameObject> DontDestroyObjects;

    void Awake() {
        //初始化
        PanelManager.Init();
        //DontDestroyObjects.Add(gameObject);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("UIRoot"));
    }
    // Use this for initialization
    void Start() {
        
        //网络监听
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        NetManager.AddMsgListener("MsgAddFriend", OnMsgAddFriend);
        NetManager.AddMsgListener("MsgAcceptAddFriend", OnMsgAcceptAddFriend);
        NetManager.AddMsgListener("MsgDeleteFriend", OnMsgDeleteFriend);
        //打开登陆面板
        SceneManager.LoadSceneAsync("LoginMenu");
        
        //打开登陆面板
        PanelManager.Open<LoginPanel>();
    }
    //收到添加好友协议
    private void OnMsgAddFriend(MsgBase msg) {
        MsgAddFriend msgAddFriend = (MsgAddFriend)msg;
        //是被添加人显示是否同意添加
        if (msgAddFriend.friendId == GameMain.id) {
            PanelManager.Open<FriendApplyPanel>("是否同意"+msgAddFriend.id+"的好友申请？");
            FriendApplyPanel friendApplyPanel = (FriendApplyPanel)PanelManager.panels["FriendApplyPanel"];
            friendApplyPanel.id = msgAddFriend.friendId;
            friendApplyPanel.friendId = msgAddFriend.id;
        }
    }

    //接受好友申请回调
    private void OnMsgAcceptAddFriend(MsgBase msg) {
        MsgAcceptAddFriend msgAcceptAddFriend = (MsgAcceptAddFriend)msg;
        if (msgAcceptAddFriend.friendId == GameMain.id) {
            PanelManager.Open<TipPanel>("添加" + msgAcceptAddFriend.id + "成功！");
        }
        if (PanelManager.panels.ContainsKey("FriendPanel")) {
            FriendPanel friendPanel = (FriendPanel)PanelManager.panels["FriendPanel"];
            friendPanel.UpdataFriendList();
        }
    }

    //收到删除好友协议
    private void OnMsgDeleteFriend(MsgBase msg) {
        MsgDeleteFriend msgDeleteFriend = (MsgDeleteFriend)msg;
        if (msgDeleteFriend.friendId == GameMain.id) {
            if (msgDeleteFriend.result == 0) {
                PanelManager.Open<TipPanel>("你已被" + msgDeleteFriend.id + "删除好友！");
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
    void OnMsgKick(MsgBase msgBase) {
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

