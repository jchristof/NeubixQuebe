using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ChallengeMenu : MonoBehaviour, ChildDragWatcher {
    public GameObject roundCornerCube;
    public List<Material> challengeRowColors;
    private List<GameObject> cubes = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {
        for (var i = 17; i >= 0; i--) {
            GameObject cube = Instantiate(roundCornerCube);
            cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
            cube.GetComponent<Renderer>().material = challengeRowColors[i / 3]; //GetMaterial();
            cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            cube.transform.localScale = Vector3.one * .9f;

            cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
            cube.GetComponent<TileScript>().SetParentComponent(this);

            cubes.Add(cube);
        }
    }

    private Vector3 mouseDownPosition;
    private Vector3 dragDirection;
    private Vector3 lockedPosition;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 initialWorldPosition;
    private List<GameObject> movingCubes;
    private GameObject dragChild;
    private int itemIndex;
    private bool inDrag;
    
    public void ChildMouseDown(GameObject child) {
        mouseDownPosition = Input.mousePosition;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        initialWorldPosition = child.transform.position;
        offset = child.transform.position -
                 Camera.main.ScreenToWorldPoint(
                     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        dragDirection = Vector3.zero;
        dragChild = child;
        inDrag = true;
    }

    public void ChildMouseDrag() {
        if (!inDrag)
            return;
        
        var diff = Input.mousePosition - mouseDownPosition;
        if (dragDirection == Vector3.zero) {
            if (diff.magnitude > .5f) {
                if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                    //drag is on the x axis
                    dragDirection = new Vector3(1f, 0, 0);
                    lockedPosition = new Vector3(0, Input.mousePosition.y, 0);

                    var rowInfo = GetRowOf(dragChild);
                    movingCubes = rowInfo.Item1;
                    itemIndex = - rowInfo.Item2;
                }
                else {
                    dragDirection = new Vector3(0, 1f, 0);
                    lockedPosition = new Vector3(Input.mousePosition.x, 0, 0);

                    var columnInfo = GetColumnOf(dragChild);
                    movingCubes = columnInfo.Item1;
                    itemIndex = -5 + columnInfo.Item2;
                }
            }

            return;
        }
        
        var curScreenPoint = new Vector3(Input.mousePosition.x * dragDirection.x + lockedPosition.x,
            Input.mousePosition.y * dragDirection.y + lockedPosition.y,
            screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);

        if ((initialWorldPosition - curPosition).magnitude >= 1.0) {
            inDrag = false;

            foreach (var cube in movingCubes) {
                var position = cube.transform.position;
                cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
            }

            return;
        }
        
        int offset2 = itemIndex;
        foreach (var cube in movingCubes) {
            cube.transform.position = curPosition + offset + offset2 * dragDirection;
            offset2 += 1;
        }
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
    public void ChildMouseUp() {
        
    }
}