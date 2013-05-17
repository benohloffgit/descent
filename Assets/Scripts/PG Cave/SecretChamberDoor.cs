using UnityEngine;
using System.Collections;

public class SecretChamberDoor : MonoBehaviour {
		
	private MeshCollider doorCollider;
	private Play play;
	
	void Awake() {
		doorCollider = GetComponentInChildren<MeshCollider>();
	}
	
	public void Initialize(Play play_) {
		play = play_;
	}
	
	void OnTriggerEnter(Collider other) {
	}

	void OnTriggerExit(Collider other) {
	}
	
	public void Open() {
	}
}

