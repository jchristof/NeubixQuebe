using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MoveScript : MonoBehaviour, ChildDragWatcher 
{
    public GameObject roundCornerCube;
    public List<Material> challengeRowColors;
    private List<GameObject> cubes = new List<GameObject>();
    
    void Start() {
        for (var i = 17; i >= 0; i--) {
            GameObject cube = Instantiate(roundCornerCube);
            cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
            cube.GetComponent<Renderer>().material = challengeRowColors[i / 3]; //GetMaterial();
            cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            cube.transform.localScale = Vector3.one * .9f;

            cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
            cube.GetComponent<TileScript>().SetParentComponent(this);
           // cube.transform.parent = transform;
            cubes.Add(cube);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private Vector3 mouseDownPosition;
    private Vector3 dragAxis;
    private Vector3 lockedPosition;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 initialWorldPosition;
    private List<GameObject> movingCubes;
    private GameObject dragChild;
    private int itemIndex;
    private bool inDrag;
    private Vector3 savedTransformPosition;
    
    void OnMouseDown() {
        savedTransformPosition = transform.position;
        mouseDownPosition = Input.mousePosition;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        initialWorldPosition = transform.position;
        offset = transform.position -
                 Camera.main.ScreenToWorldPoint(
                     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        dragAxis = Vector3.zero;
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        
        foreach(var hit in hits)
        {
            if (hit.transform.GetComponent<TileScript>()) {
                dragChild = hit.transform.gameObject;
            }
        }

        if (dragChild == null)
            return;

        inDrag = true;
      
    }

    private void OnMouseDrag() {
        if (!inDrag)
            return;
        
        var diff = Input.mousePosition - mouseDownPosition;
        if (dragAxis == Vector3.zero) {
            if (diff.magnitude > 5f) {
                if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                    //drag is on the x axis
                    dragAxis = Vector3.right;
                    lockedPosition = new Vector3(0, mouseDownPosition.y, 0);

                    var rowInfo = GetRowOf(dragChild);
                    foreach (var cube in rowInfo.Item1) {
                        cube.transform.parent = transform;
                    }

                    movingCubes = rowInfo.Item1;
                }
                else {
                    dragAxis = new Vector3(0, 1f, 0);
                    lockedPosition = new Vector3(mouseDownPosition.x, 0, 0);

                    var columnInfo = GetColumnOf(dragChild);
                    foreach (var cube in columnInfo.Item1) {
                        cube.transform.parent = transform;
                    }
                    
                    movingCubes = columnInfo.Item1;
                }
            }

            return;
        }
        
        var curScreenPoint = new Vector3(Input.mousePosition.x * dragAxis.x + lockedPosition.x,
            Input.mousePosition.y * dragAxis.y + lockedPosition.y,
            screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        
        if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).x) >= 1.0 || Mathf.Abs((initialWorldPosition - (curPosition + offset)).y)  >= 1.0) {
            inDrag = false;

            foreach (var cube in movingCubes) {
                var position = cube.transform.position;
                cube.transform.parent = null;
                cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
            }
            
            OnMouseUp();
     
            return;
        }
        
        transform.position = curPosition + offset;
    }
    
    Tuple<List<GameObject>, int> GetRowOf(GameObject gameObject) {
        List<GameObject> newList = new List<GameObject>();
        
        var indexOfSelected = cubes.IndexOf(gameObject);
        var rowIndex = indexOfSelected / 3;
        var column = indexOfSelected % 3;
        var startIndex = rowIndex * 3;

        for (int i = startIndex; i < startIndex + 3; i++) {
            newList.Add(cubes[i]);
        }

        return new Tuple<List<GameObject>, int>(newList, column);
    }
    
    Tuple<List<GameObject>, int> GetColumnOf(GameObject gameObject) {
        List<GameObject> newList = new List<GameObject>();
        
        var indexOfSelected = cubes.IndexOf(gameObject);
        var column = indexOfSelected % 3;
        var row = indexOfSelected / 3;
        var startIndex = column;

        for (int i = startIndex; i < 18 ; i += 3) {
            newList.Add(cubes[i]);
        }
        
        newList.Reverse();

        return new Tuple<List<GameObject>, int>(newList, row);
    }

    void OnMouseUp() {
        transform.position = savedTransformPosition;
        foreach (var cube in cubes) {
            cube.transform.parent = null;
        }

        inDrag = false;    
    }

    public void ChildMouseDown(GameObject gameObject) {
        
    }

    public void ChildMouseUp() {
        
    }

    public void ChildMouseDrag() {
        
    }
}
