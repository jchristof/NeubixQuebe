using DefaultNamespace;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private ChildDragWatcher _childDragWatcher;
    public void SetParentComponent(ChildDragWatcher challengeMenu) {
        _childDragWatcher = challengeMenu;
    }
    
    

    void OnMouseDown() {
        _childDragWatcher.ChildMouseDown(gameObject);
    }

    private void OnMouseUp() {
        _childDragWatcher.ChildMouseUp();
    }

    void OnMouseDrag() {
        _childDragWatcher.ChildMouseDrag();
    }
}

static class VectorExtensions {
    public static Vector3 OffsetX(this Vector3 pos, float distance) {
        return new Vector3(pos.x + distance, pos.y, pos.z);
    }
}