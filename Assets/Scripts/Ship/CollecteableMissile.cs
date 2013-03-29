using System;
using UnityEngine;

public class CollecteableMissile : MonoBehaviour {		
	private Play play;
	
	private int type;
	
	public void Initialize(Play play_, int type_) {
		play = play_;
		type = type_;
	}

	void OnCollisionEnter(Collision c) {
		if (c.collider.tag == Ship.TAG) {
			play.AddMissile(type);
			Destroy(gameObject);
		}
	}
}
