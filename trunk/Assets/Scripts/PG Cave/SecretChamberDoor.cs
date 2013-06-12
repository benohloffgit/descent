using UnityEngine;
using System.Collections;

public class SecretChamberDoor : MonoBehaviour {
		
	private SphereCollider doorCollider;
	private BoxCollider caveCollider;
	private Play play;
	private Renderer myRenderer;
	
	void Awake() {
		doorCollider = GetComponentInChildren<SphereCollider>();
		caveCollider = GetComponentInChildren<BoxCollider>();
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
	
	public void Open() {
		doorCollider.enabled = false;
		caveCollider.enabled = false;
		myRenderer.enabled = false;
	}

	public void Close() {
		doorCollider.enabled = true;
		caveCollider.enabled = true;
		myRenderer.enabled = true;
	}
}

