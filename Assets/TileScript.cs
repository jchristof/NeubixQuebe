using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 mouseDownPosition;
    void OnMouseDown() {
        mouseDownPosition = Input.mousePosition;
        Debug.Log(mouseDownPosition);
        //screenPoint = Camera.main.WorldToScreenPoint(Input.mousePosition);
        //offset = Input.mousePosition - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
    }
  
    void OnMouseDrag() {
        Vector3 offset = Input.mousePosition - mouseDownPosition;
        mouseDownPosition = Input.mousePosition;
        if(offset.x < 0)
            transform.position = transform.position.OffsetX(-offset.normalized.magnitude/100);
        else {
            transform.position = transform.position.OffsetX(offset.normalized.magnitude/100);
        }
       // Debug.Log(offset.normalized.magnitude);
//        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
//        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
//        transform.position = curPosition;
    }
}

static class VectorExtensions {
    public static Vector3 OffsetX(this Vector3 pos, float distance) {
        return new Vector3(pos.x + distance, pos.y, pos.z);
    }
}