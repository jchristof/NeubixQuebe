using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace InGame.State.ChallengeModeStates {
    class EndlessGameStart : State<InGameFsm> {
        public EndlessGameStart(InGameFsm fsm) : base(fsm) { }
    
        public override void Pre(object args = null) {
            base.Pre(args);
            
            fsm.levelTime = (float) args;
            
            TimeSpan t = TimeSpan.FromMinutes(fsm.levelTime);
            fsm.gameBehavior.inGameMenu.time.GetComponent<TextMeshProUGUI>().text =
                $"{t.Minutes:D1}:{t.Seconds:D2}";
            
            fsm.gameBehavior.moveScript = new MoveScript(fsm.gameBehavior.transform,
                fsm.gameBehavior.cubePool.cubesPool, fsm.gameBehavior.game.GetGameTiles());
            fsm.gameBehavior.moveScript?.Disable();
            fsm.SetState(typeof(VisualTransitionIn));
            
            fsm.gameBehavior.inGameMenu.ShowPause();
            //fsm.gameBehavior.inGameMenu.HideLevelCounter();
            fsm.gameBehavior.inGameMenu.ShowTimer();
            //fsm.gameBehavior.inGameMenu.ShowMoveCounter();
            fsm.gameBehavior.inGameMenu.SetChallengeNumber(fsm.levelCounter.ToString());
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
            fsm.SetState(typeof(EndlessGamePlay));
        }
    }
    
    class EndlessGamePlay : State<InGameFsm> {
        public EndlessGamePlay(InGameFsm fsm) : base(fsm) { }
        private CountdownTimer countdownTimer = new CountdownTimer();
        
        public override void Pre(object args = null) {
            base.Pre(args);
            countdownTimer.Set((int)(fsm.levelTime * 1000 * 60));
            countdownTimer.SetOnEnd(() => {
                fsm.levelTime = 0;
                fsm.SetState(typeof(VisualTransitionOut), true);
            });
            countdownTimer.Start();
            
            fsm.gameBehavior.moveScript.Enable();
            fsm.gameBehavior.inGameMenu.ShowPause();
        }

        public override void Update() {
            base.Update();
            countdownTimer.Update((int) (Time.deltaTime * 1000));
            fsm.gameBehavior.inGameMenu.time.GetComponent<TextMeshProUGUI>().text = countdownTimer.ToString();
        }

        public override void Post() {
            base.Post();
            fsm.gameBehavior.inGameMenu.HidePause();
            fsm.levelTime = (float)countdownTimer.Get() / (float)(1000 * 60);
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
            if (fsm.levelTime > 0f) {
                fsm.SetState(typeof(VisualTransitionIn));
                fsm.gameBehavior.game.Reset();
                fsm.levelCounter++;
                fsm.gameBehavior.inGameMenu.SetChallengeNumber(fsm.levelCounter.ToString());
                fsm.levelTime += (float)5 / 60;
            }
            else {
                fsm.gameBehavior.gameController.EnlessGameTimeUp();
            }
        }
    }
}