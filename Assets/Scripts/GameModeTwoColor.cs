using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class GameModeTwoColor : MonoBehaviour, TileField, GameMode {
        public List<Material> challengeRowColors;
        public int[] layout = new int[18];
        public CubePool cubePool;
        private  List<GameObject> cubes = new List<GameObject>();
        public List<GameObject> GetTiles() {
            if (cubes.Any())
                return cubes;
            
            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePool.cubeGrid[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.localScale = Vector3.one * .9f;
                cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.SetActive(true);
                cubes.Add(cube);
            }

            for (int i = 0; i < layout.Length; i++) {
                int rnd = Random.Range(0, layout.Length);
                int temp = layout[rnd];
                layout[rnd] = layout[i];
                layout[i] = temp;
            }

            for (int i = 0; i < cubes.Count; i++) {
                cubes[i].GetComponent<Renderer>().material = challengeRowColors[layout[i]];
                cubes[i].GetComponent<TileScript>().Identifier = layout[i];
                cubes[i].GetComponent<TileScript>().image.enabled = false;
                cubes[i].GetComponent<TileScript>().text.enabled = true;
            }

            return cubes;
        }
        
        public void OnDisable() {
            foreach (var c in cubes) {
                c.SetActive(false);
            }
            cubes.Clear();
        }

        public bool CheckSolved() {
            for (int i = 0; i < 9; i++) {
                if (cubes[i].GetComponent<TileScript>().Identifier != 0)
                    return false;
            }

            for (int i = 9; i < 18; i++) {
                if (cubes[i].GetComponent<TileScript>().Identifier != 1)
                    return false;
            }

            return true;
        }
    }
}