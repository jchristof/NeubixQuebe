using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript {
    public List<GameObject> cubePool;
    private List<GameObject> cubes;
    private Transform transform;
    private bool enabled;
    public MoveScript(Transform transform, List<GameObject> cubePool, List<GameObject> cubes) {
        this.transform = transform;
        this.cubePool = cubePool;
        this.cubes = cubes;
    }

    public void Enable() => enabled = true;
    public void Disable() => enabled = false;
    
    public void Update() {
        if (!inDrag)
            return;

        if (movingCubesCollection == null) {
            var diff = Input.mousePosition - mouseDownPosition;
            if (!(diff.magnitude > 5f)) return;

            if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                lockedPosition = new Vector3(0, mouseDownPosition.y, 0);
                movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes,
                    TileMover.GetRow, cubePool, transform);
            }
            else {
                lockedPosition = new Vector3(mouseDownPosition.x, 0, 0);
                movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes,
                    TileMover.GetColumn, cubePool, transform);
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

    public void Stop() {
        OnMouseUp();
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

    public void OnMouseDown() {
        if (!enabled)
            return;
        
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
            var component = hit.transform?.GetComponent(typeof(TileScript));
            if (component == null) continue;
            
            dragChild = hit.transform.gameObject;
            break;
        }

        if (dragChild == null)
            return;

        inDrag = true;
    }

    public int OnMouseUp() {
        if (!inDrag || !enabled)
            return 0;

        Vector3 offset = transform.transform.position - savedTransformPosition;
        offset = new Vector3(Mathf.Round(offset.x), Mathf.Round(offset.y), 0f);

        Debug.Log("offset:" + offset);
        transform.transform.position = savedTransformPosition + offset;

        movingCubesCollection?.EndWithWrap(transform.transform.position - savedTransformPosition);
        foreach (var cube in cubes) {
            var position = cube.transform.position;
            cube.transform.parent = null;
           
            cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
        }

        movingCubesCollection?.DestroyWrapClones();
        movingCubesCollection = null;

        transform.position = savedTransformPosition;
        inDrag = false;

        return (int) offset.magnitude;
    }
}