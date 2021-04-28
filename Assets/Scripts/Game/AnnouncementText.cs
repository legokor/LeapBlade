using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class AnnouncementText : MonoBehaviour {
        public Text target, shadow;
        public float time = 3;
        public string text;

        public float growTime = .25f;
        public float maxSize = 1.25f;
        public float fadeTime = .25f;

        Color sourceColor, shadowSource;
        float remainingTime;

        void Awake() {
            target.text = text;
            shadow.text = text;
            remainingTime = time;
            sourceColor = target.color;
            shadowSource = shadow.color;
        }

        public void NewText(string text) {
            target.rectTransform.localScale = new Vector3();
            target.text = text;
            shadow.text = text;
            remainingTime = time;
            target.color = sourceColor;
            shadow.color = shadowSource;
        }

        void Update() {
            float growEnd = time - growTime;
            if (remainingTime > growEnd) {
                float grow = 1 - Mathf.Sqrt((remainingTime - growEnd) / growTime);
                target.rectTransform.localScale = new Vector3(grow, grow, grow);
            } else {
                float grow = 1 + (1 - Mathf.Sqrt(remainingTime / growEnd)) * (maxSize - 1);
                if (!float.IsNaN(grow))
                    target.rectTransform.localScale = new Vector3(grow, grow, grow);
                if (remainingTime < fadeTime) {
                    float fade = remainingTime / fadeTime;
                    if (fade < 0)
                        fade = 0;
                    target.color = new Color(sourceColor.r, sourceColor.g, sourceColor.b, fade);
                    shadow.color = new Color(shadowSource.r, shadowSource.g, shadowSource.b, fade);
                }
            }
            remainingTime -= Time.deltaTime;
        }
    }
}
