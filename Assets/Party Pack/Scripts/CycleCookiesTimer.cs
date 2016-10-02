using UnityEngine;
using System.Collections;

public class CycleCookiesTimer : MonoBehaviour {
	
	public float interval = 5.0f;
	public Cubemap[] cookies;
	
	private float t;
	private int i;
	
	// Use this for initialization
	void Start () {
		gameObject.light.cookie = cookies[0];
		t = interval;
	}
	
	// Update is called once per frame
	void Update () {
		t -= Time.deltaTime;
		if (t > 0.0f) return;
		i++;
		if (i >= cookies.Length) i = 0;
		gameObject.light.cookie = cookies[i];
		t = interval;
    }
}
