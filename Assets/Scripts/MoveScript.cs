using System;
using System.Collections.Generic;
using System.Numerics;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class MoveScript : MonoBehaviour {
    private List<GameObject> cubes = new List<GameObject>();
    public GameObject pointer;
    private Vector3 savedPosition;
    void Start() {
        cubes = GetComponent<TileField>().GetTiles();
        savedPosition = transform.position;
        inputTracker.OnEnd(() => {
            //transform.position = incrementalTransformPosition;

            foreach (var cube in cubes) {
                cube.transform.parent = null;
                var position = cube.transform.position;
                cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
            }

            movingCubesCollection.DestroyWrapClones();
            movingCubesCollection = null;

            transform.position = savedPosition;
        });
    }

    private float accumulatedMag;
    // Update is called once per frame
    void Update() {
        
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        
        inputTracker.Update((direction, magnitude) => {
            accumulatedMag += magnitude;
            if (movingCubesCollection == null) {
                if (direction == Vector3.left || direction == Vector3.right)
                    movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes, TileMover.GetRow);
                else
                    movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes, TileMover.GetColumn);

                movingCubesCollection.AttachToMoverParent(transform);
                //movingCubesCollection.ClampPositions();
            }

            if (accumulatedMag > 1.0) {
                if (direction == Vector3.left) {
                    movingCubesCollection.LeftWrap();
                    movingCubesCollection.ProvideWrapClones(transform);
                }
                else if (direction == Vector3.right) {
                    movingCubesCollection.RightWrap();
                    movingCubesCollection.ProvideWrapClones(transform);
                }
                else if (direction == Vector3.up) {
                    movingCubesCollection.UpWrap();
                    movingCubesCollection.ProvideWrapClones(transform);
                }
                else {
                    movingCubesCollection.DownWrap();
                    movingCubesCollection.ProvideWrapClones(transform);
                }
            }

            transform.position += direction * magnitude;
        });
    }

    private MovingCubesCollection movingCubesCollection;
    private InputTracker inputTracker = new InputTracker();
    private GameObject dragChild;
    RaycastHit[] raycastHits = new RaycastHit[5];
    
    void OnMouseDown() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.RaycastNonAlloc(ray, raycastHits);

        foreach (var hit in raycastHits) {
            if (!hit.transform.GetComponent<TileScript>()) continue;
            dragChild = hit.transform.gameObject;
            break;
        }

        if (dragChild == null)
            return;
        
        inputTracker.StartTracking(Input.mousePosition, transform.position);
    }

    private void OnMouseDrag() {
        inputTracker.UpdateTracking(Input.mousePosition);

        // if (movingCubesCollection == null) {
        //     var diff = Input.mousePosition - mouseDownPosition;
        //     if (!(diff.magnitude > 5f)) return;
        //
        //     if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
        //         lockedPosition = new Vector3(0, mouseDownPosition.y, 0);
        //         movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes, TileMover.GetRow);
        //     }
        //     else {
        //         lockedPosition = new Vector3(mouseDownPosition.x, 0, 0);
        //         movingCubesCollection = TileMover.GetMovingCubesCollection(dragChild, cubes, TileMover.GetColumn);
        //     }
        //
        //     movingCubesCollection.AttachToMoverParent(transform);
        //     movingCubesCollection.ProvideWrapClones(transform);
        // }
        //
        // var curScreenPoint = new Vector3(Input.mousePosition.x * movingCubesCollection.dragAxis.x + lockedPosition.x,
        //     Input.mousePosition.y * movingCubesCollection.dragAxis.y + lockedPosition.y,
        //     screenPoint.z);
        //
        // Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        // Vector3 dragDirection = Vector3.zero;
        //
        // if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).x) >= 1.1 ||
        //     Mathf.Abs((initialWorldPosition - (curPosition + offset)).y) >= 1.1) {
        //     OnMouseUp();
        //     inDrag = false;
        //     return;
        // }
        //
        // if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).x) >= 1.0) {
        //     if ((initialWorldPosition - (curPosition + offset)).x > 0)
        //         dragDirection = Vector3.left;
        //     else {
        //         dragDirection = Vector3.right;
        //     }
        // }
        // else if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).y) >= 1.0) {
        //     if ((initialWorldPosition - (curPosition + offset)).y < 0)
        //         dragDirection = Vector3.up;
        //     else {
        //         dragDirection = Vector3.down;
        //     }
        // }
        //
        // if (dragDirection != Vector3.zero) {
        //     incrementalTransformPosition += dragDirection;
        //     initialWorldPosition = curPosition + offset;
        //     pointer.transform.position = offset;
        //     
        //     movingCubesCollection.DestroyWrapClones();
        //     movingCubesCollection.ClampPositions();
        //     
        //     if (dragDirection == Vector3.left) {
        //         movingCubesCollection.LeftWrap();
        //         movingCubesCollection.ProvideWrapClones(transform);
        //     }
        //     else if (dragDirection == Vector3.right) {
        //         movingCubesCollection.RightWrap();
        //         movingCubesCollection.ProvideWrapClones(transform);
        //     }
        //     else if (dragDirection == Vector3.up) {
        //         movingCubesCollection.UpWrap();
        //         movingCubesCollection.ProvideWrapClones(transform);
        //     }
        //     else {
        //         movingCubesCollection.DownWrap();
        //         movingCubesCollection.ProvideWrapClones(transform);
        //     }
        // }

        //transform.position = curPosition + offset;
    }

    void OnMouseUp() {
        inputTracker.End();
    }
}