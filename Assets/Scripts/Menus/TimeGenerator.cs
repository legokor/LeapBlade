using UnityEngine;

namespace Menus {
    public class TimeGenerator : MonoBehaviour {
        public float DeltaTime { get; private set; }

        float lastTime;

        void Start() => lastTime = Time.unscaledTime;

        void Update() {
            float t = Time.unscaledTime;
            DeltaTime = t - lastTime;
            lastTime = t;
        }
    }
}