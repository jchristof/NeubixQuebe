using InGame.State.ChallengeModeStates;

namespace InGame {
    public class InGameFsm : FSM<InGameFsm> {
        public InGameFsm(GameBehavior gameBehavior, float levelTime) {
            this.gameBehavior = gameBehavior;
            SetState(typeof(StartGameState), levelTime);
        }

        public readonly GameBehavior gameBehavior;
        public float levelCompletionTime;

        public bool CanPause() {
            return IsInAny(typeof(GamePlay));
        }

        public void RunSuccessAnimation() {
            SetState(typeof(SuccessTransition), false);
        }
    }
}