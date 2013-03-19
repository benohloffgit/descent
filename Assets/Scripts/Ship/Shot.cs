using System;
using UnityEngine;

public class Shot : MonoBehaviour {
	public Game.Shot shotType;
		
	private Play play;
	
	private int damage;
	private int source;
	
	public void Initialize(Play p, int damage_, int source_, Game.Shot shotType_) {
		play = p;
		damage = damage_;
		source = source_;
		shotType = shotType_;
	}
	
	void Start() {
		Invoke ("DestroySelf", 10.0f);
	}
	
	void OnCollisionEnter(Collision c) {
		CancelInvoke("DestroySelf");
		if (c.collider.tag == Ship.TAG) {
			play.DamageShip(damage, source);
//			Debug.Log ("HIT Ship");
		} else if (c.collider.tag == Enemy.TAG) {
			play.DamageEnemy(damage, c.collider.GetComponent<Enemy>(), c.contacts[0].point, source);
		} else {
			play.DamageNothing(source);
		}
		Destroy(gameObject);
	}	

	private void DestroySelf() {
		play.DamageNothing(source);
		Destroy(gameObject);
	}
	
}
