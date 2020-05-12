using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefaultNamespace {
    public class GameModeChallengeMenu : MonoBehaviour, TileField, GameMode {
        public List<Material> challengeRowColors;
        public Sprite crown;
        public CubePool cubePool;
        public List<GameObject> cubes = new List<GameObject>();
        public List<GameObject> GetTiles() {
            if(cubes.Any())
                return cubes;

            ConfigureCubes();
            return cubes;
        }

        void ConfigureCubes() {
            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePool.cubeGrid[i];
                cube.SetActive(true);
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<Renderer>().material = challengeRowColors[i / 3];
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
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
            cubes.Clear();
        }
    }
}