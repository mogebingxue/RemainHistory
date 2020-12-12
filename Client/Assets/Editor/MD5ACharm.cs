using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;//包含MD5库
using UnityEditor;

/// <summary>
/// MD5 效验器
/// 遍历该文件夹下所有子文件，并生成相对应的MD5
/// </summary>
public class MD5ACharm : MonoBehaviour
{

    [MenuItem("MD5效验器/平台/IOS平台")]
    static void BuildReleaseIOSBundle() {
        BuildBundleStart("iOS");
    }
    [MenuItem("MD5效验器/平台/Android平台")]
    static void BuildReleaseAndroidBundle() {
        BuildBundleStart("Android");
    }

    [MenuItem("MD5效验器/平台/PC平台")]
    static void BuildReleaseWindowsBundle() {
        BuildBundleStart("PC");
    }

    /// <summary>
    /// 指定下载路径,默认当前环境
    /// </summary>
    static string ABPath = "PC";


    private static Dictionary<string, string> m_BundleMD5Map = new Dictionary<string, string>();
    static void BuildBundleStart(string _path) {
        ABPath = _path;
        Caching.ClearCache();//清除所有缓存
        string path = GetTempPath();
        DeleteTempBundles(path);   //删除旧的MD5版本文件
        CreateBundleVersionNumber(path);
        AssetDatabase.Refresh();
    }



    /// <summary>
    /// 删除指定文件
    /// </summary>
    /// <param name="target"></param>
    static void DeleteTempBundles(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        string[] bundleFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (string s in bundleFiles) {
            if (s == path+"AssetVersion.txt") {
                Debug.LogWarning("删除旧版资源版本文件");
                File.Delete(s);
            }
        }
    }

    static void CreateBundleVersionNumber(string bundlePath) {
        FileMD5 _file = new FileMD5();
        string[] contents = Directory.GetFiles(bundlePath, "*.*", SearchOption.AllDirectories);
        string extension;
        string fileName = "";
        string fileMD5 = "";
        long allLength = 0;
        int fileLen;
        m_BundleMD5Map.Clear();
        for (int i = 0; i < contents.Length; i++) {
            fileName = contents[i].Replace(GetTempPath(), "").Replace("\\", @"\");
            extension = Path.GetExtension(contents[i]);
            if (extension != ".meta") {
                fileMD5 = GetMD5HashFromFile(contents[i]);
                fileLen = File.ReadAllBytes(contents[i]).Length;
                allLength += fileLen;
                m_BundleMD5Map.Add(fileName, fileMD5 + "+" + fileLen);
            }
        }

        var _list = new List<MD5Message>();
        foreach (KeyValuePair<string, string> kv in m_BundleMD5Map) {
            string[] nAndL = kv.Value.Split('+');
            MD5Message _md5 = new MD5Message();
            _md5.file = kv.Key;
            _md5.md5 = nAndL[0];
            _md5.fileLength = nAndL[1];
            _list.Add(_md5);
        }
        var _md5All = new MD5Message[_list.Count];
        for (var _i = 0; _i < _list.Count; _i++) {
            _md5All[_i] = _list[_i];
        }

        _file.length = "" + allLength;

        _file.versionInfo = _md5All;

        var _filePath = JsonUtility.ToJson(_file);

        Debug.Log(_filePath);

        File.WriteAllText(GetTempPath() + "AssetVersion.txt", _filePath);
        m_BundleMD5Map.Clear();

    }

    /// <summary>获取文件的md5校验码</summary>
    static string GetMD5HashFromFile(string fileName) {
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


    static string GetTempPath(string _path = "") {
        var _str = Const.AssetServerURL + ABPath + @"\" + _path;
        return _str;
    }
}