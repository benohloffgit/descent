using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourcePool {
	private Game game;
	//private Dictionary<int, PooledAudioSource> freeAudioSources;
	//private Dictionary<int, PooledAudioSource> usedAudioSources;
	private List<AudioSource> audioSources;
	
	private static int MAX_AUDIO_SOURCES = 8;
	public static int NO_AUDIO_SOURCE = -1;
	
	private int nextAudioSource;
	
	public AudioSourcePool(Game game_) {
		game = game_;
		
		nextAudioSource = 0;
		audioSources = new List<AudioSource>();
		
		//freeAudioSources = new Dictionary<int, PooledAudioSource>();
		//usedAudioSources = new Dictionary<int, PooledAudioSource>();
		
		for (int i=0; i<MAX_AUDIO_SOURCES; i++) {	
			AudioSource pAS = (GameObject.Instantiate(game.pooledAudioSource) as GameObject).GetComponent<AudioSource>();
			//pAS.Initialize(this, i);
			audioSources.Add(pAS);
		}
	}
	
/*	public void AttachAudioSource(Transform t) {
		if (freeAudioSources.Count > 0) {
			PooledAudioSource pAS = freeAudioSources.GetEnumerator().Current.Value;
			pAS.transform.parent = t;
		} else {
			Debug.Log ("no audio source available");
		}
	}*/
	
	public int PlaySound(int audioSourceID, Transform t, AudioClip clip) {
		int usedAudioSource;
		if (audioSourceID != NO_AUDIO_SOURCE && audioSources[audioSourceID].transform.parent == t) {
			usedAudioSource = audioSourceID;
		} else {
			usedAudioSource = nextAudioSource;
			audioSources[nextAudioSource].transform.parent = t;
			nextAudioSource++;
			if (nextAudioSource == MAX_AUDIO_SOURCES) {
				nextAudioSource = 0;
			}
		}		
		audioSources[usedAudioSource].PlayOneShot(clip);
		return usedAudioSource;
	}
}
