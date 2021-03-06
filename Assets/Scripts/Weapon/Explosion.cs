using System;
using UnityEngine;

public class Explosion : MonoBehaviour {
	
	private Play play;
	
	private static int EXPLOSION_SOUND_MIN = 19;
	private static int EXPLOSION_SOUND_MAX = 21;
		
	public void Initialize(Play p) {
		play = p;
	}
	
	void Start() {
		Invoke ("DestroySelf", 2.5f);
		play.game.PlaySound(AudioSourcePool.NO_AUDIO_SOURCE, transform, Game.SOUND_TYPE_VARIOUS, UnityEngine.Random.Range(EXPLOSION_SOUND_MIN, EXPLOSION_SOUND_MAX+1));
	}
	
	private void DestroySelf() {
		Destroy(gameObject);
	}

	void OnDisable() {
		AudioSourcePool.DecoupleAudioSource(GetComponentInChildren<PooledAudioSource>());
	}
	
}

