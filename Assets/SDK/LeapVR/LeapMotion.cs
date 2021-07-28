using Leap;
using System.Collections.Generic;
using UnityEngine;

namespace LeapVR {
    /// <summary>
    /// Leap Motion handling simplified.
    /// </summary>
    [DefaultExecutionOrder(int.MinValue + 1000)]
    [AddComponentMenu("Leap VR/Leap Motion")]
    public class LeapMotion : Singleton<LeapMotion> {
        public bool headMounted = true;
        [Tooltip("Reconnect after this many passed seconds.")]
        public float connectionTimeout = 5;
        [Tooltip("Lower values of hand detection bounds.")]
        public Vector3 leapLowerBounds = new Vector3(-200, 100, -112.5f);
        [Tooltip("Upper values of hand detection bounds.")]
        public Vector3 leapUpperBounds = new Vector3(200, 300, 112.5f);

        /// <summary>
        /// The last frame in the Leap Motion SDK's format.
        /// </summary>
        public Frame RawFrame => lastFrame;

        /// <summary>
        /// Connected Leap Motion device.
        /// </summary>
        Controller device;

        /// <summary>
        /// First time a connection was tried to the controller;
        /// </summary>
        float started;

        /// <summary>
        /// The Leap Motion frame just before the game's frame update.
        /// </summary>
        Frame lastFrame = new Frame();

        /// <summary>
        /// A position indicating unavailable Leap Motion data (e.g. no hands are detected).
        /// </summary>
        public static readonly Vector2 notAvailable = new Vector2(-1, -1);

        /// <summary>
        /// Connect to the device automatically on creation.
        /// </summary>
        void Awake() {
            device = new Controller();
            started = Time.unscaledTime;
        }

        /// <summary>
        /// Tries to reset the connection to the controller.
        /// </summary>
        public void Reconnect() {
            OnDestroy();
            Awake();
        }

