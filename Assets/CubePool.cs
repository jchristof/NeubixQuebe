﻿using System.Collections.Generic;
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
            go.transform.localScale = Vector3.one * .9f;
            go.SetActive(false);
            cubesPool.Add(go);
        }

        for (var i = 17; i >= 0; i--) {
            GameObject cube = Instantiate(roundEdgeCube);
            cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
            cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            cube.transform.localScale = Vector3.one * .9f;
            cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
            cube.name = (18 - i).ToString();
            cube.SetActive(false);
            cubeGrid.Add(cube);
        }
    }
}