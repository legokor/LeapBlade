using System.Collections.Generic;
using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Grabbable object marker.
    /// </summary>
    [AddComponentMenu("Leap VR/Interaction/Pickup")]
    public class Pickup : MonoBehaviour {
        public float GrabRange = .3f;
        public Vector3 GrabOffset;

        public bool InGrabRange(Vector3 Position) {
            return (transform.position + GrabOffset - Position).sqrMagnitude < GrabRange * GrabRange;
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + GrabOffset, GrabRange);
        }

        static List<Pickup> Pickups = new List<Pickup>();

        public static Pickup ClosestPickup(Vector3 Position) {
            float MinDistance = float.PositiveInfinity;
            Pickup Found = null;
            foreach (Pickup p in Pickups) {
                float Distance = (p.transform.position + p.GrabOffset - Position).sqrMagnitude;
                if (MinDistance > Distance) {
                    MinDistance = Distance;
                    Found = p;
                }
            }
            return Found;
        }

        void OnEnable() {
            Pickups.Add(this);
        }

        void OnDisable() {
            Pickups.Remove(this);
        }
    }
}