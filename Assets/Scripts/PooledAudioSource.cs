using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PooledAudioSource : MonoBehaviour {
	public int id;
	
	private AudioSourcePool audioSourcePool;
	private AudioSource audioSource;
	
	void Awake() {
		audioSource = GetComponent<AudioSource>();
	}
	
	public void Initialize(AudioSourcePool audioSourcePool_, int id_) {
		audioSourcePool = audioSourcePool_;
		id = id_;
	}
}
