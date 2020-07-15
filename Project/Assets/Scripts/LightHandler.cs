using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHandler : MonoBehaviour {

    public new Light light;
    public new ParticleSystem particleSystem;

    // Update is called once per frame
    private void Update() {
        if (particleSystem.isPlaying) {
            light.enabled = true;   
        }

        light.enabled = false;
    }
}
