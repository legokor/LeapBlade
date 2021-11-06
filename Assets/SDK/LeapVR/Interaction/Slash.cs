using UnityEngine;
using UnityEngine.Events;

namespace LeapVR {
    /// <summary>
    /// Events on fast directional motions.
    /// </summary>
    [AddComponentMenu("Leap VR/Interaction/Slash")]
    public class Slash : MonoBehaviour {
        [Tooltip("The ID of the used Leap Motion controller, 0-indexed in connection order.")]
        public int deviceID = 0;
        [Tooltip("Use the first available hand the controller detects.")]
        public bool anyHand;
        [Tooltip("Use the first last hand. The first right hand will be used if false.")]
        public bool leftHand;
        [Tooltip("Hand movement velocity in m/s to trigger a slash.")]
        public float requiredSpeed = 3;
        [Tooltip("Time after a slash before the player could slash again.")]
        public float timeout = .1f;
        public UnityEvent forward, backwards, left, right, up, down;

        float remaining, speed;
        LeapMotion controller;
        Vector3? palmPos;

        void Detect() {
            if (controller.Devices <= deviceID)
                return;
            int hand;
            if (anyHand)
                hand = controller.GetHandCount(deviceID) > 0 ? 0 : -1;
            else if (leftHand)
                hand = controller.FirstLeftHand(deviceID);
            else
                hand = controller.FirstRightHand(deviceID);
            if (hand != -1)
                palmPos = controller.PalmPosition(hand, deviceID);
        }

        void Start() {
            controller = LeapMotion.Instance;
            Detect();
        }

        void Update() {
            if ((remaining -= Time.unscaledDeltaTime) > 0)
                return;
            Vector3? lastPalm = palmPos;
            Detect();
            if (lastPalm.HasValue && palmPos.HasValue) {
                Vector3 delta = palmPos.Value - lastPalm.Value;
                float oldSpeed = speed;
                speed = Mathf.Lerp(speed, delta.magnitude, 200 * Time.deltaTime);
                if (oldSpeed < requiredSpeed && speed >= requiredSpeed) {
                    remaining = timeout;
                    Vector3 abs = new Vector3(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
                    float max = Mathf.Max(abs.x, abs.y, abs.z);
                    if (max == abs.x)
                        if (delta.x < 0)
                            left.Invoke();
                        else
                            right.Invoke();
                    else if (max == abs.y)
                        if (delta.y < 0)
                            down.Invoke();
                        else
                            up.Invoke();
                    else if (delta.z < 0)
                        forward.Invoke();
                    else
                        backwards.Invoke();
                }
            }
        }
    }
}