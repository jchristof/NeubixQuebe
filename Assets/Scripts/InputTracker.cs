using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace {
    public class InputTracker {
        private Vector3 mouseDownPosition;
        private Vector3 lockedPosition;
        private Vector3 screenPoint;
        private Vector3 offset;
        private Vector3 initialWorldPosition;
        private int itemIndex;
        private bool inDrag;
        private Vector3 dragAxis;
        Vector3 dragDirection = Vector3.zero;
        Vector3 curPosition = Vector3.zero;
        
        public void StartTracking(Vector3 initialMouseDownPosition, Vector3 initialTransformPosition) {
            initialWorldPosition = initialTransformPosition;
            dragAxis = Vector3.zero;
            dragDirection = Vector3.zero;
            
            mouseDownPosition = initialMouseDownPosition;
            screenPoint = Camera.main.WorldToScreenPoint(initialTransformPosition);

            offset = initialTransformPosition -
                     Camera.main.ScreenToWorldPoint(
                         new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

            curPosition = offset;
            inDrag = true;
        }

        public void Update(Action<Vector3, float> updateFunc) {
            if (!inDrag || dragDirection == Vector3.zero)
                return;
            
            updateFunc.Invoke(dragDirection, (initialWorldPosition - (curPosition + offset)).magnitude);
            initialWorldPosition = (curPosition + offset);
        }

        private Action endFunc;
        public void OnEnd(Action func) {
            endFunc = func;
        }

        public void End() {
            if (!inDrag)
                return;
            endFunc();
        }
        public void UpdateTracking(Vector3 mousePosition) {
            if (!inDrag)
                return;
            
            var diff = mouseDownPosition - mousePosition;
            if (!(diff.magnitude > 5f)) return;

            if (dragAxis == Vector3.zero) {
                if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                    lockedPosition = new Vector3(0, mouseDownPosition.y, 0);
                    dragAxis = Vector3.right;
                }
                else {
                    lockedPosition = new Vector3(mouseDownPosition.x, 0, 0);
                    dragAxis = Vector3.up;
                }
            }

            var curScreenPoint = new Vector3(Input.mousePosition.x * dragAxis.x + lockedPosition.x,
                Input.mousePosition.y * dragAxis.y + lockedPosition.y,
                screenPoint.z);

            curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            
            // if (Mathf.Abs((initialWorldPosition - (curPosition + offset)).x) >= 1.1 ||
            //     Mathf.Abs((initialWorldPosition - (curPosition + offset)).y) >= 1.1) {
            //     endFunc();
            //     inDrag = false;
            //     return;
            // }
            
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
        }
    }
}