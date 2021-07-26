using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Copy a hand's rotation to an object.
    /// </summary>
    [AddComponentMenu("Leap VR/Interaction/Rotate With Hand")]
    public class RotateWithHand : MonoBehaviour {
        public int HandID;

        void Update() {
            Quaternion? HandRotation = LeapMotion.Instance.HandRotation(HandID);
            if (HandRotation.HasValue)
                transform.localRotation = HandRotation.Value;
        }
    }
}