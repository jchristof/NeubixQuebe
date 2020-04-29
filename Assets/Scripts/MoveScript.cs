using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
                movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes, TileMover.GetRow);
            }
            else {
                lockedPosition = new Vector3(mouseDownPosition.x, 0, 0);
                movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes, TileMover.GetColumn);
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
}