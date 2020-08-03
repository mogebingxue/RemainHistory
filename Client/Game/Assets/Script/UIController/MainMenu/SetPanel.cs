using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : BasePanel
{
    //关闭按钮
    private Button closeBtn;
    private Button volumeBtn;
    private Slider volumeSlider;

    //初始化
    public override void OnInit() {
        skinPath = "SetPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        //获得组件
        volumeBtn = skin.transform.Find("VolumePanel").Find("VolumeBtn").GetComponent<Button>();
        volumeSlider = skin.transform.Find("VolumePanel").Find("VolumeSlider").GetComponent<Slider>();
        //监听
        volumeBtn.onClick.AddListener(OnVolumeClick);
        volumeSlider.onValueChanged.AddListener(OnVolumeValueChanged);
        closeBtn.onClick.AddListener(OnCloseClick);
        //设置音量
        volumeSlider.value = PlayerPrefs.GetFloat(Const.Volume);
        AudioListener.volume = volumeSlider.value;

    }
    //声音滑动条滑动
    private void OnVolumeValueChanged(float value) {
        PlayerPrefs.SetFloat(Const.Volume, value);
        AudioListener.volume = value;
    }
    //声音按钮点击
    private void OnVolumeClick() {
        volumeSlider.value = 0;

    }
    //当按下关闭按钮
    public void OnCloseClick() {
        Close();
    }

    //关闭
    public override void OnClose() {
    }
}
