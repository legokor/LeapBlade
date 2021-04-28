using UnityEngine;

namespace Actors {
    public class HealthBar : MonoBehaviour {
        public RectTransform bar;

        Vector3 startScale = Vector3.one;

        private void Start() => startScale = bar.localScale;

        public void SetHealth(float range) {
            bar.localScale = new Vector3(startScale.x * range, startScale.y, startScale.z);
        }
    }
}