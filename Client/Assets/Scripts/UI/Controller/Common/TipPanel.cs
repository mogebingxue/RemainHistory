using UIFramework;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    //提示文本
    private Text _text;
    //确定按钮
    private Button _okBtn;


    //初始化
    public override void OnInit() {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }
    //显示
    public override void OnShow(params object[] args) {
        //寻找组件
        _text = skin.transform.Find("Text").GetComponent<Text>();
        _okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
        //监听
        _okBtn.onClick.AddListener(OnOkClick);
        //提示语
        if (args.Length == 1) {
            _text.text = (string)args[0];
        }
    }

    //关闭
    public override void OnClose() {

    }

    //当按下确定按钮
    public void OnOkClick() {
        Hide();
    }
}
