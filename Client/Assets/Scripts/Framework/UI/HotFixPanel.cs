using UIFramework;
using UnityEngine;
using UnityEngine.UI;

public class HotFixPanel : BasePanel
{
    //关闭按钮
    private Scrollbar _updataProgressBar;



    //初始化
    public override void OnInit() {
        skinPath = "HotFixPanel";
        layer = PanelManager.Layer.Panel;
        isHotFix = false;
    }

    //显示
    public override void OnShow(params object[] para) {
        //寻找组件
        _updataProgressBar = skin.transform.Find("UpdataProgressBar").GetComponent<Scrollbar>();

    }

    public override void UpdataPara(params object[] para) {
        _updataProgressBar.size = (float)para[0];
        Debug.Log((float)para[0]);
        if ((float)para[0] == 1) {
            Close();
        }
    }
    //关闭
    public override void OnClose() {
    }
}
