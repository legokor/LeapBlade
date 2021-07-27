using Cavern;
using LeapVR;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menus {
    public class Menu : MonoBehaviour {
        public TimeGenerator time;
        public Button[] options;
        public GameObject selection;
        public GraphicRaycaster raycaster;
        public EventSystem eventSystem;

        [Header("Sounds")]
        public AudioSource3D source;
        public AudioClip move;
        public AudioClip select;

        protected int Selected => selected;

        protected bool active = true;
        protected float pace;
        protected Transform camTarget;

        int selected = 0;
        List<RaycastResult> results;
        PointerEventData pointer;
        RectTransform selector;
        new Transform camera;
        Vector2 scaleOffset;
        Vector3 posOffset;

        protected void Start() {
            RectTransform target = (RectTransform)options[0].transform;
            results = new List<RaycastResult>();
            pointer = new PointerEventData(eventSystem);
            selector = (RectTransform)selection.transform;
            posOffset = selector.localPosition - target.localPosition;
            scaleOffset = selector.sizeDelta - target.sizeDelta;
            camera = Camera.main.transform;
        }

        protected void Update() {
            pace = 10 * time.DeltaTime;

            if (camTarget)
                camera.SetPositionAndRotation(Vector3.Lerp(camera.position, camTarget.position, pace),
                    Quaternion.Lerp(camera.rotation, camTarget.rotation, pace));

            if (!active)
                return;

            if (LeapMouse.Instance) {
                Vector3 leapPos = LeapMouse.Instance.ScreenPosition();
                leapPos.y = Screen.height - leapPos.y;
                pointer.position = leapPos;
            } else
                pointer.position = Input.mousePosition;
            results.Clear();
            raycaster.Raycast(pointer, results);
            for (int i = 0, c = results.Count; i < c; ++i)
                for (int j = 0; j < options.Length; ++j)
                    if (results[i].gameObject == options[j].gameObject)
                        selected = j;

            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                selected = (selected + 1) % options.Length;
                source.PlayOneShot(move);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                selected = (options.Length + selected - 1) % options.Length;
                source.PlayOneShot(move);
            }
            if (Input.GetKeyDown(KeyCode.Return)) {
                options[selected].onClick.Invoke();
                source.PlayOneShot(select);
            }

            RectTransform target = (RectTransform)options[selected].transform;
            selector.localPosition = Vector3.Lerp(selector.localPosition, target.localPosition + posOffset, pace);
            selector.sizeDelta = Vector3.Lerp(selector.sizeDelta, target.sizeDelta + scaleOffset, pace);
        }
    }
}