        /// <summary>
        /// Safely disconnect the device after use.
        /// </summary>
        void OnDestroy() {
            if (device.IsConnected)
                device.ClearPolicy(Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
            device.StopConnection();
        }

        /// <summary>
        /// Returns if a connected Leap Motion has been found.
        /// </summary>
        public bool Connected => device.IsConnected;

        /// <summary>
        /// Check if the user is using the controller.
        /// </summary>
        /// <returns>True if there are any hands detected</returns>
        public bool IsUsed() => lastFrame.Hands.Count != 0;

        /// <summary>
        /// Get count of hands.
        /// </summary>
        /// <returns>The number of hands the device detects</returns>
        public int GetHandCount() => lastFrame.Hands.Count;

        /// <summary>
        /// Raw palm position data.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Palm position, or (-1, -1) if there's no hand</returns>
        public Vector3 PalmPosition(int handID = 0) {
            if (lastFrame.Hands.Count > handID) {
                Vector pos = lastFrame.Hands[handID].PalmPosition;
                return new Vector3(pos.x, pos.y, pos.z);
            } else
                return notAvailable;
        }

        /// <summary>
        /// Palm position on screen, on a vertical plane.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Palm position on screen, or (-1, -1) if there's no hand</returns>
        public Vector2 PalmOnScreenXY(int handID = 0) {
            List<Hand> hands = lastFrame.Hands;
            if (hands.Count > handID) {
                Hand checkedHand = hands[handID];
                Vector2 fromLeap = new Vector2(checkedHand.PalmPosition.x, -checkedHand.PalmPosition.y + leapLowerBounds.y + leapUpperBounds.y);
                return new Vector2(
                    (Mathf.Clamp(fromLeap.x, leapLowerBounds.x, leapUpperBounds.x) - leapLowerBounds.x) /
                        (leapUpperBounds.x - leapLowerBounds.x) * Screen.width,
                    (Mathf.Clamp(fromLeap.y, leapLowerBounds.y, leapUpperBounds.y) - leapLowerBounds.y) /
                        (leapUpperBounds.y - leapLowerBounds.y) * Screen.height);
            } else
                return notAvailable;
        }

        /// <summary>
        /// Palm position on or off screen, on a vertical plane.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Palm position on screen, or (-1, -1) if there's no hand</returns>
        public Vector2 PalmOnScreenXYUnclamped(int handID = 0) {
            List<Hand> hands = lastFrame.Hands;
            if (hands.Count > handID) {
                Hand checkedHand = hands[handID];
                Vector2 fromLeap = new Vector2(checkedHand.PalmPosition.x,
                    -checkedHand.PalmPosition.y + leapLowerBounds.y + leapUpperBounds.y);
                return new Vector2((fromLeap.x - leapLowerBounds.x) / (leapUpperBounds.x - leapLowerBounds.x) * Screen.width,
                    (fromLeap.y - leapLowerBounds.y) / (leapUpperBounds.y - leapLowerBounds.y) * Screen.height);
            } else
                return notAvailable;
        }

        /// <summary>
        /// Palm position on viewport, on a vertical plane.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Palm position on viewport, or (-1, -1) if there's no hand</returns>
        public Vector2 PalmOnViewportXY(int handID = 0) {
            List<Hand> hands = lastFrame.Hands;
            if (hands.Count > handID) {
                Hand checkedHand = hands[handID];
                Vector2 FromLeap = new Vector2(checkedHand.PalmPosition.x, -checkedHand.PalmPosition.y + leapLowerBounds.y + leapUpperBounds.y);
                return new Vector2(
                    (Mathf.Clamp(FromLeap.x, leapLowerBounds.x, leapUpperBounds.x) - leapLowerBounds.x)
                        / (leapUpperBounds.x - leapLowerBounds.x),
                    (Mathf.Clamp(FromLeap.y, leapLowerBounds.y, leapUpperBounds.y) - leapLowerBounds.y)
                        / (leapUpperBounds.y - leapLowerBounds.y));
            } else
                return notAvailable;
        }

        /// <summary>
        /// Palm position on screen, on a horizontal plane.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Palm position on screen, or (-1, -1) if there's no hand</returns>
        public Vector2 PalmOnScreenXZ(int handID = 0) {
            List<Hand> hands = lastFrame.Hands;
            if (hands.Count > handID) {
                Hand checkedHand = hands[handID];
                return new Vector2(
                    (Mathf.Clamp(checkedHand.PalmPosition.x, leapLowerBounds.x, leapUpperBounds.x) - leapLowerBounds.x) /
                        (leapUpperBounds.x - leapLowerBounds.x) * Screen.width,
                    (Mathf.Clamp(checkedHand.PalmPosition.z, leapLowerBounds.z, leapUpperBounds.z) - leapLowerBounds.z) /
                        (leapUpperBounds.z - leapLowerBounds.z) * Screen.height);
            } else
                return notAvailable;
        }

        /// <summary>
        /// Palm position on viewport, on a horizontal plane.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Palm position on viewport, or (-1, -1) if there's no hand</returns>
        public Vector2 PalmOnViewportXZ(int handID = 0) {
            List<Hand> hands = lastFrame.Hands;
            if (hands.Count > handID) {
                Hand checkedHand = hands[handID];
                return new Vector2(
                    (Mathf.Clamp(checkedHand.PalmPosition.x, leapLowerBounds.x, leapUpperBounds.x) - leapLowerBounds.x) /
                        (leapUpperBounds.x - leapLowerBounds.x),
                    (Mathf.Clamp(checkedHand.PalmPosition.z, leapLowerBounds.z, leapUpperBounds.z) - leapLowerBounds.z) /
                        (leapUpperBounds.z - leapLowerBounds.z));
            } else
                return notAvailable;
        }

        /// <summary>
        /// Furthest tip on screen on a vertical plane.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Furthest tip position on screen, or (-1, -1) if there's no hand</returns>
        public Vector2 SinglePointOnScreenXY(int handID = 0) {
            List<Hand> hands = lastFrame.Hands;
            if (hands.Count > handID) {
                Hand currentHand = hands[handID];
                Finger furthest = currentHand.Fingers[0];
                foreach (Finger checkedFinger in currentHand.Fingers)
                    if (furthest.TipPosition.z > checkedFinger.TipPosition.z)
                        furthest = checkedFinger;
                return new Vector2(
                    (Mathf.Clamp(furthest.TipPosition.x, leapLowerBounds.x, leapUpperBounds.x) - leapLowerBounds.x) /
                        (leapUpperBounds.x - leapLowerBounds.x) * Screen.width,
                    (Mathf.Clamp(furthest.TipPosition.y, leapLowerBounds.y, leapUpperBounds.y) - leapLowerBounds.y) /
                        (leapUpperBounds.y - leapLowerBounds.y) * Screen.height);
            } else
                return notAvailable;
        }

        /// <summary>
        /// Planar hand movement delta calculation.
        /// </summary>
        /// <param name="currentPosition">Current screen position</param>
        /// <param name="lastPositionHolder">Holder for the last position</param>
        /// <returns>Hand movement on the plane in pixels or viewport scale</returns>
        public Vector2 ScreenDelta(Vector2 currentPosition, ref Vector2 lastPositionHolder) {
            if (currentPosition == notAvailable || lastPositionHolder == notAvailable) {
                lastPositionHolder = currentPosition;
                return notAvailable;
            }
            Vector2 delta = currentPosition - lastPositionHolder;
            lastPositionHolder = currentPosition;
            return delta;
        }

        /// <summary>
        /// Extended fingers for a given hand.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Extended finger count</returns>
        public int ExtendedFingers(int handID = 0) {
            int counter = 0;
            List<Hand> hands = lastFrame.Hands;
            if (hands.Count > handID) {
                foreach (Finger checkedFinger in hands[handID].Fingers)
                    if (checkedFinger.IsExtended)
                        ++counter;
            }
            return counter;
        }

        /// <summary>
        /// Get the first detected left hand's ID or -1 if it's not found.
        /// </summary>
        /// <returns></returns>
        public int FirstLeftHand() {
            List<Hand> hands = lastFrame.Hands;
            int handCount = hands.Count;
            for (int i = 0; i < handCount; ++i)
                if (hands[i].IsLeft)
                    return i;
            return -1;
        }

        /// <summary>
        /// Get the first detected right hand's ID or -1 if it's not found.
        /// </summary>
        /// <returns></returns>
        public int FirstRightHand() {
            List<Hand> hands = lastFrame.Hands;
            for (int i = 0, handCount = hands.Count; i < handCount; ++i)
                if (!hands[i].IsLeft)
                    return i;
            return -1;
        }

        /// <summary>
        /// Get pointing direction if the hand is pointing a direction, relative to the controller.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Pointing direction vector for the given hand or null if it does not exist or point in a direction</returns>
        public Vector3? PointingDirection(int handID = 0) {
            int fingers = ExtendedFingers(handID);
            if ((fingers == 1 || fingers == 2) && lastFrame.Hands.Count > handID) {
                foreach (Finger checkedFinger in lastFrame.Hands[handID].Fingers) {
                    if (checkedFinger.IsExtended) {
                        Vector tipPos = checkedFinger.Bone(Bone.BoneType.TYPE_DISTAL).Basis.translation,
                            preTip = checkedFinger.Bone(Bone.BoneType.TYPE_INTERMEDIATE).Basis.translation;
                        return new Vector3(tipPos.x - preTip.x, tipPos.y - preTip.y, tipPos.z - preTip.z).normalized;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get a hand's rotation by averaging where each finger's base points.
        /// </summary>
        /// <param name="handID">Hand ID</param>
        /// <returns>Hand rotation</returns>
        public Quaternion? HandRotation(int handID = 0) {
            if (lastFrame.Hands.Count > handID) {
                Vector3 pointing = Vector3.zero;
                foreach (Finger checkedFinger in lastFrame.Hands[handID].Fingers) {
                    if (checkedFinger.Type != Finger.FingerType.TYPE_THUMB) {
                        Vector fingerBase = checkedFinger.Bone(checkedFinger.IsExtended ?
                                Bone.BoneType.TYPE_PROXIMAL : Bone.BoneType.TYPE_METACARPAL).Basis.translation,
                            end = checkedFinger.Bone(checkedFinger.IsExtended ?
                                Bone.BoneType.TYPE_INTERMEDIATE : Bone.BoneType.TYPE_PROXIMAL).Basis.translation;
                        pointing += new Vector3(end.x - fingerBase.x, end.y - fingerBase.y, end.z - fingerBase.z);
                    }
                }
                if (pointing != Vector3.zero) {
                    Vector palmNormal = lastFrame.Hands[handID].PalmNormal;
                    Vector3 forward = Quaternion.LookRotation(pointing, Vector3.up).eulerAngles;
                    Vector3 normal = Quaternion.FromToRotation(Vector3.up, new Vector3(palmNormal.x, palmNormal.y, palmNormal.z)).eulerAngles;
                    return Quaternion.Euler(-forward.x, -forward.y, -normal.z);
                }
            }
            return null;
        }

        /// <summary>
        /// Poll the head mounted flag and get last Leap frame in each application frame.
        /// </summary>
        void Update() {
            if (headMounted && !device.IsPolicySet(Controller.PolicyFlag.POLICY_OPTIMIZE_HMD))
                device.SetPolicy(Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
            lastFrame = device.Frame();

            if (!Connected && !float.IsNaN(started) && Time.unscaledTime - started > connectionTimeout) {
                Reconnect();
                started = float.NaN;
            }
        }
    }
}