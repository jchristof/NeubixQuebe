using UnityEngine;

namespace DefaultNamespace {
    public interface ChildDragWatcher {
        void ChildMouseDown(GameObject gameObject);
        void ChildMouseUp();
        void ChildMouseDrag();
    }
}