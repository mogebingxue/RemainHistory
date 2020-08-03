using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMain : MonoBehaviour
{
    public static string id = "";


    //加载场景时不销毁的物体
    public GameObject[] DontDestroyObjects;

    //是否已经存在DontDestroy的物体
    private static bool isExist;

    void Awake() {
        if (!isExist) {
            for (int i = 0; i < DontDestroyObjects.Length; i++) {
                //如果第一次加载，将这些物体设为DontDestroy
                DontDestroyOnLoad(DontDestroyObjects[i]);
            }

            isExist = true;
        }
        else {
            for (int i = 0; i < DontDestroyObjects.Length; i++) {
                //如果已经存在，则删除重复的物体
                Destroy(DontDestroyObjects[i]);
            }
        }

    }
    // Use this for initialization
    void Start() {
        //网络监听
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);

        //打开登陆面板
        SceneManager.LoadSceneAsync("LoginMenu");
        //初始化
        PanelManager.Init();
        //打开登陆面板
        PanelManager.Open<LoginPanel>();
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

