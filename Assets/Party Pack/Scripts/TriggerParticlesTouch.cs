using UnityEngine;
using System.Collections;

public class TriggerParticlesTouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		gameObject.particleSystem.Stop();
		gameObject.particleSystem.Play();
		if (gameObject.audio != null) gameObject.audio.Play();
    }
}
