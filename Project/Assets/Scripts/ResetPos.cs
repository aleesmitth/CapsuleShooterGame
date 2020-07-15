using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPos : MonoBehaviour {

    public Transform spawn;

    private void OnTriggerEnter(Collider other) {
        other.transform.position = spawn.position;
    }
}
