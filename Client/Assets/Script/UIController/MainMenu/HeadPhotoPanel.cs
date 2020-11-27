using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadPhotoPanel : BasePanel
{
    //关闭按钮
    private Button _closeBtn;
    //左移
    private Button _leftBtn;
    //右移
    private Button _rightBtn;
    //头像
    private Image _headPhoto;
    //头像索引
    private int _headPhotoIndex;
    //初始化
    public override void OnInit() {
        skinPath = "HeadPhotoPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        _closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        _leftBtn = skin.transform.Find("LeftBtn").GetComponent<Button>();
        _rightBtn = skin.transform.Find("RightBtn").GetComponent<Button>();
        _headPhoto = skin.transform.Find("HeadPhoto").GetComponent<Image>();
        //监听
        _closeBtn.onClick.AddListener(OnCloseClick);
        _leftBtn.onClick.AddListener(OnLeftClick);
        _rightBtn.onClick.AddListener(OnRightClick);
        //网络监听
        NetManager.AddMsgListener("MsgGetHeadPhoto", OnMsgGetHeadPhoto);
        NetManager.AddMsgListener("MsgSaveHeadPhoto", OnMsgSaveHeadPhoto);

        _headPhoto.sprite = ResManager.LoadTexture("Texture/HeadPhoto" + _headPhotoIndex);
    }

    //右移
    private void OnRightClick() {
        string[] headPhoto = _headPhoto.sprite.name.Split('o');
        if (int.Parse(headPhoto[2]) < 10) {
            int i = int.Parse(headPhoto[2]) + 1;
            _headPhoto.sprite = ResManager.LoadTexture("Texture/HeadPhoto" + i );
            Debug.Log(_headPhoto.sprite.name);
        }
    }
    //左移
    private void OnLeftClick() {
        string[] headPhoto = _headPhoto.sprite.name.Split('o');
        if (int.Parse(headPhoto[2]) >0) {
            int i = int.Parse(headPhoto[2]) - 1;
            _headPhoto.sprite = ResManager.LoadTexture("Texture/HeadPhoto" + i);
            Debug.Log(_headPhoto.sprite.name);
        }
    }

    //当按下关闭按钮
    public void OnCloseClick() {
        Close();
    }

    //获取头像回调
    private void OnMsgGetHeadPhoto(MsgBase msg) {
        MsgGetHeadPhoto msgGetHeadPhoto = (MsgGetHeadPhoto)msg;
        _headPhotoIndex = msgGetHeadPhoto.headPhoto;
        if (_headPhoto != null) {
            _headPhoto.sprite = ResManager.LoadTexture("Texture/HeadPhoto" + _headPhotoIndex);
        }
    }

    //保存头像回调
    private void OnMsgSaveHeadPhoto(MsgBase msg) {
        MsgGetHeadPhoto msgGetHeadPhoto = new MsgGetHeadPhoto();
        NetManager.Send(msgGetHeadPhoto);
        Debug.Log("保存头像成功！");
    }

    //关闭
    public override void OnClose() {
        MsgSaveHeadPhoto msgSaveHeadPhoto = new MsgSaveHeadPhoto();
        string[] headPhoto = _headPhoto.sprite.name.Split('o');
        msgSaveHeadPhoto.headPhoto = int.Parse(headPhoto[2]);
        NetManager.Send(msgSaveHeadPhoto);
    }
}
