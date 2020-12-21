using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Debug.Log("姚姚姚姚！");

        GameObject go = new GameObject();
        go.name = "雨雨雨雨雨";
       go.AddComponent<aaa>();
         go.GetComponent<aaa>().printa();
         Debug.Log(aaa.a);
         if (Input.GetKeyDown(KeyCode.A)) {
 
         }

    }

    // Update is called once per frame
    void Update() {

    }
}