using System;
using InGame.State.ChallengeModeStates;
using TMPro;

namespace InGame {
    public class InGameFsm : FSM<InGameFsm> {
        public InGameFsm(GameBehavior gameBehavior, float levelTime) {
            this.gameBehavior = gameBehavior;
            this.levelTime = levelTime;
            SetState(typeof(StartGameState));
            TimeSpan t = TimeSpan.FromMinutes(levelTime);
            gameBehavior.inGameMenu.time.GetComponent<TextMeshProUGUI>().text =
                $"{t.Minutes:D1}:{t.Seconds:D2}";
        }

        public readonly GameBehavior gameBehavior;
        public readonly float levelTime;

        public float levelCompletionTime;

        public bool CanPause() {
            return IsInAny(typeof(GamePlay));
        }

        public void RunSuccessAnimation() {
            SetState(typeof(SuccessTransition), false);
        }
    }
}