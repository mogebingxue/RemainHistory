using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;//包含MD5库
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UIFramework;

/// <summary>
/// 获取到更新脚本
/// </summary>
public class HotFix : MonoBehaviour
{
    private string platform {
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
    void Start() {
        //初始化
        PanelManager.Init();
        //DontDestroyObjects.Add(gameObject);
        DontDestroyOnLoad(GameObject.Find("UIRoot"));
        StartCoroutine(VersionUpdate());
    }

    private int allfilesLength = 0;
    /// <summary>
    /// 版本更新       
    /// </summary>
    /// <returns></returns>
    IEnumerator VersionUpdate() {
        UnityWebRequest uwr = UnityWebRequest.Get(Const.AssetServerURL + platform + @"\AssetVersion.txt");
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError) {
            Debug.Log("Download Error:" + uwr.error);
            yield break;
        }

        string versionInfo = uwr.downloadHandler.text;
        List<BundleInfo> bims = new List<BundleInfo>();
        FileMD5 date = JsonUtility.FromJson<FileMD5>(versionInfo);

        //删除所有不受版本控制的文件
        
        DeleteOtherBundles(date);
        var _list = date.versionInfo;
        string md5, file, path;
        int length;
        for (int i = 0; i < _list.Length; i++) {
            MD5Message _md5 = _list[i];
            file = _md5.file;
            path = Const.LocalAssetURL + file;
            //Debug.LogWarning("姚"+path);
            md5 = GetMD5HashFromFile(path);

            if (string.IsNullOrEmpty(md5) || md5 != _md5.md5) {
                
                bims.Add(new BundleInfo() {
                    Url = HttpDownLoadUrl(file),
                    Path = path
                });
                length = int.Parse(_md5.fileLength);
                allfilesLength += length;
            }
        }
        if (bims.Count > 0) {
            Debug.LogWarning("开始尝试更新");
            StartCoroutine(DownLoadBundleFiles(bims, (progress) => {
                PanelManager.Open<HotFixPanel>(progress, allfilesLength);
            }, (isfinish) => {
                if (isfinish)
                    StartCoroutine(VersionUpdateFinish());
                else {
                    StartCoroutine(VersionUpdate());
                }
            }));
        }
        else {
            StartCoroutine(VersionUpdateFinish());
        }

    }

    /// <summary>
    /// 删除所有不受版本控制的所有文件
    /// </summary>
    /// <param name="_md5"></param>
    void DeleteOtherBundles(FileMD5 _md5) {
        Debug.LogWarning("删除不受版本控制的文件");
        string[] bundleFiles = Directory.GetFiles(Const.LocalAssetURL, "*.*", SearchOption.AllDirectories);
        foreach (string idx in bundleFiles) {
            var _s = idx.Replace(Const.LocalAssetURL, "");
            if (!FindNameInFileMD5(_md5, _s)) {
                File.Delete(idx);
                //Debug.Log(_s + "不存在");
            }
        }
    }

    /// <summary>获取文件的md5校验码</summary>
    public string GetMD5HashFromFile(string fileName) {
        if (File.Exists(fileName)) {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));
            return sb.ToString();
        }
        return null;
    }
    /// <summary>
    /// 对比文件
    /// </summary>
    /// <param name="date"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    static bool FindNameInFileMD5(FileMD5 date, string _name) {
        foreach (var _m in date.versionInfo) {
            if (_m.file == _name) {
                return true;
            }
        }
        return false;
    }

    //脚本替换（lua等）验证
    //生成一遍MD5文件在和对比一下就完了
    IEnumerator VersionUpdateFinish() {
        yield return 0;
        Debug.Log("热更新完毕，进入游戏");
        GameMain.Instance.enabled = true;
        GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// 路径比对,文件下载
    /// </summary>  
    public IEnumerator DownLoadBundleFiles(List<BundleInfo> infos, Action<float> LoopCallBack = null, Action<bool> CallBack = null) {
        int num = 0;
        string dir;
        for (int i = 0; i < infos.Count; i++) {
            BundleInfo info = infos[i];
            //Debug.Log(info.Url);
            UnityWebRequest uwr = UnityWebRequest.Get(info.Url);
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError) {
                Debug.Log("Download Error:" + uwr.error);
                yield break;
            }
            try {
                string filepath = info.Path;
                Debug.Log(filepath);
                dir = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllBytes(filepath, uwr.downloadHandler.data);
                num++;
                if (LoopCallBack != null) {
                    LoopCallBack.Invoke((float)num / infos.Count);
                }
                Debug.Log(dir + "下载完成");
            }
            catch (Exception e) {
                Debug.LogWarning("姚" + info.Path);
                Debug.Log("下载失败" + e);
            }
        }
        if (CallBack != null) {
            CallBack.Invoke(num == infos.Count);
        }
    }

    /// <summary>
    /// 记录信息
    /// </summary>
    public struct BundleInfo
    {
        public string Path { get; set; }
        public string Url { get; set; }
        public override bool Equals(object obj) {
            return obj is BundleInfo && Url == ((BundleInfo)obj).Url;
        }
        public override int GetHashCode() {
            return Url.GetHashCode();
        }
    }




    string HttpDownLoadUrl(string _str) {
        return Const.AssetServerURL + platform + @"\" + _str;
    }
}
