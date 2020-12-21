using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object syncRoot = new UnityEngine.Object();
    public static T Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) {
                        GameObject go = new GameObject();
                        go.name = typeof(T).Name;
                        instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go);
                    }
                }
            }
            return instance;
        }
    }
    protected MonoSingleton() {
    }
}