using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class GameModeChallengeMenu  : MonoBehaviour, TileField, GameMode {
        public GameObject roundCornerCube;
        public List<Material> challengeRowColors;
    
        private List<GameObject> cubes = new List<GameObject>();
        public List<GameObject> GetTiles() {
            return cubes;
        }
        void Start() {
            for (var i = 17; i >= 0; i--) {
                GameObject cube = Instantiate(roundCornerCube);
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<Renderer>().material = challengeRowColors[i / 3]; //GetMaterial();
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.localScale = Vector3.one * .9f;
 
                cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                //cube.tag = "id here";
                cubes.Add(cube);
            }
        }

        public bool CheckSolved() {
            return false;
        }

        public void OnDisable() {
            foreach (var c in cubes) {
                c.SetActive(false);
            }
        }

        public void OnEnable() {
            foreach (var c in cubes) {
                c.SetActive(true);
            }
        }
    }
}