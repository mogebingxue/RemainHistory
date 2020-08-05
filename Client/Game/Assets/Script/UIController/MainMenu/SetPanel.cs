using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : BasePanel
{
    //关闭按钮
    private Button _closeBtn;
    //音量按钮
    private Button _volumeBtn;
    //音量滑动条
    private Slider _volumeSlider;
    //是否开启音乐
    private bool _isOnVolume;
    //初始化
    public override void OnInit() {
        skinPath = "SetPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        _closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        //获得组件
        _volumeBtn = skin.transform.Find("VolumePanel").Find("VolumeBtn").GetComponent<Button>();
        _volumeSlider = skin.transform.Find("VolumePanel").Find("VolumeSlider").GetComponent<Slider>();
        //监听
        _volumeBtn.onClick.AddListener(OnVolumeClick);
        _volumeSlider.onValueChanged.AddListener(OnVolumeValueChanged);
        _closeBtn.onClick.AddListener(OnCloseClick);
        //设置音量
        _volumeSlider.value = PlayerPrefs.GetFloat(Const.Volume);
        AudioListener.volume = _volumeSlider.value;
        _isOnVolume = true;

    }
    //声音滑动条滑动
    private void OnVolumeValueChanged(float value) {
        PlayerPrefs.SetFloat(Const.Volume, value);
        AudioListener.volume = value;
    }
    //声音按钮点击
    private void OnVolumeClick() {
        if (_isOnVolume) {
            _isOnVolume = false;
            AudioListener.volume = 0;
            _volumeBtn.GetComponent<Image>().color = Color.gray;
            return;
        }
        if (!_isOnVolume) {
            _isOnVolume = true;
            AudioListener.volume = _volumeSlider.value;
            _volumeBtn.GetComponent<Image>().color = Color.green;
            return;
        }
    }
    //当按下关闭按钮
    public void OnCloseClick() {
        Close();
    }

    //关闭
    public override void OnClose() {
    }
}
