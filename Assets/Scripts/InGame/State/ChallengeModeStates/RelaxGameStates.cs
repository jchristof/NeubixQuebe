using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGame.State.ChallengeModeStates.Relax {
    class RelaxGameState : State<InGameFsm> {
        public RelaxGameState(InGameFsm fsm) : base(fsm) { }
    
        public override void Pre(object args = null) {
            base.Pre(args);
            fsm.gameBehavior.moveScript = new MoveScript(fsm.gameBehavior.transform,
                fsm.gameBehavior.cubePool.cubesPool, fsm.gameBehavior.game.GetGameTiles());
            fsm.gameBehavior.moveScript?.Disable();
            fsm.SetState(typeof(VisualTransitionIn));
            fsm.gameBehavior.inGameMenu.HideTimer();
            fsm.gameBehavior.inGameMenu.HideLevelCounter();
        }
    }


    class VisualTransitionIn : State<InGameFsm> {
        public VisualTransitionIn(InGameFsm fsm) : base(fsm) { }
        private List<GameObject> cubes = new List<GameObject>();
        private float duration = .5f;

        public override void Pre(object args) {
            base.Pre(args);

            var c = fsm.gameBehavior.game.GetGameTiles();
            foreach (var cube in c) {
                var color = cube.GetComponent<Renderer>().material.color;
                cube.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0);
            }

            fsm.gameBehavior.cubePool.ShowAll();
            fsm.gameBehavior.StartCoroutine(RunTransitionAnimation());
        }

        public override void Update() {
            base.Update();
            foreach (var cube in cubes) {
                var tileScript = cube.GetComponent<TileScript>();
                var color = cube.GetComponent<Renderer>().material.color;
                var startColor = new Color(color.r, color.g, color.b, 0);
                var endColor = new Color(color.r, color.g, color.b, 1);
                cube.GetComponent<Renderer>().material.color =
                    Color.Lerp(startColor, endColor, tileScript.fadeTime);
                tileScript.fadeTime += Time.deltaTime / duration;
            }
        }

        IEnumerator RunTransitionAnimation() {
            int row = 0;

            while (row < 6) {
                foreach (var cube in fsm.gameBehavior.cubePool.cubeGrid) {
                    if (cube.transform.position.y == row) {
                        cube.GetComponent<TileScript>().fadeTime = 0f;
                        cubes.Add(cube);
                    }
                }

                row++;
                yield return new WaitForSeconds(.25f);
            }

            yield return new WaitForSeconds(.25f);

            cubes.Clear();
            fsm.SetState(typeof(RelaxGamePlay));
        }
    }
    
    class RelaxGamePlay : State<InGameFsm> {
        public RelaxGamePlay(InGameFsm fsm) : base(fsm) { }

        public override void Pre(object args = null) {
            base.Pre(args);
            
            fsm.gameBehavior.moveScript.Enable();
            fsm.gameBehavior.inGameMenu.ShowPause();
        }

        public override void Update() {
            base.Update();
        }

        public override void Post() {
            base.Post();
            fsm.gameBehavior.inGameMenu.HidePause();
        }
    }
    
    class VisualTransitionOut : State<InGameFsm> {
        public VisualTransitionOut(InGameFsm fsm) : base(fsm) { }
        private Color startColor = Color.green;
        private Color endColor = new Color(0f, 1f, 0f, 0f);
        private List<GameObject> cubes = new List<GameObject>();
        private float duration = .5f;

        public override void Pre(object args) {
            base.Pre(args);

            fsm.gameBehavior.moveScript.Disable();
            fsm.gameBehavior.StartCoroutine(RunTransitionAnimation());
            fsm.gameBehavior.inGameMenu.HidePause();
        }

        public override void Update() {
            base.Update();
            foreach (var cube in cubes) {
                var tilescript = cube.GetComponent<TileScript>();
                cube.GetComponent<Renderer>().material.color =
                    Color.Lerp(startColor, endColor, tilescript.fadeTime);
                tilescript.fadeTime += Time.deltaTime / duration;
            }
        }

        IEnumerator RunTransitionAnimation() {
            int row = 5;

            while (row > -1) {
                foreach (var cube in fsm.gameBehavior.cubePool.cubeGrid) {
                    if (cube.transform.position.y == row) {
                        cube.GetComponent<Renderer>().material = new Material(fsm.gameBehavior.successMaterial);
                        cube.GetComponent<TileScript>().fadeTime = 0f;
                        cubes.Add(cube);
                    }
                }

                row--;
                yield return new WaitForSeconds(.25f);
            }

            yield return new WaitForSeconds(.25f);

            cubes.Clear();
            fsm.SetState(typeof(VisualTransitionIn));
            fsm.gameBehavior.game.Reset();
        }
    }
}