using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : BasePanel
{
    //关闭按钮
    private Button _closeBtn;
    //音量按钮
    private Button _volumeBtn;
    //退出游戏
    private Button _quitBtn;
    //音量滑动条
    private Slider _volumeSlider;
    //初始化
    public override void OnInit() {
        skinPath = "SetPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        _closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        _quitBtn = skin.transform.Find("QuitBtn").GetComponent<Button>();
        _volumeBtn = skin.transform.Find("VolumePanel/VolumeBtn").GetComponent<Button>();
        _volumeSlider = skin.transform.Find("VolumePanel/VolumeSlider").GetComponent<Slider>();
        //监听
        _volumeBtn.onClick.AddListener(OnVolumeClick);
        _volumeSlider.onValueChanged.AddListener(OnVolumeValueChanged);
        _closeBtn.onClick.AddListener(OnCloseClick);
        _quitBtn.onClick.AddListener(OnQiutClick);
        //初始化音量控制条及小喇叭颜色
        string isOnVolume = PlayerPrefs.GetString(Const.IsOnVolume);
        _volumeSlider.value = PlayerPrefs.GetFloat(Const.Volume);
        PlayerPrefs.SetString(Const.IsOnVolume, isOnVolume);
        PlayerPrefs.Save();
        if (PlayerPrefs.GetString(Const.IsOnVolume) == "true") {
            AudioListener.volume = PlayerPrefs.GetFloat(Const.Volume);
            _volumeBtn.GetComponent<Image>().color = Color.green;
        }
        if (PlayerPrefs.GetString(Const.IsOnVolume) == "false") {
            AudioListener.volume = 0f;
            _volumeBtn.GetComponent<Image>().color = Color.gray;
        }
    }



    //声音滑动条滑动
    private void OnVolumeValueChanged(float value) {
        PlayerPrefs.SetFloat(Const.Volume, value);
        AudioListener.volume = value;
        PlayerPrefs.SetString(Const.IsOnVolume, "true");
        PlayerPrefs.Save();
        _volumeBtn.GetComponent<Image>().color = Color.green;
    }
    //声音按钮点击
    private void OnVolumeClick() {
        if (PlayerPrefs.GetString(Const.IsOnVolume) == "true") {
            PlayerPrefs.SetString(Const.IsOnVolume, "false");
            PlayerPrefs.Save();
            AudioListener.volume = 0;
            _volumeBtn.GetComponent<Image>().color = Color.gray;
            return;
        }
        if (PlayerPrefs.GetString(Const.IsOnVolume) == "false") {
            PlayerPrefs.SetString(Const.IsOnVolume, "true");
            PlayerPrefs.Save();
            AudioListener.volume = _volumeSlider.value;
            _volumeBtn.GetComponent<Image>().color = Color.green;
            return;
        }
    }
    //当按下关闭按钮
    public void OnCloseClick() {
        Hide();
    }

    //退出游戏
    private void OnQiutClick() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    //关闭
    public override void OnClose() {
    }
}
