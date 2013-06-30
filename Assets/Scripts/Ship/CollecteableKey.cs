using System;
using UnityEngine;

public class CollecteableKey : MonoBehaviour {
	public static string TAG = "Key";
	
	private Play play;
	
	private float angle;
	
	public static int TYPE_SILVER = 0;
	public static int TYPE_GOLD = 1;
	
	private Transform keyMesh;
	
	private int type;
	
	void Awake() {
		keyMesh = transform.Find("Key Mesh");
	}
	
	public void Initialize(Play play_, int type_, Texture2D keyTexture) {
		keyMesh.renderer.material.mainTexture = keyTexture;
		play = play_;
		type = type_;
		angle = 0f;
	}

	void FixedUpdate() {
		Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
			if (!transform.particleSystem.isPlaying) {
				transform.particleSystem.Play();
			}
			keyMesh.LookAt(play.GetShipPosition(), play.ship.transform.up);
			angle += 0.05f;
			keyMesh.RotateAround(keyMesh.up, angle);
			keyMesh.Rotate(Vector3.right, 30.0f);
			if (angle >= 360f) {
				angle -= 360f;
			}
		} else {
			if (!transform.particleSystem.isPlaying) {
				transform.particleSystem.Stop();
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == Ship.TAG) {
			play.KeyFound(type);
			play.ship.PlaySound(Game.SOUND_TYPE_VARIOUS, 24);
			Destroy(gameObject);
		}
	}

}

