using System;
using UnityEngine;

public class Shot : MonoBehaviour {
	public Game.Shot shotType;
		
	private Play play;
	
	private int damage;
	
	public void Initialize(Play p, int damage_) {
		play = p;
		damage = damage_;
	}
	
	void Start() {
		Invoke ("DestroySelf", 10.0f);
	}
	
	void OnCollisionEnter(Collision c) {
		CancelInvoke("DestroySelf");
		if (c.collider.tag == Ship.TAG) {
			play.DamageShip(damage);
//			Debug.Log ("HIT Ship");
		} else if (c.collider.tag == Enemy.TAG) {
			play.DamageEnemy(damage, c.collider.GetComponent<Enemy>(), c.contacts[0].point);
		}
		Destroy(gameObject);
	}	

	private void DestroySelf() {
		Destroy(gameObject);
	}
	
}
