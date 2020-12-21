using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public class PanelManager : MonoSingleton<PanelManager>
    {
        /// <summary> 层级枚举 </summary>
        public enum Layer
        {
            Panel,
            Tip,
        }
        /// <summary> 层级列表 </summary>
        private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();
        /// <summary> 面板列表 </summary>
        public static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();
        //结构
        public static Transform root;
        public static Transform canvas;

        /// <summary>
        /// 初始化UI系统
        /// </summary>
        public static void Init() {
            if (GameObject.Find("UIRoot") == null) {
                GameObject UIRoot = Instantiate(ABManager.Instance.LoadRes<GameObject>("prefab/ui", "UIRoot"));
                UIRoot.name = "UIRoot";
            }
            root = GameObject.Find("UIRoot").transform;
            canvas = root.Find("Canvas");
            Transform panel = canvas.Find("Panel");
            Transform tip = canvas.Find("Tip");
            layers.Add(Layer.Panel, panel);
            layers.Add(Layer.Tip, tip);
        }

        /// <summary>
        /// 打开Panel
        /// </summary>
        /// <typeparam name="T">Panel的类型</typeparam>
        /// <param name="para">Panel 的名字</param>
        public static void Open<T>(params object[] para) where T : BasePanel {
            //已经存在
            string name = typeof(T).ToString();
            if (panels.ContainsKey(name)) {
                Transform panelType = canvas.Find(panels[name].layer.ToString());
                //已经显示
                if (panelType.Find(name + "(Clone)").gameObject.activeInHierarchy && panels[name].enabled) {
                    //组件
                    BasePanel thisPanel = root.gameObject.GetComponent<T>();
                    thisPanel.UpdataPara(para);
                    return;
                }
                //正在隐藏
                else {
                    panelType.Find(name + "(Clone)").gameObject.SetActive(true);
                    panels[name].enabled = true;

                    return;
                }

            }

            //组件
            BasePanel panel = root.gameObject.AddComponent<T>();
            panel.OnInit();
            panel.Init();
            //父容器
            Transform layer = layers[panel.layer];
            panel.skin.transform.SetParent(layer, false);
            //列表
            panels.Add(name, panel);
            //OnShow
            panel.OnShow(para);
            panel.OnAddListener();

        }

        /// <summary>
        /// 关闭Panel
        /// </summary>
        /// <typeparam name="T">Panel的类型</typeparam>
        /// <param name="para">Panel 的名字</param>
        public static void Close(string name) {
            //没有打开
            if (!panels.ContainsKey(name)) {
                return;
            }
            BasePanel panel = panels[name];

            //OnClose
            panel.OnClose();
            panel.OnRemoveListener();
            //列表
            panels.Remove(name);
            //销毁
            GameObject.Destroy(panel.skin);
            Component.Destroy(panel);
        }

        /// <summary>
        /// 隐藏 Panel
        /// </summary>
        /// <param name="name"></param>
        public static void Hide(string name) {
            //没有打开
            if (!panels.ContainsKey(name)) {
                return;
            }
            BasePanel panel = panels[name];
            //OnHide
            panel.OnHide();
            //隐藏物体
            canvas.Find(panels[name].layer.ToString()).Find(name + "(Clone)").gameObject.SetActive(false);
            panels[name].enabled = false;
        }

        /// <summary>
        /// Panel 是否已经显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static bool IsOpen<T>() where T : BasePanel {
            //已经存在
            string name = typeof(T).ToString();
            if (panels.ContainsKey(name)) {
                //已经显示
                if (canvas.Find(panels[name].layer.ToString()).Find(name + "(Clone)").gameObject.activeInHierarchy && panels[name].enabled) {
                    return true;
                }
                //正在隐藏
                else {
                    return false;
                }
            }
            else {
                return false;
            }

        }
    }



}
