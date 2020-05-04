using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScript : MonoBehaviour {
    private List<GameObject> cubes = new List<GameObject>();
    private readonly List<GameObject> objectPool = new List<GameObject>();
    private GameMode gameMode;
    void Start() {
        cubes = GetComponent<TileField>().GetTiles();
        gameMode = GetComponent<GameMode>();
        for (int i = 0; i < 40; i++) {
            var go = Instantiate(cubes[0]);
            go.SetActive(false);
            objectPool.Add(go);
        }
    }
    
    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        
        if (!inDrag)
            return;

        if (movingCubesCollection == null) {
            var diff = Input.mousePosition - mouseDownPosition;
            if (!(diff.magnitude > 5f)) return;

            if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                lockedPosition = new Vector3(0, mouseDownPosition.y, 0);
                movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes, TileMover.GetRow, objectPool, transform);
            }
            else {
                lockedPosition = new Vector3(mouseDownPosition.x, 0, 0);
                movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes, TileMover.GetColumn, objectPool, transform);
            }

            movingCubesCollection.AttachToMoverParent(transform);
            movingCubesCollection.ProvideWrapClones();
        }

        var curScreenPoint = new Vector3(Input.mousePosition.x * movingCubesCollection.dragAxis.x + lockedPosition.x,
            Input.mousePosition.y * movingCubesCollection.dragAxis.y + lockedPosition.y,
            screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);

        if ((curPosition - savedTransformPosition).magnitude > 10) {
            OnMouseUp();
            return;
        }

        transform.position = curPosition + offset;
    }

    private Vector3 mouseDownPosition;
    private Vector3 lockedPosition;
    private Vector3 screenPoint;
    private Vector3 offset;
    private GameObject dragChild;
    private int itemIndex;
    private bool inDrag;
    private Vector3 savedTransformPosition;

    private MovingCubesCollection movingCubesCollection;

    RaycastHit[] raycastHits = new RaycastHit[5];

    void OnMouseDown() {
        var initialTransformPosition = transform.position;
        savedTransformPosition = initialTransformPosition;

        mouseDownPosition = Input.mousePosition;
        screenPoint = Camera.main.WorldToScreenPoint(initialTransformPosition);

        offset = initialTransformPosition -
                 Camera.main.ScreenToWorldPoint(
                     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.RaycastNonAlloc(ray, raycastHits);

        foreach (var hit in raycastHits) {
            if (hit.transform.GetComponent<TileScript>() == null) continue;
            dragChild = hit.transform.gameObject;
            break;
        }

        if (dragChild == null)
            return;

        inDrag = true;
    }

    void OnMouseUp() {
        if (!inDrag)
            return;

        Vector3 offset = transform.transform.position - savedTransformPosition;
        offset = new Vector3(Mathf.Round(offset.x),Mathf.Round(offset.y), 0f);

        transform.transform.position = savedTransformPosition + offset;

        movingCubesCollection.EndWithWrap(transform.transform.position - savedTransformPosition);
        foreach (var cube in cubes) {
            cube.transform.parent = null;
            var position = cube.transform.position;
            cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
        }

        movingCubesCollection.DestroyWrapClones();
        movingCubesCollection = null;

        transform.position = savedTransformPosition;
        inDrag = false;
        
        if(gameMode.CheckSolved())
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}