namespace InGame.State.ChallengeModeStates {
    public class EndlessModeGameStates {
        class StartGameState : State<InGameFsm> {
            public StartGameState(InGameFsm fsm) : base(fsm) { }

            public override void Pre(object args = null) {
                base.Pre(args);
                fsm.gameBehavior.moveScript?.Disable();
            }
        }
    }
}