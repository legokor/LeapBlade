using Menus;
using UnityEngine;

namespace Movement {
    public class Wiggle : MonoBehaviour {
        public Vector3 maxOffset = Vector3.one;
        public float swapDistance = .25f;
        public float speed = 20;
        public TimeGenerator time;

        Vector3 startPos, nextPos;

        void Start() => startPos = nextPos = transform.position;

        void Update() {
            if (Vector3.Distance(transform.position, nextPos) < swapDistance)
                nextPos = startPos +
                    transform.rotation * Vector3.Scale(maxOffset, new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
            transform.position = Vector3.Lerp(transform.position, nextPos, time.DeltaTime * speed);
        }
    }
}