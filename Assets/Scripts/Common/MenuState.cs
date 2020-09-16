using System.Collections;
using UnityEngine;

namespace Common {

    public class MenuFsm : FSM<MenuFsm> {

        public MenuFsm(GameController gameController) {
            this.gameController = gameController;
        }

        public readonly GameController gameController;

        public void OnClickSplash() {
            GetState<SplashIdleState>()?.StartSplashFade();
        }

    }

    public class SplashIdleState : State<MenuFsm> {

        private float startTime = Time.time;

        public SplashIdleState(MenuFsm fsm) : base(fsm) { }

        public override void Update() {
            if (Time.time - startTime > 3)
                fsm.SetState(typeof(SplashMenuFadeState));
        }

        public void StartSplashFade() {
            fsm.SetState(typeof(SplashMenuFadeState));
        }

    }

    public class SplashMenuFadeState : State<MenuFsm> {

        public SplashMenuFadeState(MenuFsm fsm) : base(fsm) { }

        public override void Pre(object args = null) {
            fsm.gameController.challengeMenu.Menu.SetActive(true);
            fsm.gameController.StartCoroutine(FadeOutSplash());
        }

        IEnumerator FadeOutSplash() {
            float splashAlpha = 1f;
            float challengeAlpha = 0f;
            while (splashAlpha > 0) {
                splashAlpha -= .2f;
                challengeAlpha += .2f;
                fsm.gameController.splashMenu.canvasGroup.alpha = splashAlpha;
                fsm.gameController.challengeMenu.canvasGroup.alpha = challengeAlpha;
                yield return new WaitForSeconds(.1f);
            }

            fsm.gameController.splashMenu.gameObject.SetActive(false);
            fsm.gameController.challengeMenu.challengeMenuBehavior.EnableInput();
            fsm.gameController.menuState = null;
        }

    }

}