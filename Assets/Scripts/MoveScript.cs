using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CollectionFunction =
    System.Func<UnityEngine.GameObject, System.Collections.Generic.List<UnityEngine.GameObject>, int, int,
        MovingCubesCollection>;
using Object = UnityEngine.Object;

abstract class MovingCubesCollection {
    public List<GameObject> allCubes;
    public List<GameObject> movers = new List<GameObject>();
    public int row;
    public int column;
    public Vector3 dragAxis;

    protected GameObject clone1;
    protected GameObject clone2;

    public GameObject Head() => movers.First();
    public GameObject Tail() => movers.Last();

    public void AttachToMoverParent(Transform transform) {
        foreach (var cube in movers)
            cube.transform.parent = transform;
    }

    public void DetachFromMoverParent() {
        foreach (var cube in movers)
            cube.transform.parent = null;
    }

    public abstract void ProvideWrapClones(Transform parentTransform);

    public void LeftWrap() {
        var cube = movers.First();
        cube.transform.position = movers.Last().transform.position + dragAxis;
        var myIndex = allCubes.IndexOf(cube);
        allCubes.Remove(cube);
        allCubes.Insert(myIndex + 2, cube);
        movers.Remove(cube);
        movers.Add(cube);
    }

    public void RightWrap() {
        var cube = movers.Last();
        cube.transform.position = movers.First().transform.position - dragAxis;
        var myIndex = allCubes.IndexOf(cube);
        allCubes.Remove(cube);
        allCubes.Insert(myIndex - 2, cube);

        movers.Remove(cube);
        movers.Insert(0, cube);
    }

    public void UpWrap() {
        var cube = movers.First();
        cube.transform.position = movers.Last().transform.position - dragAxis;

        movers.Remove(cube);
        movers.Add(cube);

        foreach (var c in movers) {
            allCubes.Remove(c);
        }

        var col = column;
        foreach (var c in movers) {
            allCubes.Insert(col, c);
            col += 3;
        }
    }

    public void DownWrap() {
        var cube = movers.Last();
        cube.transform.position = movers.First().transform.position + dragAxis;

        movers.Remove(cube);
        movers.Insert(0, cube);

        foreach (var c in movers) {
            allCubes.Remove(c);
        }

        var col = column;
        foreach (var c in movers) {
            allCubes.Insert(col, c);
            col += 3;
        }
    }

    public void DestroyWrapClones() {
        Object.Destroy(clone1);
        Object.Destroy(clone2);
    }

    public void ClampPositions() {
        foreach (var cube in movers) {
            var position = cube.transform.position;
            cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
        }
    }
}

class RowCollection : MovingCubesCollection {
    public RowCollection() {
        dragAxis = Vector3.right;
    }

    public override void ProvideWrapClones(Transform parentTransform) {
        clone1 = Object.Instantiate(Head(), parentTransform);
        clone1.transform.position = Tail().transform.position + dragAxis;
        clone2 = Object.Instantiate(Tail(), parentTransform);
        clone2.transform.position = Head().transform.position - dragAxis;
    }
}

class ColumnCollection : MovingCubesCollection {
    public ColumnCollection() {
        dragAxis = Vector3.up;
    }

    public override void ProvideWrapClones(Transform parentTransform) {
        clone1 = Object.Instantiate(Head(), parentTransform);
        clone1.transform.position = Tail().transform.position - dragAxis;
        clone2 = Object.Instantiate(Tail(), parentTransform);
        clone2.transform.position = Head().transform.position + dragAxis;
    }
}

public class MoveScript : MonoBehaviour {
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
            //cube.tag = "id here";
            cubes.Add(cube);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    private Vector3 mouseDownPosition;
    private Vector3 lockedPosition;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 initialWorldPosition;
    private GameObject dragChild;
    private int itemIndex;
    private bool inDrag;
    private Vector3 savedTransformPosition;
    private Vector3 incrementalTransformPosition;

    private MovingCubesCollection movingCubesCollection;

    RaycastHit[] raycastHits = new RaycastHit[5];

