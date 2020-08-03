using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltarPanel : BasePanel
{
    //关闭按钮
    private Button closeBtn;
 


    //初始化
    public override void OnInit() {
        skinPath = "AltarPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        //监听
        closeBtn.onClick.AddListener(OnCloseClick);

    }

    //当按下关闭按钮
    public void OnCloseClick() {
        Close();
    }

    //关闭
    public override void OnClose() {
    }
}
