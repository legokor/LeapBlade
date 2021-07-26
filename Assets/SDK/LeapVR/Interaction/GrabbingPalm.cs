using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// A palm able to pick up objects.
    /// </summary>
    [AddComponentMenu("Leap VR/Interaction/Grabbing Palm")]
    public class GrabbingPalm : MonoBehaviour {
        public int HandID;

        Pickup LastGrabbed;

        void Update() {
            int ExtendedFingers = LeapMotion.Instance.ExtendedFingers(HandID);
            if (ExtendedFingers < 3) {
                Pickup Closest = Pickup.ClosestPickup(transform.position);
                if (Closest && Closest.InGrabRange(transform.position))
                    (LastGrabbed = Closest).GetComponent<Collider>().isTrigger = true;
            } else if (LastGrabbed && ExtendedFingers > 3) {
                LastGrabbed.GetComponent<Collider>().isTrigger = false;
                LastGrabbed = null;
            }
            if (LastGrabbed) {
                LastGrabbed.transform.position = transform.position - transform.up * transform.lossyScale.x;
                LastGrabbed.transform.rotation = transform.rotation;
            }
        }
    }
}