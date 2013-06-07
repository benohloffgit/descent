using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	
	public static int TYPE_LAST_EXIT = 0;
	public static int TYPE_ENTRY = 1;
	public static int TYPE_EXIT = 2;
	public static int TYPE_NEXT_ENTRY = 3;
	
	private static string ANIM_OPEN_DOOR = "Door Open";
	private static string ANIM_CLOSE_DOOR = "Door Close";
	
	private BoxCollider doorCollider;
	private Play play;
	private int type;
	private bool isShut;
	private int myAudioSourceID = AudioSourcePool.NO_AUDIO_SOURCE;
	
	void Awake() {
		doorCollider = GetComponentInChildren<BoxCollider>();
	}
	
	public void Initialize(Play play_, int type_) {
		play = play_;
		type = type_;
		Reset();
	}
	
	public void Reset() {
		if (type == TYPE_LAST_EXIT || type == TYPE_NEXT_ENTRY || type == TYPE_EXIT) {
			isShut = true;
		} else {
			isShut = false;
		}
		//animation.Play(ANIM_CLOSE_DOOR);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG && !isShut) {
			animation.Play(ANIM_OPEN_DOOR);
			myAudioSourceID = play.game.PlaySound(myAudioSourceID, transform, Game.SOUND_TYPE_VARIOUS, 9);
			doorCollider.enabled = false;
		}
	}

	void OnTriggerExit(Collider other) {
		if (!isShut && other.tag == Ship.TAG) {
			animation.Play(ANIM_CLOSE_DOOR);
			myAudioSourceID = play.game.PlaySound(myAudioSourceID, transform, Game.SOUND_TYPE_VARIOUS, 9);
			doorCollider.enabled = true;
			if (type == TYPE_ENTRY && play.isShipInPlayableArea) {
				isShut = true;
			} else if (type == TYPE_EXIT && !play.isShipInPlayableArea) {
				isShut = true;
				play.ZoneCompleted();
			}
		}
	}
	
	public void Open() {
		isShut = false;
	}
}
