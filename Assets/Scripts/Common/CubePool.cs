using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CubePool : MonoBehaviour {
    public List<GameObject> cubesPool { get; } = new List<GameObject>();
    public List<GameObject> cubeGrid { get; } = new List<GameObject>();
    public GameObject roundEdgeCube;

    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < 40; i++) {
            var go = Instantiate(roundEdgeCube);
            go.GetComponent<TileScript>().image.enabled = false;
            go.GetComponentInChildren<TileScript>().text.enabled = false;
            go.SetActive(false);
            cubesPool.Add(go);
        }

        for (var i = 17; i >= 0; i--) {
            GameObject cube = Instantiate(roundEdgeCube);
            cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
            cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            cube.transform.rotation = new Quaternion(0,0,0,0);
            cube.GetComponentInChildren<TextMeshProUGUI>().text = (18 - i).ToString();
            cube.name = (18 - i).ToString();
            cube.SetActive(false);
            cube.GetComponent<Animation>().Stop();
            cubeGrid.Add(cube);
        }
    }

    public void HideAll() {
        foreach (GameObject o in cubeGrid) {
            o.SetActive(false);
        }
    }

    public void ShowAll() {
        foreach (GameObject o in cubeGrid) {
            o.SetActive(true);
        }
    }
    
    public void Update() {
        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            var allCubes = new List<GameObject>();
            allCubes.AddRange(cubesPool);
            allCubes.AddRange(cubeGrid);
            var textEnabled = allCubes[0].GetComponentInChildren<TileScript>().text.enabled;
            foreach (var cube in allCubes) {
                cube.GetComponentInChildren<TileScript>().text.enabled = !textEnabled;
                cube.GetComponentInChildren<TileScript>().text.text =
                    cube.GetComponentInChildren<TileScript>().Identifier.ToString();
            }
        }
    }
}