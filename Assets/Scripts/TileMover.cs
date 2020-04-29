using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CollectionFunction = System.Func<UnityEngine.GameObject, System.Collections.Generic.List<UnityEngine.GameObject>, int, int,
    MovingCubesCollection>;

static class TileMover {
    public static CollectionFunction GetRow =
        (clickedGameObject, allObjects, row, column) => {
            var startIndex = row * 3;

            var movingCubesDescription = new RowCollection();
            for (var i = startIndex; i < startIndex + 3; i++) {
                movingCubesDescription.movers.Add(allObjects[i]);
            }

            movingCubesDescription.allCubes = allObjects;
            movingCubesDescription.row = row;
            movingCubesDescription.column = column;

            return movingCubesDescription;
        };

    public static CollectionFunction GetColumn =
        (clickedGameObject, allObjects, row, column) => {
            var startIndex = column;

            var movingCubesDescription = new ColumnCollection();
            for (var i = startIndex; i < 18; i += 3) {
                movingCubesDescription.movers.Add(allObjects[i]);
            }

            movingCubesDescription.allCubes = allObjects;
            movingCubesDescription.row = row;
            movingCubesDescription.column = column;

            return movingCubesDescription;
        };

    public static MovingCubesCollection GetMovingCubesCollection(GameObject gameObject, List<GameObject> allCubes, CollectionFunction getCollection) {
        var indexOfSelected = allCubes.IndexOf(gameObject);
        var row = indexOfSelected / 3;
        var column = indexOfSelected % 3;

        return getCollection(gameObject, allCubes, row, column);
    }
}
abstract class MovingCubesCollection {
    public List<GameObject> allCubes;
    public List<GameObject> movers = new List<GameObject>();
    public int row;
    public int column;
    public Vector3 dragAxis;

    protected GameObject clone1;
    protected GameObject clone2;

    public GameObject Head() => movers.First();
    public GameObject Tail() => movers.Last();

    public void AttachToMoverParent(Transform transform) {
        foreach (var cube in movers)
            cube.transform.parent = transform;
    }

    public void DetachFromMoverParent() {
        foreach (var cube in movers)
            cube.transform.parent = null;
    }

    public abstract void ProvideWrapClones(Transform parentTransform);

    public void LeftWrap() {
        var cube = movers.First();
        cube.transform.position = movers.Last().transform.position + dragAxis;
        var myIndex = allCubes.IndexOf(cube);
        allCubes.Remove(cube);
        allCubes.Insert(myIndex + 2, cube);
        movers.Remove(cube);
        movers.Add(cube);
    }

    public void RightWrap() {
        var cube = movers.Last();
        cube.transform.position = movers.First().transform.position - dragAxis;
        var myIndex = allCubes.IndexOf(cube);
        allCubes.Remove(cube);
        allCubes.Insert(myIndex - 2, cube);

        movers.Remove(cube);
        movers.Insert(0, cube);
    }

    public void UpWrap() {
        var cube = movers.First();
        cube.transform.position = movers.Last().transform.position - dragAxis;

        movers.Remove(cube);
        movers.Add(cube);

        foreach (var c in movers) {
            allCubes.Remove(c);
        }

        var col = column;
        foreach (var c in movers) {
            allCubes.Insert(col, c);
            col += 3;
        }
    }

    public void DownWrap() {
        var cube = movers.Last();
        cube.transform.position = movers.First().transform.position + dragAxis;

        movers.Remove(cube);
        movers.Insert(0, cube);

        foreach (var c in movers) {
            allCubes.Remove(c);
        }

        var col = column;
        foreach (var c in movers) {
            allCubes.Insert(col, c);
            col += 3;
        }
    }

    public void DestroyWrapClones() {
        Object.Destroy(clone1);
        Object.Destroy(clone2);
    }

    public void ClampPositions() {
        foreach (var cube in movers) {
            var position = cube.transform.position;
            cube.transform.position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), position.z);
        }
    }
}

class RowCollection : MovingCubesCollection {
    public RowCollection() {
        dragAxis = Vector3.right;
    }

    public override void ProvideWrapClones(Transform parentTransform) {
        clone1 = Object.Instantiate(Head(), parentTransform);
        clone1.transform.position = Tail().transform.position + dragAxis;
        clone2 = Object.Instantiate(Tail(), parentTransform);
        clone2.transform.position = Head().transform.position - dragAxis;
    }
}

class ColumnCollection : MovingCubesCollection {
    public ColumnCollection() {
        dragAxis = Vector3.up;
    }

    public override void ProvideWrapClones(Transform parentTransform) {
        clone1 = Object.Instantiate(Head(), parentTransform);
        clone1.transform.position = Tail().transform.position - dragAxis;
        clone2 = Object.Instantiate(Tail(), parentTransform);
        clone2.transform.position = Head().transform.position + dragAxis;
    }
}