    void OnMouseDown() {
        var initialTransformPosition = transform.position;
        savedTransformPosition = initialTransformPosition;
        incrementalTransformPosition = initialTransformPosition;
        initialWorldPosition = initialTransformPosition;

        mouseDownPosition = Input.mousePosition;
        screenPoint = Camera.main.WorldToScreenPoint(initialTransformPosition);

        offset = initialTransformPosition -
                 Camera.main.ScreenToWorldPoint(
                     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.RaycastNonAlloc(ray, raycastHits);

        foreach (var hit in raycastHits) {
            if (!hit.transform.GetComponent<TileScript>()) continue;
            dragChild = hit.transform.gameObject;
            break;
        }

        if (dragChild == null)
            return;

        inDrag = true;
    }

    private void OnMouseDrag() {
        if (!inDrag)
            return;

        if (movingCubesCollection == null) {
            var diff = Input.mousePosition - mouseDownPosition;
            if (!(diff.magnitude > 5f)) return;

            if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                lockedPosition = new Vector3(0, mouseDownPosition.y, 0);
                movingCubesCollection = GetMovingCubesCollection(dragChild, GetRow);
            }
            else {
                lockedPosition = new Vector3(mouseDownPosition.x, 0, 0);
                movingCubesCollection = GetMovingCubesCollection(dragChild, GetColumn);
            }

            movingCubesCollection.AttachToMoverParent(transform);
            movingCubesCollection.ProvideWrapClones(transform);
        }

        var curScreenPoint = new Vector3(Input.mousePosition.x * movingCubesCollection.dragAxis.x + lockedPosition.x,
            Input.mousePosition.y * movingCubesCollection.dragAxis.y + lockedPosition.y,
            screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        Vector3 dragDirection = Vector3.zero;

        if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).x) >= 1.1 ||
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

        if (dragDirection != Vector3.zero) {
            incrementalTransformPosition += dragDirection;
            initialWorldPosition = curPosition + offset;
            pointer.transform.position = offset;
            
            movingCubesCollection.DestroyWrapClones();
            movingCubesCollection.ClampPositions();
            
            if (dragDirection == Vector3.left) {
                movingCubesCollection.LeftWrap();
                movingCubesCollection.ProvideWrapClones(transform);
            }
            else if (dragDirection == Vector3.right) {
                movingCubesCollection.RightWrap();
                movingCubesCollection.ProvideWrapClones(transform);
            }
            else if (dragDirection == Vector3.up) {
                movingCubesCollection.UpWrap();
                movingCubesCollection.ProvideWrapClones(transform);
            }
            else {
                movingCubesCollection.DownWrap();
                movingCubesCollection.ProvideWrapClones(transform);
            }
        }

        transform.position = curPosition + offset;
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

        movingCubesCollection.DestroyWrapClones();
        movingCubesCollection = null;

        transform.position = savedTransformPosition;
        inDrag = false;
    }

    private CollectionFunction GetRow =
        (clickedGameObject, allObjects, row, column) => {
            var startIndex = row * 3;

            var movingCubesDescription = new RowCollection();
            for (var i = startIndex; i < startIndex + 3; i++) {
                movingCubesDescription.movers.Add(allObjects[i]);
            }

            movingCubesDescription.allCubes = allObjects;
            movingCubesDescription.row = row;
            movingCubesDescription.column = column;

            return movingCubesDescription;
        };

    private CollectionFunction GetColumn =
        (clickedGameObject, allObjects, row, column) => {
            var startIndex = column;

            var movingCubesDescription = new ColumnCollection();
            for (var i = startIndex; i < 18; i += 3) {
                movingCubesDescription.movers.Add(allObjects[i]);
            }

            movingCubesDescription.allCubes = allObjects;
            movingCubesDescription.row = row;
            movingCubesDescription.column = column;

            return movingCubesDescription;
        };

    private MovingCubesCollection GetMovingCubesCollection(GameObject gameObject, CollectionFunction getCollection) {
        var indexOfSelected = cubes.IndexOf(gameObject);
        var row = indexOfSelected / 3;
        var column = indexOfSelected % 3;

        return getCollection(gameObject, cubes, row, column);
    }
}