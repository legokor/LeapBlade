using Game;
using UnityEngine;

public class Office : MonoBehaviour {
    public GameObject[] roofElements;

    /// <summary>
    /// Roof element scaling multiplier on roof crash.
    /// </summary>
    public float downscale = .9f;

    /// <summary>
    /// Maximum force in any direction for a roof element when the roof crashes down.
    /// </summary>
    public float explosiveForce = 3000;

    /// <summary>
    /// The height to drop down players from.
    /// </summary>
    public float roofHeight = 7;

    Vector3[] positions, scales;
    Quaternion[] rotations;

    void Start() {
        positions = new Vector3[roofElements.Length];
        rotations = new Quaternion[roofElements.Length];
        scales = new Vector3[roofElements.Length];
        for (int i = 0; i < roofElements.Length; ++i) {
            positions[i] = roofElements[i].transform.localPosition;
            rotations[i] = roofElements[i].transform.localRotation;
            scales[i] = roofElements[i].transform.localScale;
        }
    }

    public void OnKO() {
        for (int i = 0; i < roofElements.Length; ++i) {
            Rigidbody body = roofElements[i].GetComponent<Rigidbody>();
            if (!body)
                body = roofElements[i].AddComponent<Rigidbody>();

            roofElements[i].transform.localPosition = positions[i];
            roofElements[i].transform.localRotation = rotations[i];
            roofElements[i].transform.localScale = scales[i] * downscale;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.AddForce(Random.rotation * new Vector3(Random.Range(-explosiveForce, explosiveForce), 0));
        }

        // TODO: drop down from the sky to the floor
        //Vector3 dropPoint = new(0, roofHeight, 0);
        //Announcer.Instance.fighterLeft.transform.position += dropPoint;
        //Announcer.Instance.fighterRight.transform.position += dropPoint;
    }
}