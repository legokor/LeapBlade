using Cavern;
using UnityEngine;
using UnityEngine.UI;

namespace Menus {
    public class Menu : MonoBehaviour {
        public TimeGenerator time;
        public Button[] options;
        public GameObject selection;

        [Header("Sounds")]
        public AudioSource3D source;
        public AudioClip move;
        public AudioClip select;

        protected int Selected => selected;

        protected bool active = true;
        protected float pace;
        protected Transform camTarget;

        int selected = 0;
        RectTransform selector;
        new Transform camera;
        Vector2 scaleOffset;
        Vector3 posOffset;

        protected void Start() {
            RectTransform target = (RectTransform)options[0].transform;
            selector = (RectTransform)selection.transform;
            posOffset = selector.localPosition - target.localPosition;
            scaleOffset = selector.sizeDelta - target.sizeDelta;
            camera = Camera.main.transform;
        }

        protected void Update() {
            pace = 10 * time.DeltaTime;

            if (camTarget) {
                camera.position = Vector3.Lerp(camera.position, camTarget.position, pace);
                camera.rotation = Quaternion.Lerp(camera.rotation, camTarget.rotation, pace);
            }

            if (!active)
                return;

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