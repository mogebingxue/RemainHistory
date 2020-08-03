using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandbookPanel : BasePanel
{
    //关闭按钮
    private Button closeBtn;
    //左移
    private Button leftBtn;
    //右移
    private Button rightBtn;


    //初始化
    public override void OnInit() {
        skinPath = "HandbookPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        leftBtn = skin.transform.Find("LeftBtn").GetComponent<Button>();
        rightBtn = skin.transform.Find("RightBtn").GetComponent<Button>();
        //监听
        closeBtn.onClick.AddListener(OnCloseClick);
        leftBtn.onClick.AddListener(OnLeftClick);
        rightBtn.onClick.AddListener(OnRightClick);

    }

    //左移
    private void OnRightClick() {
        throw new NotImplementedException();
    }
    //右移
    private void OnLeftClick() {
        throw new NotImplementedException();
    }

    //当按下关闭按钮
    public void OnCloseClick() {
        Close();
    }

    //关闭
    public override void OnClose() {
    }
}
