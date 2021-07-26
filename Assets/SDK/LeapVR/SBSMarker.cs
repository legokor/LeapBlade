using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Marks a camera (other than the main camera) for <see cref="SBS"/> handling.
    /// </summary>
    [AddComponentMenu("Leap VR/Side-by-Side 3D Marker")]
    [RequireComponent(typeof(Camera))]
    public class SBSMarker : MonoBehaviour {
        Camera Cam;

        void Awake() {
            SBS.AddCamera(Cam = GetComponent<Camera>());
        }

        void OnDestroy() {
            SBS.RemoveCamera(Cam);
        }
    }
}