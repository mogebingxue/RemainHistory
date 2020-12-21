using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaa : MonoBehaviour
{

    public static string a = "姚雨廷";
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("廷廷廷！");
    }

    public void printa() {
        Debug.Log(a+"调用方法");
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Settings.Paths.CompiledOutDir);
    }
}
