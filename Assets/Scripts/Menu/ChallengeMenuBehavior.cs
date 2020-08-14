using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Menu;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefaultNamespace {
    public class ChallengeMenuBehavior : MonoBehaviour {
        public List<Material> challengeRowColors;
        public Sprite crown;
        public CubePool cubePool;
        public List<GameObject> cubes = new List<GameObject>();
        public GameController gameController;
        
        [CanBeNull] private ChallengeMenuState.ChallengeMenuFSM fsm;
        void Update() {
            fsm?.Update();
        }

        private void OnMouseOver() {
            if (!fsm?.AllowSelection == true)
                return;
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] raycastHits = new RaycastHit[5];
            Physics.RaycastNonAlloc(ray, raycastHits);

            GameObject hoverCube = null;
            foreach (var hit in raycastHits) {
                var t = hit.transform;
                if (t == null) break;
                var component = t.GetComponent(typeof(TileScript));
                if (component == null) continue;
                hoverCube = hit.transform.gameObject;

                break;
            }

            if (hoverCube == null)
                return;

            foreach (var cube in cubes) {
                cube.GetComponent<Animator>().SetBool("ToAnimate", cube == hoverCube);
            }
        }

        private void OnMouseExit() {
            if (fsm?.AllowSelection == false)
                return;
            
            foreach (var cube in cubes) {
                cube.GetComponent<Animator>().SetBool("ToAnimate", false);
            }
        }

        private void OnMouseUp() {
            if (fsm?.AllowSelection == false)
                return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] raycastHits = new RaycastHit[5];
            Physics.RaycastNonAlloc(ray, raycastHits);

            foreach (var hit in raycastHits) {
                var t = hit.transform;
                if (t == null) break;
                var tile = t.GetComponent(typeof(TileScript));
                if (tile == null) continue;

                OnMouseExit();
                fsm?.ChallengeSelected(tile.gameObject);
                
                break;
            }
        }

        public void EnableInput() {
            fsm = new ChallengeMenuState.ChallengeMenuFSM(this);
        }
        
        public void OnEnable() {
            cubes = GetTiles();
        }

        public void OnDisable() {
            cubes.Clear();
        }

        public List<GameObject> GetTiles() {
            if (cubes.Any())
                return cubes;

            ConfigureCubes();
            return cubes;
        }

        void ConfigureCubes() {
            var savedProgress = gameController.savedProgress.challengeProgress.challenges;
            for (var i = 17; i >= 0; i--) {
                GameObject cube = cubePool.cubeGrid[i];
                cube.SetActive(true);
                cube.transform.position = new Vector3(2 - (i % 3), i / 3, 0);
                var material = challengeRowColors[i / 3];
                if (savedProgress[17 - i].complete) {
                    var newMaterial = new Material(material);
                    var color = newMaterial.GetColor("Color_E1158FD4");
                    var intensity = .1f;
                    newMaterial.SetColor("Color_E1158FD4", new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a));
                    cube.GetComponent<TileScript>().silhouettePlane.SetActive(false);
                    cube.GetComponent<Renderer>().material = newMaterial;
                }
                else {
                    cube.GetComponent<Renderer>().material = material;

                    TileScript tileScript = cube.GetComponent<TileScript>();
                    GameObject silhouettePlane = tileScript.silhouettePlane;
                    silhouettePlane.SetActive(true);
                    Material silhouetteMaterial = silhouettePlane.GetComponent<Renderer>().material;
                    silhouetteMaterial.SetColor("Color_E1158FD4",new Color(0f, 0f, 0f, 1.0f));
                }

                cube.GetComponent<TileScript>().text.color = Color.white;
                cube.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.GetComponent<TileScript>().text.text = (18 - i).ToString();
                cube.name = (18 - i).ToString();
                cube.GetComponent<TileScript>().image.enabled = false;
                cube.GetComponent<TileScript>().text.enabled = true;
                cubes.Add(cube);
            }

            cubes.Last().GetComponent<TileScript>().image.enabled = true;
            cubes.Last().GetComponent<TileScript>().image.sprite = crown;
            cubes.Last().GetComponent<TileScript>().text.enabled = false;
        }
    }
}