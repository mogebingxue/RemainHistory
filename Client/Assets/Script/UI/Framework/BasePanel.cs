using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{

    public class BasePanel : MonoBehaviour
    {
        /// <summary> 皮肤路径 </summary>
        public string skinPath;
        /// <summary> 皮肤 </summary>
        public GameObject skin;
        /// <summary> 层级 </summary>
        public PanelManager.Layer layer = PanelManager.Layer.Panel;


        /// <summary>
        /// 初始化 Panel:实例化Panel
        /// </summary>
        public void Init() {
            //皮肤
            GameObject skinPrefab = ABManager.Instance.LoadRes<GameObject>("prefab/ui", skinPath);
            skin = Instantiate(skinPrefab);
        }

        /// <summary>
        /// 隐藏 Panel
        /// </summary>
        protected void Hide() {
            string name = this.GetType().ToString();
            PanelManager.Hide(name);
        }

        /// <summary>
        /// Panel 初始化时:设置Panel路径和层级
        /// </summary>
        public virtual void OnInit() {
        }

        /// <summary>
        /// Panel 显示时:获取Panel的组件，如按钮、文本等,以及处理其他在Panel显示时的工作
        /// </summary>
        public virtual void OnShow(params object[] args) {
        }

        /// <summary>
        /// Panel 关闭时
        /// </summary>
        public virtual void OnClose() {
        }

        /// <summary>
        /// Panel 隐藏时
        /// </summary>
        public virtual void OnHide() {
        }


        /// <summary>
        /// Panel 添加监听游戏事件:如，添加监听按钮、滑动条等UI事件，添加网络监听事件
        /// </summary>
        public virtual void OnAddListener() {

        }

        /// <summary>
        /// Panel 移除监听游戏事件:如移除网络监听事件
        /// </summary>
        public virtual void OnRemoveListener() {

        }
    }
}