using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveScript : MonoBehaviour, ChildDragWatcher {
    public GameObject roundCornerCube;
    public List<Material> challengeRowColors;
    private List<GameObject> cubes = new List<GameObject>();
    public GameObject pointer;

    void Start() {
        for (var i = 17; i >= 0; i--) {
            GameObject cube = Instantiate(roundCornerCube);
            cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
            cube.GetComponent<Renderer>().material = challengeRowColors[i / 3]; //GetMaterial();
            cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            cube.transform.localScale = Vector3.one * .9f;

            cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
            cube.name = (18 - i).ToString();
            cube.GetComponent<TileScript>().SetParentComponent(this);
            // cube.transform.parent = transform;
            cubes.Add(cube);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    private Vector3 mouseDownPosition;
    private Vector3 dragAxis;
    private Vector3 lockedPosition;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 initialWorldPosition;
    private List<GameObject> movingCubes;
    private int row;
    private int col;
    private GameObject dragChild;
    private int itemIndex;
    private bool inDrag;
    private Vector3 savedTransformPosition;
    private Vector3 incrementalTransformPosition;
    private GameObject clone1;
    private GameObject clone2;

    void OnMouseDown() {
        savedTransformPosition = transform.position;
        incrementalTransformPosition = transform.position;
        mouseDownPosition = Input.mousePosition;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        initialWorldPosition = transform.position;
        offset = transform.position -
                 Camera.main.ScreenToWorldPoint(
                     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        // pointer.transform.position =  Camera.main.ScreenToWorldPoint(
        //     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        dragAxis = Vector3.zero;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);

        foreach (var hit in hits) {
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
       
        if (dragAxis == Vector3.zero) {
            var diff = Input.mousePosition - mouseDownPosition;
            if (!(diff.magnitude > 5f)) return;
            
            if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                //drag is on the x axis
                dragAxis = Vector3.right;
                lockedPosition = new Vector3(0, mouseDownPosition.y, 0);

                var rowInfo = GetRowOf(dragChild);
                foreach (var cube in rowInfo.Item1) {
                    cube.transform.parent = transform;
                }

                movingCubes = rowInfo.Item1;

                row = rowInfo.Item2;
                col = rowInfo.Item3;
                
                clone1 = Instantiate(movingCubes.First(), transform);
                clone1.transform.position = movingCubes.Last().transform.position + dragAxis;
                clone2 = Instantiate(movingCubes.Last(), transform);
                clone2.transform.position = movingCubes.First().transform.position - dragAxis;
            }
            else {
                dragAxis = new Vector3(0, 1f, 0);
                lockedPosition = new Vector3(mouseDownPosition.x, 0, 0);

                var columnInfo = GetColumnOf(dragChild);
                foreach (var cube in columnInfo.Item1) {
                    cube.transform.parent = transform;
                }

                movingCubes = columnInfo.Item1;
                row = columnInfo.Item2;
                col = columnInfo.Item3;
                
                clone1 = Instantiate(movingCubes.First(), transform);
                clone1.transform.position = movingCubes.Last().transform.position - dragAxis;
                clone2 = Instantiate(movingCubes.Last(), transform);
                clone2.transform.position = movingCubes.First().transform.position + dragAxis;
            }

            return;
        }

        var curScreenPoint = new Vector3(Input.mousePosition.x * dragAxis.x + lockedPosition.x,
            Input.mousePosition.y * dragAxis.y + lockedPosition.y,
            screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        // pointer.transform.position =  Camera.main.ScreenToWorldPoint(
        //     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        Vector3 dragDirection = Vector3.zero;

        if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).x) >= 1.1||
            Mathf.Abs((initialWorldPosition - (curPosition + offset)).y) >= 1.1) {
            OnMouseUp();
            inDrag = false;
            return;
        }

        if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).x) >= 1.0) {
            if ((initialWorldPosition - (curPosition + offset)).x > 0)
                dragDirection = Vector3.left;
            else {
                dragDirection = Vector3.right;
            }
        }
        else if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).y) >= 1.0) {
            if ((initialWorldPosition - (curPosition + offset)).y < 0)
                dragDirection = Vector3.up;
            else {
                dragDirection = Vector3.down;
            }
        }
        if ( dragDirection != Vector3.zero) {
            incrementalTransformPosition += dragDirection;      
            initialWorldPosition = (curPosition + offset);
            pointer.transform.position = offset;
            Destroy(clone1);
            Destroy(clone2);
            
            foreach (var cube in movingCubes) {
                var position = cube.transform.position;
                cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
            }
            
            if (dragDirection == Vector3.left) {
                var cube = movingCubes.First();
                cube.transform.position = movingCubes.Last().transform.position + dragAxis;
                var myIndex = cubes.IndexOf(cube);
                cubes.Remove(cube);
                cubes.Insert(myIndex + 2, cube);
                movingCubes.Remove(cube);
                movingCubes.Add(cube);
                
                clone1 = Instantiate(movingCubes.First(), transform);
                clone1.transform.position = movingCubes.Last().transform.position + dragAxis;
                clone2 = Instantiate(movingCubes.Last(), transform);
                clone2.transform.position = movingCubes.First().transform.position - dragAxis;
                
            }
            else if (dragDirection == Vector3.right) {
                var cube = movingCubes.Last();
                cube.transform.position = movingCubes.First().transform.position - dragAxis;
                var myIndex = cubes.IndexOf(cube);
                cubes.Remove(cube);
                cubes.Insert(myIndex - 2, cube);

                movingCubes.Remove(cube);
                movingCubes.Insert(0, cube);
                
                clone1 = Instantiate(movingCubes.First(), transform);
                clone1.transform.position = movingCubes.Last().transform.position + dragAxis;
                clone2 = Instantiate(movingCubes.Last(), transform);
                clone2.transform.position = movingCubes.First().transform.position - dragAxis;
            }
            else if (dragDirection == Vector3.up) {
                var cube = movingCubes.First();
                cube.transform.position = movingCubes.Last().transform.position - dragAxis;

                movingCubes.Remove(cube);
                movingCubes.Add(cube);

                foreach (var c in movingCubes) {
                    cubes.Remove(c);
                }

                int collumn = this.col;
                foreach (var c in movingCubes) {
                    cubes.Insert(collumn, c);
                    collumn += 3;
                }
                
                clone1 = Instantiate(movingCubes.First(), transform);
                clone1.transform.position = movingCubes.Last().transform.position - dragAxis;
                clone2 = Instantiate(movingCubes.Last(), transform);
                clone2.transform.position = movingCubes.First().transform.position + dragAxis;
            
            }
            else {
                var cube = movingCubes.Last();
                cube.transform.position = movingCubes.First().transform.position + dragAxis;

                movingCubes.Remove(cube);
                movingCubes.Insert(0,cube);

                foreach (var c in movingCubes) {
                    cubes.Remove(c);
                }

                int collumn = this.col;
                foreach (var c in movingCubes) {
                    cubes.Insert(collumn, c);
                    collumn += 3;
                }
                
                clone1 = Instantiate(movingCubes.First(), transform);
                clone1.transform.position = movingCubes.Last().transform.position - dragAxis;
                clone2 = Instantiate(movingCubes.Last(), transform);
                clone2.transform.position = movingCubes.First().transform.position + dragAxis;
            }
            
        }

        transform.position = curPosition + offset;
    }

    Tuple<List<GameObject>, int, int> GetRowOf(GameObject gameObject) {
        List<GameObject> newList = new List<GameObject>();

        var indexOfSelected = cubes.IndexOf(gameObject);
        var rowIndex = indexOfSelected / 3;
        var column = indexOfSelected % 3;
        var startIndex = rowIndex * 3;

        for (int i = startIndex; i < startIndex + 3; i++) {
            newList.Add(cubes[i]);
        }

        return new Tuple<List<GameObject>, int, int>(newList, rowIndex, column);
    }

    Tuple<List<GameObject>, int, int> GetColumnOf(GameObject gameObject) {
        List<GameObject> newList = new List<GameObject>();

        var indexOfSelected = cubes.IndexOf(gameObject);
        var column = indexOfSelected % 3;
        var row = indexOfSelected / 3;
        var startIndex = column;

        for (int i = startIndex; i < 18; i += 3) {
            newList.Add(cubes[i]);
        }

        return new Tuple<List<GameObject>, int, int>(newList, row, column);
    }

    void OnMouseUp() {
        if (!inDrag)
            return;
        
        transform.position = incrementalTransformPosition;
        
        foreach (var cube in cubes) {
            cube.transform.parent = null;
            var position = cube.transform.position;
            cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
        }
        
        Destroy(clone1);
        Destroy(clone2);
        transform.position = savedTransformPosition;
        inDrag = false;
    }

    public void ChildMouseDown(GameObject gameObject) { }

    public void ChildMouseUp() { }

    public void ChildMouseDrag() { }
}