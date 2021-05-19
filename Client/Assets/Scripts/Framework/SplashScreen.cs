using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SplashScreen : MonoBehaviour
{

    VideoPlayer sc; 

    // Start is called before the first frame update
    void Start()
    {
        sc = transform.GetComponent<VideoPlayer>();
        Invoke("instanceGameMain", 1);
    }
    void instanceGameMain() {
        GameObject gameMain=Instantiate(ABManager.Instance.LoadRes<GameObject>("prefab/other", "GameMain"));
        gameMain.name = "GameMain";
    }

    // Update is called once per frame
    void Update()
    {
    }
}
