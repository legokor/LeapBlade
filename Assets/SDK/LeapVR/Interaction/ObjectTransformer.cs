using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Move and rotate an object with hand gestures.
    /// </summary>
    [AddComponentMenu("Leap VR/Interaction/Object Transformer")]
    public class ObjectTransformer : MonoBehaviour {
        public Transform Target;

        [Header("Movement")]
        public bool EnableMovement = true;
        [Tooltip("Sensitivity for movement (single hand grab).")]
        public float MovementSensitivity = .1f;

        [Header("Rotation")]
        public bool EnableRotation = true;
        [Tooltip("Sensitivity for rotation (dual hand grab).")]
        public float RotationSensitivity = .5f;

        [Header("Scaling")]
        public bool EnableScaling = true;
        [Tooltip("Sensitivity for scaling (dual hand pinch).")]
        public float ScalingSensitivity = .005f;

        public void ToggleMovement() {
            EnableMovement = !EnableMovement;
        }

        public void ToggleRotation() {
            EnableRotation = !EnableRotation;
        }

        public void ToggleScaling() {
            EnableScaling = !EnableScaling;
        }

        void Start() {
            MinScale = transform.localScale * .0001f;
        }

        void Update() {
            if (LeapMouse.Instance.Action()) {
                int LeftID = LeapMotion.Instance.FirstLeftHand(), RightID = LeapMotion.Instance.FirstRightHand();
                if (LeftID != -1 && RightID != -1 && LeapMotion.Instance.ExtendedFingers(LeftID) == 0 && LeapMotion.Instance.ExtendedFingers(RightID) == 0) { // Rotate
                    Vector3 Left = LeapMotion.Instance.PalmPosition(LeftID), Right = LeapMotion.Instance.PalmPosition(RightID), NewRotation = Right - Left;
                    if (PrevRotation != Vector3.up) {
                        Transform Cam = Camera.main.transform;
                        Vector3 Delta = (NewRotation - PrevRotation) * RotationSensitivity;
                        if (EnableRotation)
                            Target.rotation = Quaternion.Euler(Cam.right * (Right.y - PrevHeight - Delta.y)) * Quaternion.Euler(Cam.forward * Delta.y) *
                                Quaternion.Euler(Cam.up * Delta.z) * Target.rotation;
                        if (EnableScaling)
                            Target.localScale = Vector3.Max(Target.localScale * (1 + Delta.x * ScalingSensitivity), MinScale);
                    }
                    PrevHeight = Right.y;
                    PrevRotation = NewRotation;
                    Target.position = MovementStart;
                } else { // Move
                    Vector3 PalmPos = LeapMotion.Instance.PalmPosition();
                    PalmPos = new Vector3(PalmPos.x, PalmPos.y, -PalmPos.z);
                    if (EnableMovement && PrevPosition != Vector3.up)
                        Target.position += Camera.main.transform.rotation * ((PalmPos - PrevPosition) * MovementSensitivity);
                    PrevPosition = PalmPos;
                }
            } else {
                PrevPosition = Vector3.up;
                PrevRotation = Vector3.up;
                MovementStart = Target.position;
            }
        }

        /// <summary>
        /// Previous hand height.
        /// </summary>
        float PrevHeight;

        /// <summary>
        /// Starting position of a movement, to cancel the movement if the gesture is a rotation.
        /// </summary>
        Vector3 MovementStart;

        /// <summary>
        /// Previous hand position for movement.
        /// </summary>
        Vector3 PrevPosition;

        /// <summary>
        /// Previous hand difference vector for rotation.
        /// </summary>
        Vector3 PrevRotation;

        /// <summary>
        /// Minimal possible scaling.
        /// </summary>
        Vector3 MinScale;
    }
}