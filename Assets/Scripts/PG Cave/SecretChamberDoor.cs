using UnityEngine;
using System.Collections;

public class SecretChamberDoor : MonoBehaviour {
		
	private SphereCollider doorCollider;
	private Play play;
	private Renderer myRenderer;
	
	void Awake() {
		doorCollider = GetComponentInChildren<SphereCollider>();
		myRenderer = GetComponentInChildren<Renderer>();
	}
	
	public void Initialize(Play play_) {
		play = play_;
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.SwitchMode();
		}
	}

	void OnTriggerExit(Collider other) {
	}
	
	public void Open() {
		doorCollider.enabled = false;
		myRenderer.enabled = false;
	}

	public void Close() {
		doorCollider.enabled = true;
		myRenderer.enabled = true;
	}
}

