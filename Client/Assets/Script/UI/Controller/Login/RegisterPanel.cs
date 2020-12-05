using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel {
	//账号输入框
	private InputField _idInput;
	//密码输入框
	private InputField _pwInput;
	//重复输入框
	private InputField _repInput;
	//注册按钮
	private Button _regBtn;
	//关闭按钮
	private Button _closeBtn;


	//初始化
	public override void OnInit() {
		skinPath = "RegisterPanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] para) {
		//寻找组件
		_idInput = skin.transform.Find("IdInput").GetComponent<InputField>();
		_pwInput = skin.transform.Find("PwInput").GetComponent<InputField>();
		_repInput = skin.transform.Find("RepInput").GetComponent<InputField>();
		_regBtn = skin.transform.Find("RegisterBtn").GetComponent<Button>();
		_closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
		//监听
		_regBtn.onClick.AddListener(OnRegClick);
		_closeBtn.onClick.AddListener(OnCloseClick);
		//网络协议监听
		NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
	}

	//关闭
	public override void OnClose() {
		//网络协议监听
		NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
	}

	//当按下注册按钮
	public void OnRegClick() {
		//用户名密码为空
		if (_idInput.text == "" || _pwInput.text == "") {
			PanelManager.Open<TipPanel>("用户名和密码不能为空");
			return;
		}
		//两次密码不同
		if (_repInput.text != _pwInput.text) {
			PanelManager.Open<TipPanel>("两次输入的密码不同");
			return;
		}
		//发送
		MsgRegister msgReg = new MsgRegister();
		msgReg.id = _idInput.text;
		msgReg.pw = _pwInput.text;
		NetManager.Send(msgReg);
	}

	//收到注册协议
	public void OnMsgRegister (MsgBase msgBase) {
		MsgRegister msg = (MsgRegister)msgBase;
		if(msg.result == 0){
			Debug.Log("注册成功");
			//提示
			PanelManager.Open<TipPanel>("注册成功");
			//关闭界面
			Hide();
		}
		else{
			PanelManager.Open<TipPanel>("注册失败");
		}
	}

	//当按下关闭按钮
	public void OnCloseClick() {
		Hide();
	}
}
