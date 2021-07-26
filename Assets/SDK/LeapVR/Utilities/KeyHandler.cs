using UnityEngine;
using UnityEngine.Events;

namespace LeapVR {
    /// <summary>
    /// Do something when a key is pressed.
    /// </summary>
    [AddComponentMenu("Leap VR/Utilities/Key Handler")]
    class KeyHandler : MonoBehaviour {
        public KeyCode key = KeyCode.None;
        public UnityEvent onKey;

        void Update() {
            if (Input.GetKeyDown(key))
                onKey?.Invoke();
        }
    }
}