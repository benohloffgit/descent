using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PooledAudioSource : MonoBehaviour {
	public int id;
	
	public AudioSource audioSource;
	
	void Awake() {
		audioSource = GetComponent<AudioSource>();
	}
	
	public void Initialize(int id_) {
		id = id_;
	}
	
//	void OnDisable() {
//		Debug.Log ("this is inadvertently disabled " + id + " " + gameObject.GetInstanceID() +  " " + GetComponentInChildren<Explosion>());
//	}
	
//	void OnDestroy() {
//		Debug.Log ("this is inadvertently destroyed "  + id + " " + gameObject.GetInstanceID());
//	}
}
