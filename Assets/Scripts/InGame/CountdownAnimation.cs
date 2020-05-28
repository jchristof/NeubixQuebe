using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame {
    public class CountdownAnimation {
        public CountdownAnimation(List<GameObject> cubes, Material cubeColor0, Material cubeColor1, MonoBehaviour context) {
            this.cubes = cubes;
            this.cubeColor0 = cubeColor0;
            this.cubeColor1 = cubeColor1;
            this.context = context;
        }

        private List<GameObject> cubes;
        private Material cubeColor0;
        private Material cubeColor1;
        private MonoBehaviour context;
        private Action onEnd;
        
        public void Start(Action onEnd) {
            this.onEnd = onEnd;
            
            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubes[i];
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                cube.GetComponent<Renderer>().material = cubeColor0;
                cube.transform.rotation = new Quaternion(0, 0, 0, 0);
                cubes[i].GetComponent<TileScript>().image.enabled = false;
                cubes[i].GetComponent<TileScript>().text.enabled = false;
                cube.SetActive(true);
                cubes.Add(cube);
            }

            context.StartCoroutine(CountDownAnimation());
        }
        
        private IEnumerator CountDownAnimation() {
            var count = 2;
            while (count > -1) {
                for (int i = 0; i < 18; i++) {
                    var index = 17 - i;
                    var cube = cubes[index];
                    if (numbers[count, i] == 1) {
                        cube.GetComponent<Renderer>().material = cubeColor0;
                    }
                    else {
                        cube.transform.SetParent(null);
                        cube.GetComponent<Renderer>().material = cubeColor1;
                    }
                }

                count--;

                yield return new WaitForSeconds(1f);
            }
            onEnd?.Invoke();
        }
        
        private readonly int[,] numbers = new int[3, 18] {
            {
                1, 1, 0,
                0, 1, 0,
                0, 1, 0,
                0, 1, 0,
                0, 1, 0,
                1, 1, 1
            }, {
                1, 1, 1,
                0, 0, 1,
                0, 1, 1,
                1, 0, 0,
                1, 0, 0,
                1, 1, 1
            }, {
                1, 1, 1,
                0, 0, 1,
                1, 1, 1,
                0, 0, 1,
                0, 0, 1,
                1, 1, 1
            },
        };
    }
    
}
