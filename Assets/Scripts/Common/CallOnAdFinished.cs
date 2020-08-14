using System;
using UnityEngine.Advertisements;

namespace Common {

    public class CallOnAdFinished : IUnityAdsListener {

        public CallOnAdFinished(Action<float> onAddFinishedAction) {
            this.onAddFinishedAction = onAddFinishedAction;
        }
        
        Action<float> onAddFinishedAction;

        public void OnAdFinished() {
            if (onAddFinishedAction != null) {
                onAddFinishedAction(1f);
                onAddFinishedAction = null;

                Advertisement.RemoveListener(this);
            }
        }
    
        public void OnUnityAdsReady(string placementId) { }

        public void OnUnityAdsDidError(string message) {
            OnAdFinished();
        }

        public void OnUnityAdsDidStart(string placementId) { }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
            OnAdFinished();
        }

    }

}