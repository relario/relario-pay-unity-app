using System.Collections;
using UnityEngine;

public class Spin : MonoBehaviour {

    private Vector3 degreePerSecond = new Vector3 (1, 1, 1);
    private bool reverse = false;

    void Update () {
        if (transform.rotation.eulerAngles.z > 5 && transform.rotation.eulerAngles.z < 10 && !reverse) {
            reverse = true;
        }

        if (transform.rotation.eulerAngles.z < 355 && transform.rotation.eulerAngles.z > 10 && reverse) {
            reverse = false;
        }

        if (reverse) {
            transform.Rotate (degreePerSecond * -1 * Time.deltaTime);
        } else {
            transform.Rotate (degreePerSecond * Time.deltaTime);
        }
    }
}