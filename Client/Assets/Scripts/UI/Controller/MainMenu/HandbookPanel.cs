using System;
using UIFramework;
using UnityEngine.UI;

public class HandbookPanel : BasePanel
{
    //关闭按钮
    private Button _closeBtn;
    //左移
    private Button _leftBtn;
    //右移
    private Button _rightBtn;


    //初始化
    public override void OnInit() {
        skinPath = "HandbookPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        _closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        _leftBtn = skin.transform.Find("LeftBtn").GetComponent<Button>();
        _rightBtn = skin.transform.Find("RightBtn").GetComponent<Button>();
        //监听
        _closeBtn.onClick.AddListener(OnCloseClick);
        _leftBtn.onClick.AddListener(OnLeftClick);
        _rightBtn.onClick.AddListener(OnRightClick);

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
        Hide();
    }

    //关闭
    public override void OnClose() {
    }
}
