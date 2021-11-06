using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeapVR {
    /// <summary>
    /// Unity UI mouse simulation for Leap Motion.
    /// </summary>
    [DefaultExecutionOrder(int.MinValue + 1000 + 100)]
    [AddComponentMenu("Leap VR/Leap Mouse")]
    public class LeapMouse : Singleton<LeapMouse> {
        [Tooltip("Cursor texture.")]
        public Texture mouseIcon;
        [Tooltip("On-screen off-hand marker.")]
        public Texture offHandIcon;
        [Tooltip("Size of the cursor.")]
        public Vector2 mouseSize = new Vector2(64, 64);
        [Tooltip("The center of the cursor is the selection.")]
        public bool centerPointer = true;
        [Tooltip("Raycaster for finding UI elements.")]
        public GraphicRaycaster raycaster;

        /// <summary>
        /// A tap happened in the last frame.
        /// </summary>
        bool tapped = false;
        /// <summary>
        /// The UI element the cursor was over last frame.
        /// </summary>
        Selectable lastHovered;
        /// <summary>
        /// Dummy data required for some UI calls.
        /// </summary>
        PointerEventData pointer;
        /// <summary>
        /// Extended finger count at the last frame.
        /// </summary>
        int lastFingerCount = 0;
        /// <summary>
        /// Cached hand position.
        /// </summary>
        Vector2 handPosition = new Vector2(-1, -1);

        /// <summary>
        /// Create a ray from the camera at the given screen point.
        /// </summary>
        public static Ray ScreenPointToRay() {
            Vector2 leapPosition = LeapMotion.Instance.PalmOnScreenXY();
            return SBS.StereoRay(leapPosition != LeapMotion.notAvailable ? new Vector3(leapPosition.x * .5f, Screen.height - leapPosition.y) :
                Input.mousePosition);
        }

        /// <summary>
        /// Gets if the user tapped or clicked.
        /// </summary>
        public bool ActionDown() => LeapMotion.Instance.IsUsed() ? tapped : Input.GetMouseButtonDown(0);

        /// <summary>
        /// Gets if the user grabs.
        /// </summary>
        public bool Action() => LeapMotion.Instance.IsUsed() ? lastFingerCount == 0 : Input.GetMouseButton(0);

        /// <summary>
        /// Gets the pointer (mouse or main hand) position on screen.
        /// </summary>
        public Vector2 ScreenPosition() {
            if (LeapMotion.Instance.IsUsed()) {
                Vector2 leapPos = LeapMotion.Instance.PalmOnScreenXY();
                return SBS.Enabled ? new Vector2(leapPos.x * .5f, leapPos.y) : leapPos;
            } else
                return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        /// <summary>
        /// Gets the pointer (mouse or main hand) position on or off screen.
        /// </summary>
        public Vector2 ScreenPositionUnclamped() {
            if (LeapMotion.Instance.IsUsed()) {
                Vector2 leapPos = LeapMotion.Instance.PalmOnScreenXYUnclamped();
                return SBS.Enabled ? new Vector2(leapPos.x * .5f, leapPos.y) : leapPos;
            } else
                return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }

        void Start() => pointer = new PointerEventData(GetComponent<EventSystem>());

        /// <summary>
        /// Draw a cursor on the screen.
        /// </summary>
        /// <param name="position">Position on the screen</param>
        /// <param name="fullSize">Draw a full size (not pressed) cursor</param>
        /// <param name="cursor">Cursor texture</param>
        void DrawPointer(Vector2 position, bool fullSize, Texture cursor) {
            Vector2 drawStartPos = position;
            if (centerPointer)
                drawStartPos -= mouseSize * (fullSize ? .6f : .5f);
            GUI.DrawTexture(new Rect(drawStartPos, mouseSize * (fullSize ? 1 : .8f)), cursor);
            if (SBS.Enabled) {
                float halfWidth = Screen.width * .5f;
                drawStartPos.x += drawStartPos.x < halfWidth ? halfWidth : -halfWidth;
                GUI.DrawTexture(new Rect(drawStartPos, mouseSize * (fullSize ? 1 : .8f)), cursor);
            }
        }

        void OnGUI() {
            if (LeapMotion.Instance.IsUsed()) {
                DrawPointer(handPosition, lastFingerCount != 0, mouseIcon);
                if (offHandIcon) {
                    for (int offHand = 1; offHand < LeapMotion.Instance.GetHandCount(); ++offHand) {
                        Vector2 leapPos = LeapMotion.Instance.PalmOnScreenXYUnclamped(offHand);
                        DrawPointer(SBS.Enabled ? new Vector2(leapPos.x * .5f, leapPos.y) : leapPos,
                            LeapMotion.Instance.ExtendedFingers(offHand) != 0, offHandIcon);
                    }
                }
            }
        }

        void Update() {
            handPosition = ScreenPosition();
            pointer.position = new Vector2(handPosition.x, Screen.height - handPosition.y);
            int fingerCount = LeapMotion.Instance.ExtendedFingers();
            tapped = fingerCount == 0 && lastFingerCount != 0;
            List<RaycastResult> hovered = new List<RaycastResult>();
            raycaster.Raycast(pointer, hovered);
            if (hovered.Count != 0) {
                for (int i = 0; i < hovered.Count; ++i) {
                    Selectable current = hovered[i].gameObject.GetComponent<Selectable>();
                    if (current == null)
                        continue;
                    if (current) {
                        if (lastHovered && current != lastHovered)
                            lastHovered.OnPointerExit(pointer);
                        current.OnPointerEnter(pointer);
                        lastHovered = current;
                        if (ActionDown())
                            if (current.GetType() == typeof(Button))
                                ((Button)current).OnPointerClick(pointer);
                            else if (current.GetType() == typeof(Toggle))
                                ((Toggle)current).isOn ^= true;
                            else
                                current.Select();
                    } else if (lastHovered)
                        lastHovered.OnPointerExit(pointer);
                }
            } else if (lastHovered)
                lastHovered.OnPointerExit(pointer);
            lastFingerCount = fingerCount;
        }
    }
}