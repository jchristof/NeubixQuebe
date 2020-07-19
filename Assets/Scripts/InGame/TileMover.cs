using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CollectionFunction =
    System.Func<UnityEngine.GameObject, System.Collections.Generic.List<UnityEngine.GameObject>, int, int,
        System.Collections.Generic.List<UnityEngine.GameObject>, UnityEngine.Transform,
        MovingCubesCollection>;

static class TileMover {
    public static CollectionFunction GetRow =
        (clickedGameObject, allObjects, row, column, pool, transform) => {
            var startIndex = row * 3;

            var movingCubesDescription = new RowCollection(pool, transform);
            for (var i = startIndex; i < startIndex + 3; i++) {
                movingCubesDescription.movers.Add(allObjects[i]);
            }

            movingCubesDescription.allCubes = allObjects;
            movingCubesDescription.row = row;
            movingCubesDescription.column = column;

            return movingCubesDescription;
        };

    public static CollectionFunction GetColumn =
        (clickedGameObject, allObjects, row, column, pool, transform) => {
            var startIndex = column;

            var movingCubesDescription = new ColumnCollection(pool, transform);
            for (var i = startIndex; i < 18; i += 3) {
                movingCubesDescription.movers.Add(allObjects[i]);
            }

            movingCubesDescription.allCubes = allObjects;
            movingCubesDescription.row = row;
            movingCubesDescription.column = column;

            return movingCubesDescription;
        };

    public static MovingCubesCollection GetMovingCubesCollection(GameObject gameObject, List<GameObject> allCubes,
        CollectionFunction getCollection, List<GameObject> pool, Transform transform) {
        var indexOfSelected = allCubes.IndexOf(gameObject);
        var row = indexOfSelected / 3;
        var column = indexOfSelected % 3;

        return getCollection(gameObject, allCubes, row, column, pool, transform);
    }
}

abstract class MovingCubesCollection {
    public List<GameObject> allCubes;
    public List<GameObject> movers = new List<GameObject>();
    public int row;
    public int column;
    public Vector3 dragAxis;

    // protected List<GameObject> clones;
    protected List<GameObject> pool;
    protected Transform parentTransform;

    public MovingCubesCollection(List<GameObject> pool, Transform parentTransform) {
        this.pool = pool;
        this.parentTransform = parentTransform;
    }

    protected void CloneFrom(GameObject cloneThis, Vector3 newPosition) {
        foreach (var pGameObject in pool) {
            if (pGameObject.activeInHierarchy)
                continue;

            pGameObject.transform.parent = parentTransform;
            pGameObject.transform.position = newPosition;
            pGameObject.GetComponent<Renderer>().material = cloneThis.GetComponent<Renderer>().material;
            pGameObject.GetComponentInChildren<TextMeshProUGUI>().text = cloneThis.GetComponentInChildren<TextMeshProUGUI>().text;
            pGameObject.GetComponentInChildren<TextMeshProUGUI>().color = cloneThis.GetComponentInChildren<TextMeshProUGUI>().color;
            pGameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = cloneThis.GetComponentInChildren<TextMeshProUGUI>().enabled;
            pGameObject.GetComponent<TileScript>().Identifier = cloneThis.GetComponent<TileScript>().Identifier;
            pGameObject.SetActive(true);
            return;
        }
    }

    protected void ReleaseClones() {
        foreach (var pGameObject in pool) {
            pGameObject.transform.parent = null;
            pGameObject.SetActive(false);
        }
    }

    public void AttachToMoverParent(Transform transform) {
        foreach (var cube in movers)
            cube.transform.parent = transform;
    }

    public void DetachFromMoverParent() {
        foreach (var cube in movers)
            cube.transform.parent = null;
    }

    public abstract void ProvideWrapClones();
    public abstract void EndWithWrap(Vector3 distance);

    public void DestroyWrapClones() {
        ReleaseClones();
    }

    public void ClampPositions() {
        foreach (var cube in movers) {
            var position = cube.transform.position;
            cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
        }
    }
}

class RowCollection : MovingCubesCollection {
    public RowCollection(List<GameObject> pool, Transform parentTransform) : base(pool, parentTransform) {
        dragAxis = Vector3.right;
    }

    public override void ProvideWrapClones() {
        for (int i = 0; i < 5 * 3; i++) {
            var cloneThis = movers[i % 3];
            var cloneTransform = cloneThis.transform;
            CloneFrom(cloneThis,
                new Vector3(cloneTransform.position.x + 3 * (i / 3 + 1), cloneTransform.position.y,
                    cloneTransform.position.z));
        }

        for (int i = 0; i < 5 * 3; i++) {
            var cloneThis = movers[i % 3];
            var cloneTransform = cloneThis.transform;
            CloneFrom(
                cloneThis,
                new Vector3(cloneTransform.position.x - 3 * (i / 3), cloneTransform.position.y,
                    cloneTransform.position.z));
        }
    }

    public override void EndWithWrap(Vector3 distance) {
        var count = ((int) distance.x) % 3;

        if (count < 0)
            count = count + 3;

        for (int i = 0; i < count; i++) {
            var m = movers[2];
            movers.Remove(m);
            movers.Insert(0, m);
        }

        foreach (var c in movers) {
            allCubes.Remove(c);
        }

        for (int i = 0; i < 3; i++) {
            allCubes.Insert((row * 3) + i, movers[i]);
            var position = movers[i].transform.position;
            movers[i].transform.position = new Vector3(i, position.y, position.z);
        }
    }
}

class ColumnCollection : MovingCubesCollection {
    public ColumnCollection(List<GameObject> pool, Transform parentTransform) : base(pool, parentTransform) {
        dragAxis = Vector3.up;
    }

    public override void ProvideWrapClones() {
        for (int i = 0; i < 5 * 3; i++) {
            var cloneThis = movers[i % 6];
            var cloneTransform = cloneThis.transform;
            CloneFrom(
                cloneThis,
                new Vector3(cloneTransform.position.x, cloneTransform.position.y + 6 * (i / 6 + 1),
                    cloneTransform.position.z));
        }

        for (int i = 0; i < 5 * 3; i++) {
            var cloneThis = movers[i % 6];
            var cloneTransform = cloneThis.transform;
            CloneFrom(
                cloneThis,
                new Vector3(cloneTransform.position.x, cloneTransform.position.y - 6 * (i / 6 + 1),
                    cloneTransform.position.z));
        }
    }

    public override void EndWithWrap(Vector3 distance) {
        var count = ((int) distance.y) % 6;

        if (count < 0)
            count = count + 6;

        for (int i = 0; i < count; i++) {
            var m = movers[0];
            movers.Remove(m);
            movers.Insert(movers.Count, m);
        }

        foreach (var c in movers) {
            allCubes.Remove(c);
        }

        for (int i = 0; i < 6; i++) {
            allCubes.Insert(column + (i * 3), movers[i]);
            var position = movers[i].transform.position;
            movers[i].transform.position = new Vector3(column, 5 - i, position.z);
        }
    }
}