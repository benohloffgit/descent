using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourcePool {
	private Game game;
	//private Dictionary<int, PooledAudioSource> freeAudioSources;
	//private Dictionary<int, PooledAudioSource> usedAudioSources;
	private List<PooledAudioSource> audioSources;
	
	private static int MAX_AUDIO_SOURCES = 16;
	public static int NO_AUDIO_SOURCE = -1;
	
	private int nextAudioSource;
	
	public AudioSourcePool(Game game_) {
		game = game_;
		
		nextAudioSource = 0;
		audioSources = new List<PooledAudioSource>();
		
		//freeAudioSources = new Dictionary<int, PooledAudioSource>();
		//usedAudioSources = new Dictionary<int, PooledAudioSource>();
		
		for (int i=0; i<MAX_AUDIO_SOURCES; i++) {	
			PooledAudioSource pAS = (GameObject.Instantiate(game.pooledAudioSource) as GameObject).GetComponent<PooledAudioSource>();
			pAS.Initialize(i);
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
			audioSources[nextAudioSource].transform.localPosition = Vector3.zero;
			nextAudioSource++;
			if (nextAudioSource == MAX_AUDIO_SOURCES) {
				nextAudioSource = 0;
			}
		}		
		audioSources[usedAudioSource].audioSource.PlayOneShot(clip);
//		Debug.Log ("Using AudioSource " + audioSources[usedAudioSource].id);
		return usedAudioSource;
	}

	public void PlaySound(Vector3 pos, AudioClip clip) {
		audioSources[nextAudioSource].transform.position = pos;
		//audioSources[nextAudioSource].transform.localPosition = Vector3.zero;
		audioSources[nextAudioSource].audioSource.PlayOneShot(clip);
		nextAudioSource++;
		if (nextAudioSource == MAX_AUDIO_SOURCES) {
			nextAudioSource = 0;
		}
//		Debug.Log ("Using AudioSource " + audioSources[usedAudioSource].id);
	}

	public static void DecoupleAudioSource(PooledAudioSource aS) {
		if (aS != null) {
//			Debug.Log ("AudioSource disabled " + aS.id + " " + aS.gameObject.GetInstanceID());
			aS.transform.parent = null;
		}
	}
	
	public static void DecoupleAudioSource(PooledAudioSource[] audioSources) {
		foreach (PooledAudioSource aS in audioSources) {
			if (aS != null) {
//				Debug.Log ("AudioSource disabled " + aS.id + " " + aS.gameObject.GetInstanceID());
				aS.transform.parent = null;
			}
		}
	}
}
