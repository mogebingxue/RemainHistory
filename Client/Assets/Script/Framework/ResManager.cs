using UnityEngine;

public class ResManager : MonoBehaviour {

    /// <summary>
    /// 加载预制体
    /// </summary>
    /// <param name="path">预制体的路径</param>
    /// <returns>预制体</returns>
	public static GameObject LoadPrefab(string path){
		return Resources.Load<GameObject>(path);
	}


    /// <summary>
    /// 加载图片
    /// </summary>
    /// <param name="path">图片的路径</param>
    /// <returns>图片</returns>
    public static Sprite LoadTexture(string path) {
        return Resources.Load<Sprite>(path);
    }
}
