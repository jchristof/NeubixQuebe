using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace InGame.State.ChallengeModeStates {
    class StartGameState : State<InGameFsm> {
        public StartGameState(InGameFsm fsm) : base(fsm) { }
        private CountdownAnimation countDownAnimation;

        public override void Pre(object args = null) {
            base.Pre(args);
            fsm.gameBehavior.moveScript?.Disable();
            var levelTime = (float) args;
            
            TimeSpan t = TimeSpan.FromMinutes(levelTime);
            fsm.gameBehavior.inGameMenu.time.GetComponent<TextMeshProUGUI>().text =
                $"{t.Minutes:D1}:{t.Seconds:D2}";
            
            countDownAnimation = new CountdownAnimation(fsm.gameBehavior.cubePool.cubeGrid, fsm.gameBehavior.cubeColor0,
                fsm.gameBehavior.cubeColor1, fsm.gameBehavior);
            countDownAnimation.Start(() => { fsm.SetState(typeof(GamePlay), levelTime); });
        }
    }

    class GamePlay : State<InGameFsm> {
        public GamePlay(InGameFsm fsm) : base(fsm) { }
        private CountdownTimer countdownTimer = new CountdownTimer();

        public override void Pre(object args = null) {
            base.Pre(args);

            countdownTimer.Set((int) TimeSpan.FromMinutes((float)args).TotalMilliseconds);
            countdownTimer.SetOnEnd(() => { fsm.SetState(typeof(SuccessTransition), true); });
            countdownTimer.Start();

            fsm.gameBehavior.moveScript = new MoveScript(fsm.gameBehavior.transform, fsm.gameBehavior.cubePool.cubesPool,
                fsm.gameBehavior.game.GetGameTiles());
            fsm.gameBehavior.moveScript.Enable();
        }

        public override void Update() {
            base.Update();
            countdownTimer.Update((int) (Time.deltaTime * 1000));
            fsm.gameBehavior.inGameMenu.time.GetComponent<TextMeshProUGUI>().text = countdownTimer.ToString();
        }

        public override void Post() {
            base.Post();
            countdownTimer.Stop();
            fsm.levelCompletionTime = countdownTimer.Get();
        }
    }

    class SuccessTransition : State<InGameFsm> {
        public SuccessTransition(InGameFsm fsm) : base(fsm) { }
        private Color startColor = Color.green;
        private Color endColor = new Color(0f, 1f, 0f, 0f);
        private List<GameObject> cubes = new List<GameObject>();
        private float duration = .5f;
        private bool failed;

        public override void Pre(object args) {
            base.Pre(args);
            failed = (bool) args;

            if (failed) {
                startColor = Color.red;
                endColor = new Color(1f, 0f, 0f, 0f);
            }

            fsm.gameBehavior.moveScript.Stop();
            fsm.gameBehavior.StartCoroutine(RunTransitionAnimation());
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
            if (failed) {
                fsm.gameBehavior.gameController.FailedAnimDone();
            }
            else {
                fsm.gameBehavior.gameController.SuccessAnimDone();
                fsm.gameBehavior.gameController.GameModeWon(fsm.levelCompletionTime);
            }
        }
    }
}