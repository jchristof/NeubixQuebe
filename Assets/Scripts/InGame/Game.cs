using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace InGame {
    public abstract class Game {
        public abstract List<GameObject> GetGameTiles();
        public abstract bool CheckedSolved();

        public abstract void Reset();
    }

    public class GameFactory {
        public static Game GetGame(GameMode mode, List<GameObject> cubeGrid, List<Material> challengeRowColors, Material numberedMaterial) {
            if (mode == GameMode.TwoColor)
                return new TwoColorGame(cubeGrid, new[] {challengeRowColors[0], challengeRowColors[1]});
            if (mode == GameMode.ThreeColor)
                return new ThreeColorGame(cubeGrid,
                    new[] {challengeRowColors[0], challengeRowColors[1], challengeRowColors[2]});
            if (mode == GameMode.Numbered)
                return new NumberedGame(cubeGrid, numberedMaterial);
            
            throw new NotImplementedException();
        }
    }
    
    public class TwoColorGame : Game {
        public TwoColorGame(List<GameObject> cubePoolGrid, Material[] materials) {
            this.cubePoolGrid = cubePoolGrid;
            this.materials = materials;
        }

        private readonly List<GameObject> cubes = new List<GameObject>();
        private readonly List<GameObject> cubePoolGrid;
        private readonly Material[] materials;

        private readonly int[] twoColorLayout = {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1};

        public override List<GameObject> GetGameTiles() {
            if (cubes.Any())
                return cubes;

            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePoolGrid[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.rotation = new Quaternion(0, 0, 0, 0);
                cube.GetComponentInChildren<TextMeshProUGUI>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.SetActive(false);
                cubes.Add(cube);
            }

            int[] scrambled = (int[]) twoColorLayout.Clone();
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
            var firstIdentifier =  cubes[0].GetComponent<TileScript>().Identifier;
            for (int i = 0; i < 9; i++) {
                var tileScript = cubes[i].GetComponent<TileScript>();
                if (firstIdentifier != tileScript.Identifier)
                    return false;
            }

            return true;
        }

        public override void Reset() {
            cubes.Clear();
        }
    }

    public class ThreeColorGame : Game {
        public ThreeColorGame(List<GameObject> cubePoolGrid, Material[] materials) {
            this.cubePoolGrid = cubePoolGrid;
            this.materials = materials;
        }

        private readonly List<GameObject> cubes = new List<GameObject>();
        private readonly List<GameObject> cubePoolGrid;
        private readonly Material[] materials;

        private readonly int[] colorLayout = {0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1};

        public override List<GameObject> GetGameTiles() {
            if (cubes.Any())
                return cubes;

            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePoolGrid[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.rotation = new Quaternion(0, 0, 0, 0);
                cube.GetComponentInChildren<TextMeshProUGUI>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.SetActive(true);
                cubes.Add(cube);
            }

            int[] scrambled = (int[]) colorLayout.Clone();
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
            var firstIdentifier = cubes[0].GetComponent<TileScript>().Identifier;
            for (int i = 0; i < 6; i++) {
                var tileScript = cubes[i].GetComponent<TileScript>();
                if (firstIdentifier != tileScript.Identifier)
                    return false;
            }
            
            var secondIdentifier = cubes[9].GetComponent<TileScript>().Identifier;
            for (int i = 6; i < 12; i++) {
                var tileScript = cubes[i].GetComponent<TileScript>();
                if (secondIdentifier != tileScript.Identifier)
                    return false;
            }

            return true;
        }

        public override void Reset() {
            cubes.Clear();
        }
    }

    public class NumberedGame : Game {
        public NumberedGame(List<GameObject> cubePoolGrid, Material material) {
            this.cubePoolGrid = cubePoolGrid;
            this.material = material;
        }

        private readonly List<GameObject> cubes = new List<GameObject>();
        private readonly List<GameObject> cubePoolGrid;
        private readonly Material material;


        private readonly int[] numberLayout = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18};

        public override List<GameObject> GetGameTiles() {
            if (cubes.Any())
                return cubes;

            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePoolGrid[i];
                cube.GetComponent<Renderer>().material = material;
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.rotation = new Quaternion(0, 0, 0, 0);
                cube.GetComponent<TileScript>().text.text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.GetComponent<TileScript>().image.enabled = false;
                cube.GetComponent<TileScript>().text.enabled = true;
                cube.GetComponent<TileScript>().text.color = Color.black;
                cube.SetActive(true);
                cubes.Add(cube);
            }

            int[] scrambled = (int[]) numberLayout.Clone();
            for (int i = 0; i < scrambled.Length; i++) {
                int rnd = Random.Range(0, scrambled.Length);
                int temp = scrambled[rnd];
                scrambled[rnd] = scrambled[i];
                scrambled[i] = temp;
            }

            for (int i = 0; i < cubes.Count; i++) {
                
                cubes[i].GetComponent<TileScript>().Identifier = scrambled[i];
                cubes[i].GetComponentInChildren<TextMeshProUGUI>().text = scrambled[i].ToString();
            }

            return cubes;
        }

        public override bool CheckedSolved() {
            for (int i = 0; i < 18; i++) {
                var tileScript = cubes[i].GetComponent<TileScript>();
                if (tileScript.Identifier - 1 != i)
                    return false;
            }

            return true;
        }

        public override void Reset() {
            cubes.Clear();
        }
    }
}