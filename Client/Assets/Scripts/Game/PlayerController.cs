using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Vector3 beginSelect;
    Vector3 endSelect;
    Vector3 targetPos;
    List<GameObject> selectList;


    // Start is called before the first frame update
    void Start() {
        selectList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        selectPawn();
        if (Input.GetMouseButtonDown(1)) {
            if (selectList.Count != 0) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider.tag == "land") {
                    foreach (var pawn in selectList) {
                        pawn.GetComponent<PawnBase>().MoveTo(hit.point);
                    }
                }

            }
        }
    }



    public void selectPawn() {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            beginSelect = new Vector3(hit.point.x, 0, hit.point.z);
        }
        if ((Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift)) || (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftShift))) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            endSelect = new Vector3(hit.point.x, 10, hit.point.z);
            AddPawn();
            return;
        }

        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            beginSelect = new Vector3(hit.point.x, 0, hit.point.z);
        }
        if ((Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl)) || (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl))) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            endSelect = new Vector3(hit.point.x, 10, hit.point.z);
            RemovePawn();
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            selectList.Clear();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            beginSelect = new Vector3(hit.point.x, 0, hit.point.z);
        }
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            endSelect = new Vector3(hit.point.x, 10, hit.point.z);
            AddPawn();
        }
    }

    private void RemovePawn() {
        Vector3 center = new Vector3((beginSelect.x + endSelect.x) * 0.5f, (beginSelect.y + endSelect.y) * 0.5f, (beginSelect.z + endSelect.z) * 0.5f);
        Vector3 size = new Vector3(Mathf.Abs(beginSelect.x - endSelect.x) * 0.5f, Mathf.Abs(beginSelect.y - endSelect.y) * 0.5f, Mathf.Abs(beginSelect.z - endSelect.z) * 0.5f);
        Collider[] colliders = Physics.OverlapBox(center, size);

        foreach (Collider collider in colliders) {

            if (collider.tag == "pawn" && selectList.Contains(collider.gameObject)) {
                collider.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                selectList.Remove(collider.gameObject);
            }
        }
    }

    private void AddPawn() {
        Vector3 center = new Vector3((beginSelect.x + endSelect.x) * 0.5f, (beginSelect.y + endSelect.y) * 0.5f, (beginSelect.z + endSelect.z) * 0.5f);
        Vector3 size = new Vector3(Mathf.Abs(beginSelect.x - endSelect.x) * 0.5f, Mathf.Abs(beginSelect.y - endSelect.y) * 0.5f, Mathf.Abs(beginSelect.z - endSelect.z) * 0.5f);
        Collider[] colliders = Physics.OverlapBox(center, size);

        foreach (Collider collider in colliders) {

            if (collider.tag == "pawn" && !selectList.Contains(collider.gameObject)) {
                collider.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                selectList.Add(collider.gameObject);
            }
        }
    }

    //绘制出来
    public void OnDrawGizmos() {

        Gizmos.color = Color.green;
        Vector3 center = new Vector3((beginSelect.x + endSelect.x) * 0.5f, (beginSelect.y + endSelect.y) * 0.5f, (beginSelect.z + endSelect.z) * 0.5f);
        Vector3 size = new Vector3(Mathf.Abs(beginSelect.x - endSelect.x), Mathf.Abs(beginSelect.y - endSelect.y), Mathf.Abs(beginSelect.z - endSelect.z));
        Gizmos.DrawWireCube(center, size);
    }
}
