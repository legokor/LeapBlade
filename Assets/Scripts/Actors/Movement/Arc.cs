using UnityEngine;

namespace Movement {
    public class Arc : MonoBehaviour {
        public Transform target;
        public float arcWidth = 45;

        Vector3 direction;

        void Start() => direction = transform.position - target.position;

        void Update() {
            transform.position = target.position + Quaternion.Euler(0, Mathf.Sin(Time.unscaledTime) * arcWidth, 0) * direction;
            transform.LookAt(target);
        }
    }
}