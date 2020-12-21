using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ABManager : MonoSingleton<ABManager>
{

    private AssetBundle _mainAB = null;
    private AssetBundleManifest _manifest = null;
    private Dictionary<string, AssetBundle> _abList = new Dictionary<string, AssetBundle>();

    private string PathUrl {
        get {
            return Application.streamingAssetsPath + "/";
        }
    }
    private string MainABName {
        get {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else 
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="abName">ab包的名称</param>
    /// <param name="resName">资源名称</param>
    public T LoadRes<T>(string abName, string resName) where T : Object {
        //加载AB包

        if (!_mainAB) {
            _mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            _manifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        }
        //获取依赖包相关信息
        string[] strs = _manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++) {
            //判断包是否加载过
            if (!_abList.ContainsKey(strs[i])) {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                _abList.Add(strs[i], ab);
            }
        }
        //加载资源来源包
        if (!_abList.ContainsKey(abName)) {
            AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
            _abList.Add(abName, ab);
        }
        //加载资源
        return (T)_abList[abName].LoadAsset(resName);
    }



    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object {

        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack));
    }

    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object {
        //加载AB包
        if (_mainAB == null) {
            _mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            _manifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        //获取依赖包相关信息
        string[] strs = _manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++) {
            //判断包是否加载过
            if (!_abList.ContainsKey(strs[i])) {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                _abList.Add(strs[i], ab);
            }
        }
        //加载资源来源包
        if (!_abList.ContainsKey(abName)) {
            AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
            _abList.Add(abName, ab);
        }
        //加载资源
        AssetBundleRequest abr = _abList[abName].LoadAssetAsync(resName);
        yield return abr;
        callBack((T)abr.asset);
    }

    /// <summary>
    /// 卸载单个包
    /// </summary>
    /// <param name="abName"></param>
    public void Unload(string abName) {
        if (_abList.ContainsKey(abName)) {
            _abList[abName].Unload(false);
            _abList.Remove(abName);
        }

    }

    /// <summary>
    /// 卸载所有包
    /// </summary>
    public void ClearAB() {
        AssetBundle.UnloadAllAssetBundles(false);
        _abList.Clear();
        _mainAB = null;
        _manifest = null;
    }
}
