using UnityEngine;

public class RelativeDolly : MonoBehaviour {
    public float maxDistance = 1;
    public float timeScale = .25f;
    public Vector3 direction = new Vector3(0, 0, 1);

    Vector3 first;

    void Start() => first = transform.localPosition;

    void Update() => transform.localPosition = first + direction * (maxDistance * Mathf.Sin(Time.unscaledTime * timeScale));
}