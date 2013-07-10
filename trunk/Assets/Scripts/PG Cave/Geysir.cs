using UnityEngine;
using System.Collections;

public class Geysir : MonoBehaviour {
	public static string TAG = "Geysir";
	
	private Play play;
	private bool hasParticleSystem;
	private Vector3 visibilityPosition;
	
	private static Vector3 GEYSIR_POSITION = new Vector3(0f, 0f, 1f);
		
	public void Initialize(Play play_) {
		play = play_;
		hasParticleSystem = false;
	}
	
	void Start() {
		visibilityPosition = transform.position + transform.TransformDirection(GEYSIR_POSITION);
	}
	
	void FixedUpdate() {
		if (!hasParticleSystem && play.ship.IsVisibleFrom(visibilityPosition) != Vector3.zero) {
			play.SetGeysirParticleSystem(transform);
			hasParticleSystem = true;
			//Debug.Log ("visible " + gameObject.GetInstanceID());
		} else if (hasParticleSystem && play.ship.IsVisibleFrom(visibilityPosition) == Vector3.zero) {
			play.LetGeysirParticleSystem();
			hasParticleSystem = false;
			//Debug.Log ("INvisible " + gameObject.GetInstanceID());
		}
	}
	
	void OnDisable() {
		if (hasParticleSystem) {
			play.LetGeysirParticleSystem();
			hasParticleSystem = false;
		}
	}
	
}

