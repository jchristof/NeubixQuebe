using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class GameModeChallengeMenu  : MonoBehaviour, TileField, GameMode {
        public GameObject roundCornerCube;
        public List<Material> challengeRowColors;
        public Sprite crown;
        private List<GameObject> cubes = new List<GameObject>();
        public List<GameObject> GetTiles() {
            return cubes;
        }
        void Start() {
            for (var i = 17; i >= 0; i--) {
                GameObject cube = Instantiate(roundCornerCube);
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<Renderer>().material = challengeRowColors[i / 3];
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.localScale = Vector3.one * .9f;
                cube.GetComponent<TileScript>().text.text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.GetComponent<TileScript>().image.enabled = false;
                cubes.Add(cube);
            }
            cubes.Last().GetComponent<TileScript>().image.enabled = true;
            cubes.Last().GetComponent<TileScript>().image.sprite = crown;
            cubes.Last().GetComponent<TileScript>().text.enabled = false;
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