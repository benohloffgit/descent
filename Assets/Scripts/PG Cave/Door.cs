using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	
	public static int TYPE_ENTRY = 0;
	public static int TYPE_EXIT = 1;
	
	private static string ANIM_OPEN_DOOR = "Door Open";
	private static string ANIM_CLOSE_DOOR = "Door Close";
	
	private MeshCollider doorCollider;
	private Play play;
	private int type;
	private bool isShut;
	
	void Awake() {
		doorCollider = GetComponentInChildren<MeshCollider>();
	}
	
	public void Initialize(Play play_, int type_) {
		play = play_;
		type = type_;
		isShut = false;
	}

	void OnTriggerEnter(Collider other) {
		if (!isShut && other.tag == Ship.TAG) {
			animation.Play(ANIM_OPEN_DOOR);
			doorCollider.enabled = false;
		}
	}

	void OnTriggerExit(Collider other) {
		if (!isShut && other.tag == Ship.TAG) {
			animation.Play(ANIM_CLOSE_DOOR);
			doorCollider.enabled = true;
		}
		if (type == TYPE_ENTRY && play.isShipInPlayableArea) {
			isShut = true;
		} else if (type == TYPE_EXIT && !play.isShipInPlayableArea) {
			isShut = true;
		}
	}
}
