using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class TileScript : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private ChallengeMenu _challengeMenu;
    public void SetParentComponent(ChallengeMenu challengeMenu) {
        _challengeMenu = challengeMenu;
    }
    
    
    private Vector3 mouseDownPosition;
    private Vector3 dragDirection;
    private Vector3 lockedPosition;
    void OnMouseDown() {
        mouseDownPosition = Input.mousePosition;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset =  transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,screenPoint.z));
        dragDirection = Vector3.zero;

        _challengeMenu.ChildMouseDown(gameObject);
    }
  
    void OnMouseDrag() {
        var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var transformToScreen = Camera.main.WorldToScreenPoint(transform.position);
        //transform.position  = new Vector3(transform.position.x + worldPoint.x, transform.position.y, transform.position.z);
        Debug.Log("Mouse position:" + Input.mousePosition);
        Debug.Log("Transform position:" + transform.position);
        Debug.Log("Mouse to world position:" + worldPoint);
        Debug.Log("Transform to screen position:" + transformToScreen);

        var diff = Input.mousePosition - mouseDownPosition;
        if (dragDirection == Vector3.zero) {
            if (diff.magnitude > .5f) {
                if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                    dragDirection = new Vector3(1f, 0, 1);
                    lockedPosition = new Vector3(0, Input.mousePosition.y, 0);
                }
                else {
                    dragDirection = new Vector3(0, 1f, 1);
                    lockedPosition = new Vector3(Input.mousePosition.x, 0, 0);
                }
            }

            return;
        }

        var curScreenPoint = new Vector3(Input.mousePosition.x * dragDirection.x + lockedPosition.x, Input.mousePosition.y * dragDirection.y + lockedPosition.y,
            screenPoint.z);
        
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
       
        _challengeMenu.ChildMouseDrag();
    }
}

static class VectorExtensions {
    public static Vector3 OffsetX(this Vector3 pos, float distance) {
        return new Vector3(pos.x + distance, pos.y, pos.z);
    }
}