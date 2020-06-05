using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace InGame {
    public class InGameFsm : FSM<InGameFsm> {
        public InGameFsm(GameBehavior gameMode, float levelTime) {
            this.gameMode = gameMode;
            this.levelTime = levelTime;
            SetState(typeof(StartGameState));
            TimeSpan t = TimeSpan.FromMinutes(levelTime);
            gameMode.inGameMenu.time.GetComponent<TextMeshProUGUI>().text =
                string.Format("{0:D1}:{1:D2}",
                    t.Minutes,
                    t.Seconds);
        }

        public readonly GameBehavior gameMode;
        public readonly float levelTime;

        public float levelCompletionTime;

        public bool CanPause() {
            return IsInAny(new[] {typeof(GamePlay)});
        }

        public void RunSuccessAnimation() {
            SetState(typeof(SuccessTransition), false);
        }
    }

    class StartGameState : State<InGameFsm> {
        public StartGameState(InGameFsm fsm) : base(fsm) { }
        private CountdownAnimation countDownAnimation;

        public override void Pre(object args = null) {
            base.Pre(args);
            fsm.gameMode.moveScript?.Disable();

            countDownAnimation = new CountdownAnimation(fsm.gameMode.cubePool.cubeGrid, fsm.gameMode.cubeColor0,
                fsm.gameMode.cubeColor1, fsm.gameMode);
            countDownAnimation.Start(() => { fsm.SetState(typeof(GamePlay)); });
        }
    }

    class GamePlay : State<InGameFsm> {
        public GamePlay(InGameFsm fsm) : base(fsm) { }
        private CountdownTimer countdownTimer = new CountdownTimer();

        public override void Pre(object args = null) {
            base.Pre(args);

            countdownTimer.Set((int) TimeSpan.FromMinutes(fsm.levelTime).TotalMilliseconds);
            countdownTimer.SetOnEnd(() => { fsm.SetState(typeof(SuccessTransition), true); });
            countdownTimer.Start();

            fsm.gameMode.moveScript = new MoveScript(fsm.gameMode.transform, fsm.gameMode.cubePool.cubesPool,
                fsm.gameMode.game.GetGameTiles());
            fsm.gameMode.moveScript.Enable();
        }

        public override void Update() {
            base.Update();
            countdownTimer.Update((int) (Time.deltaTime * 1000));
            fsm.gameMode.inGameMenu.time.GetComponent<TextMeshProUGUI>().text = countdownTimer.ToString();
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

            fsm.gameMode.moveScript.Stop();
            fsm.gameMode.StartCoroutine(RunTranistionAnimation());
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

        IEnumerator RunTranistionAnimation() {
            int row = 5;

            while (row > -1) {
                foreach (var cube in fsm.gameMode.cubePool.cubeGrid) {
                    if (cube.transform.position.y == row) {
                        cube.GetComponent<Renderer>().material = new Material(fsm.gameMode.successMaterial);
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
                fsm.gameMode.gameController.FailedAnimDone();
            }
            else {
                fsm.gameMode.gameController.SuccessAnimDone();
                fsm.gameMode.gameController.GameModeWon(fsm.levelCompletionTime);
            }
        }
    }
}