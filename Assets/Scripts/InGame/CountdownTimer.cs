using System;

namespace DefaultNamespace {
    public class CountdownTimer {
        private int currentTime;
        private bool running;
        private Action onEnd;
        
        public void Set(int milliseconds) {
            currentTime = milliseconds;
        }

        public int Get() {
            return currentTime;
        }

        public void Start() {
            running = true;
        }

        public void Stop() {
            running = false;
            onEnd = null;
        }

        public void Update(int deltaMilliseconds) {
            var newTime = currentTime - deltaMilliseconds;
            if (newTime < 0) {
                running = false;
                currentTime = 0;
                onEnd?.Invoke();
                onEnd = null;
            }
            else {
                currentTime = newTime;
            }
        }

        override public string ToString() {
            TimeSpan t = TimeSpan.FromMilliseconds(currentTime);
            return string.Format("{0:D1}:{1:D2}",
                t.Minutes, 
                t.Seconds);
        }

        public void SetOnEnd(Action endAction) {
            onEnd = endAction;
        }
    }
}