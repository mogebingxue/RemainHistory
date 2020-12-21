public class Singleton<T> where T : new()
{
    // 定义一个静态变量来保存类的实例
    private static T instance;

    // 定义一个标识确保线程同步
    private static readonly object locker = new object();
    public static T Instance {
        get {
            if (instance == null) {
                lock (locker) {
                    if (instance == null) {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }
    protected Singleton() {
    }
}