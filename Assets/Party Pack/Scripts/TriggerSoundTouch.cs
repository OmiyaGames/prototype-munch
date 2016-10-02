using UnityEngine;
using System.Collections;

public class TriggerSoundTouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		if (gameObject.audio != null) gameObject.audio.Play();
    }
}
