using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DefaultNamespace {
    public class GameModeTwoColor : MonoBehaviour, TileField, GameMode {
        public List<Material> challengeRowColors;
        public int[] layout = new int[18];
        public CubePool cubePool;
        public InGameMenu inGameMenu;
        private List<GameObject> cubes = new List<GameObject>();
        private CountdownTimer countdownTimer = new CountdownTimer();

        public void Update() {
            countdownTimer.Update((int)(Time.deltaTime * 1000));
            inGameMenu.time.GetComponent<TextMeshProUGUI>().text = countdownTimer.ToString();
            if (Input.GetKeyUp(KeyCode.Alpha1)) {
                var textEnabled = cubes[0].GetComponentInChildren<TileScript>().text.enabled;
                foreach (var cube in cubes) {
                    cube.GetComponentInChildren<TileScript>().text.enabled = !textEnabled;
                }
            }
        }

        public List<GameObject> GetTiles() {
            if (cubes.Any())
                return cubes;

            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePool.cubeGrid[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.localScale = Vector3.one * .9f;
                cube.transform.rotation = new Quaternion(0,0,0,0);
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
                cubes[i].GetComponent<TileScript>().text.enabled = false;
            }

            return cubes;
        }

        public void OnEnable() {
            
            countdownTimer.Set((int)TimeSpan.FromMinutes(2).TotalMilliseconds);
            countdownTimer.SetOnEnd(() => {
                
            });
            countdownTimer.Start();
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