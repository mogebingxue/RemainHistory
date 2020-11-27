using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //移动边框大小
    public float GUISize = 25f;
    //相机移动速度
    public float cameraSpeed = 1f;
    //相机缩放速度
    public float zoomSpeed = 1f;
    public float maxZoom = 100f;
    public float minZoom = 15f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdata();
        ZoomUpdate();

    }

    void ZoomUpdate() {
        
        if(Camera.main.fieldOfView>maxZoom) {
            Camera.main.fieldOfView = maxZoom;
            return;
        }
        if (Camera.main.fieldOfView <minZoom) {
            Camera.main.fieldOfView = minZoom;
            return;
        }
        float axis = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.fieldOfView -= axis * zoomSpeed; 

    }

    void MoveUpdata() {
        Rect rectDown = new Rect(0, 0, Screen.width, GUISize);
        Rect rectUp = new Rect(0, Screen.height - GUISize, Screen.width, GUISize);
        Rect rectLeft = new Rect(0, 0, GUISize, Screen.height);
        Rect rectRight = new Rect(Screen.width - GUISize, 0, GUISize, Screen.height);

        if (rectDown.Contains(Input.mousePosition)) {
            transform.
            transform.Translate(0, 0 , cameraSpeed, Space.World);
        }
        if (rectUp.Contains(Input.mousePosition)) {
            transform.Translate(0, 0, -cameraSpeed, Space.World);
        }
        if (rectLeft.Contains(Input.mousePosition)) {
            transform.Translate(cameraSpeed, 0, 0, Space.World);
        }
        if (rectRight.Contains(Input.mousePosition)) {
            transform.Translate(-cameraSpeed, 0, 0, Space.World);
        }
        
    }
}
