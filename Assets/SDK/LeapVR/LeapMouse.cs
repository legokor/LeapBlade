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
        public Texture MouseIcon;
        [Tooltip("On-screen off-hand marker.")]
        public Texture OffHandIcon;
        [Tooltip("Size of the cursor.")]
        public Vector2 MouseSize = new Vector2(64, 64);
        [Tooltip("The center of the cursor is the selection.")]
        public bool CenterPointer = true;

        /// <summary>
        /// A tap happened in the last frame.
        /// </summary>
        bool Tapped = false;
        /// <summary>
        /// The UI element the cursor was over last frame.
        /// </summary>
        Selectable LastHovered;
        /// <summary>
        /// Dummy data required for some UI calls.
        /// </summary>
        PointerEventData RandomPointerEventData;
        /// <summary>
        /// Extended finger count at the last frame.
        /// </summary>
        int LastFingerCount = 0;
        /// <summary>
        /// Cached hand position.
        /// </summary>
        Vector2 HandPosition = new Vector2(-1, -1);

        /// <summary>
        /// Create a ray from the camera at the given screen point.
        /// </summary>
        public static Ray ScreenPointToRay() {
            Vector2 LeapPosition = LeapMotion.Instance.PalmOnScreenXY();
            return SBS.StereoRay(LeapPosition != LeapMotion.notAvailable ? new Vector3(LeapPosition.x * .5f, Screen.height - LeapPosition.y) :
                Input.mousePosition);
        }

        /// <summary>
        /// Gets if the user tapped or clicked.
        /// </summary>
        public bool ActionDown() => LeapMotion.Instance.IsUsed() ? Tapped : Input.GetMouseButtonDown(0);

        /// <summary>
        /// Gets if the user grabs.
        /// </summary>
        public bool Action() => LeapMotion.Instance.IsUsed() ? LastFingerCount == 0 : Input.GetMouseButton(0);

        /// <summary>
        /// Gets the pointer (mouse or main hand) position on screen.
        /// </summary>
        public Vector2 ScreenPosition() {
            if (LeapMotion.Instance.IsUsed()) {
                Vector2 LeapPos = LeapMotion.Instance.PalmOnScreenXY();
                return SBS.Enabled ? new Vector2(LeapPos.x * .5f, LeapPos.y) : LeapPos;
            } else
                return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }

        /// <summary>
        /// Gets the pointer (mouse or main hand) position on or off screen.
        /// </summary>
        public Vector2 ScreenPositionUnclamped() {
            if (LeapMotion.Instance.IsUsed()) {
                Vector2 LeapPos = LeapMotion.Instance.PalmOnScreenXYUnclamped();
                return SBS.Enabled ? new Vector2(LeapPos.x * .5f, LeapPos.y) : LeapPos;
            } else
                return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }

        void Start() => RandomPointerEventData = new PointerEventData(GetComponent<EventSystem>());

        /// <summary>
        /// Draw a cursor on the screen.
        /// </summary>
        /// <param name="Position">Position on the screen</param>
        /// <param name="FullSize">Draw a full size (not pressed) cursor</param>
        /// <param name="Cursor">Cursor texture</param>
        void DrawPointer(Vector2 Position, bool FullSize, Texture Cursor) {
            Vector2 DrawStartPos = Position;
            if (CenterPointer)
                DrawStartPos -= MouseSize * (FullSize ? .6f : .5f);
            GUI.DrawTexture(new Rect(DrawStartPos, MouseSize * (FullSize ? 1 : .8f)), Cursor);
            if (SBS.Enabled) {
                float HalfWidth = Screen.width * .5f;
                DrawStartPos.x += DrawStartPos.x < HalfWidth ? HalfWidth : -HalfWidth;
                GUI.DrawTexture(new Rect(DrawStartPos, MouseSize * (FullSize ? 1 : .8f)), Cursor);
            }
        }

        void OnGUI() {
            if (LeapMotion.Instance.IsUsed()) {
                DrawPointer(HandPosition, LastFingerCount != 0, MouseIcon);
                if (OffHandIcon) {
                    for (int OffHand = 1; OffHand < LeapMotion.Instance.GetHandCount(); ++OffHand) {
                        Vector2 LeapPos = LeapMotion.Instance.PalmOnScreenXYUnclamped(OffHand);
                        DrawPointer(SBS.Enabled ? new Vector2(LeapPos.x * .5f, LeapPos.y) : LeapPos,
                            LeapMotion.Instance.ExtendedFingers(OffHand) != 0, OffHandIcon);
                    }
                }
            }
        }

        void Update() {
            HandPosition = ScreenPosition();
            int FingerCount = LeapMotion.Instance.ExtendedFingers();
            Tapped = FingerCount == 0 && LastFingerCount != 0;
            if (Physics.Raycast(SBS.StereoRay(new Vector2(HandPosition.x, Screen.height - HandPosition.y)), out RaycastHit hit)) {
                Selectable Hovered = hit.collider.gameObject.GetComponentInChildren<Selectable>();
                if (Hovered) {
                    if (LastHovered && Hovered != LastHovered)
                        LastHovered.OnPointerExit(RandomPointerEventData);
                    Hovered.OnPointerEnter(RandomPointerEventData);
                    LastHovered = Hovered;
                    if (ActionDown())
                        if (Hovered.GetType() == typeof(Button))
                            ((Button)Hovered).OnPointerClick(RandomPointerEventData);
                        else if (Hovered.GetType() == typeof(Toggle))
                            ((Toggle)Hovered).isOn ^= true;
                        else
                            Hovered.Select();
                } else if (LastHovered)
                    LastHovered.OnPointerExit(RandomPointerEventData);
            } else if (LastHovered)
                LastHovered.OnPointerExit(RandomPointerEventData);
            LastFingerCount = FingerCount;
        }
    }
}