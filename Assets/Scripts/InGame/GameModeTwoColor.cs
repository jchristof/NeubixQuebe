using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace InGame {
    public class GameModeTwoColor : MonoBehaviour, TileField, GameMode {
        public List<Material> challengeRowColors;
        public Material cubeColor0;
        public Material cubeColor1;

        public Material successMaterial;

        public CubePool cubePool;
        public InGameMenu inGameMenu;
        public GameController gameController;
        private List<GameObject> cubes = new List<GameObject>();

        private InGameState.InGameFSM inGameState;
        //private int[] twoColorLayout = {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        private int[] threeColorLayout = {0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1};

        public void Start() {
            inGameState = new InGameState.InGameFSM(this);
        }

        public void Update() {
            if (Input.GetKeyUp(KeyCode.Alpha2))
                inGameState.RunSuccessAnimation();

            inGameState?.Update();
        }

        public List<GameObject> GetTiles() {
            if (cubes.Any())
                return cubes;

            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePool.cubeGrid[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.localScale = Vector3.one * .9f;
                cube.transform.rotation = new Quaternion(0, 0, 0, 0);
                cube.GetComponentInChildren<Text>().text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.SetActive(true);
                cubes.Add(cube);
            }

            int[] scrambled = (int[])threeColorLayout.Clone();
            for (int i = 0; i < scrambled.Length; i++) {
                int rnd = Random.Range(0, scrambled.Length);
                int temp = scrambled[rnd];
                scrambled[rnd] = scrambled[i];
                scrambled[i] = temp;
            }

            for (int i = 0; i < cubes.Count; i++) {
                cubes[i].GetComponent<Renderer>().material = challengeRowColors[scrambled[i]];
                cubes[i].GetComponent<TileScript>().Identifier = scrambled[i];
                cubes[i].GetComponent<TileScript>().image.enabled = false;
                cubes[i].GetComponent<TileScript>().text.enabled = false;
            }

            return cubes;
        }

        public void OnEnable() {
            inGameState = new InGameState.InGameFSM(this);
        }

        public void OnDisable() {
            cubes.Clear();
        }

        public bool CheckSolved(int distance) {
            for (int i = 0; i < 18; i++) {
                var tileScript = cubes[i].GetComponent<TileScript>();
                if (tileScript.Identifier != threeColorLayout[i])
                    return false;
            }

            inGameState.RunSuccessAnimation();
            return true;
        }
    }
}