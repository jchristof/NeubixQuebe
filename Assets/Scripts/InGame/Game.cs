using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace InGame {
    public abstract class Game {
        public abstract List<GameObject> GetGameTiles();
        public abstract bool CheckedSolved();
    }

    public class TwoColorGame : Game{
        public TwoColorGame(List<GameObject> cubePoolGrid, Material[] materials) {
            this.cubePoolGrid = cubePoolGrid;
            this.materials = materials;
        }
        
        private List<GameObject> cubes = new List<GameObject>();
        private List<GameObject> cubePoolGrid;
        private Material[] materials;
        
        
        private int[] twoColorLayout = {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        
        public override List<GameObject> GetGameTiles() {
            if (cubes.Any())
                return cubes;

            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePoolGrid[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.rotation = new Quaternion(0, 0, 0, 0);
                cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.SetActive(true);
                cubes.Add(cube);
            }

            int[] scrambled = (int[])twoColorLayout.Clone();
            for (int i = 0; i < scrambled.Length; i++) {
                int rnd = Random.Range(0, scrambled.Length);
                int temp = scrambled[rnd];
                scrambled[rnd] = scrambled[i];
                scrambled[i] = temp;
            }

            for (int i = 0; i < cubes.Count; i++) {
                cubes[i].GetComponent<Renderer>().material = materials[scrambled[i]];
                cubes[i].GetComponent<TileScript>().Identifier = scrambled[i];
                cubes[i].GetComponent<TileScript>().image.enabled = false;
                cubes[i].GetComponent<TileScript>().text.enabled = false;
            }

            return cubes;
        }
        
        public override bool CheckedSolved() {
            for (int i = 0; i < 18; i++) {
                var tileScript = cubes[i].GetComponent<TileScript>();
                if (tileScript.Identifier != twoColorLayout[i])
                    return false;
            }

            return true;
        }
    }
    
    public class ThreeColorGame : Game{
        public ThreeColorGame(List<GameObject> cubePoolGrid, Material[] materials) {
            this.cubePoolGrid = cubePoolGrid;
            this.materials = materials;
        }
        
        private List<GameObject> cubes = new List<GameObject>();
        private List<GameObject> cubePoolGrid;
        private Material[] materials;
        
        
        private int[] colorLayout = {0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1};
        
        public override List<GameObject> GetGameTiles() {
            if (cubes.Any())
                return cubes;

            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePoolGrid[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.rotation = new Quaternion(0, 0, 0, 0);
                cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.SetActive(true);
                cubes.Add(cube);
            }

            int[] scrambled = (int[])colorLayout.Clone();
            for (int i = 0; i < scrambled.Length; i++) {
                int rnd = Random.Range(0, scrambled.Length);
                int temp = scrambled[rnd];
                scrambled[rnd] = scrambled[i];
                scrambled[i] = temp;
            }

            for (int i = 0; i < cubes.Count; i++) {
                cubes[i].GetComponent<Renderer>().material = materials[scrambled[i]];
                cubes[i].GetComponent<TileScript>().Identifier = scrambled[i];
                cubes[i].GetComponent<TileScript>().image.enabled = false;
                cubes[i].GetComponent<TileScript>().text.enabled = false;
            }

            return cubes;
        }
        
        public override bool CheckedSolved() {
            for (int i = 0; i < 18; i++) {
                var tileScript = cubes[i].GetComponent<TileScript>();
                if (tileScript.Identifier != colorLayout[i])
                    return false;
            }

            return true;
        }
    }
    
        public class NumberedGame : Game{
        public NumberedGame(List<GameObject> cubePoolGrid, Material[] materials) {
            this.cubePoolGrid = cubePoolGrid;
            this.materials = materials;
        }
        
        private List<GameObject> cubes = new List<GameObject>();
        private List<GameObject> cubePoolGrid;
        private Material[] materials;
        
        
        private int[] numberLayout = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18};
        
        public override List<GameObject> GetGameTiles() {
            if (cubes.Any())
                return cubes;

            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePoolGrid[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.rotation = new Quaternion(0, 0, 0, 0);
                cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.GetComponent<TileScript>().image.enabled = false;
                cube.GetComponent<TileScript>().text.enabled = true;
                cube.GetComponent<TileScript>().text.color = Color.black;
                cube.SetActive(true);
                cubes.Add(cube);
            }

            int[] scrambled = (int[])numberLayout.Clone();
            for (int i = 0; i < scrambled.Length; i++) {
                int rnd = Random.Range(0, scrambled.Length);
                int temp = scrambled[rnd];
                scrambled[rnd] = scrambled[i];
                scrambled[i] = temp;
            }

            for (int i = 0; i < cubes.Count; i++) {
                cubes[i].GetComponent<Renderer>().material = materials[1];
                cubes[i].GetComponent<TileScript>().Identifier = scrambled[i];
                cubes[i].GetComponentInChildren<Text>().text = scrambled[i].ToString();
                
                // cubes[i].GetComponent<TileScript>().image.enabled = false;
                // cubes[i].GetComponent<TileScript>().text.enabled = false;
            }

            return cubes;
        }
        
        public override bool CheckedSolved() {
            for (int i = 0; i < 18; i++) {
                var tileScript = cubes[i].GetComponent<TileScript>();
                if (tileScript.Identifier + 1 != i)
                    return false;
            }

            return true;
        }
    }
}