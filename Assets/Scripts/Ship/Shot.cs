using System;
using UnityEngine;

public class Shot : MonoBehaviour {
	public Game.Shot shotType;
	
	public static float SPEED = 100.0f;
	public static float LASER_SPEED = 400.0f;
	
	private Play play;
	
	private static int DAMAGE = 5;
	
	public void Initialize(Play p) {
		play = p;
	}
	
	void Start() {
		Invoke ("DestroySelf", 10.0f);
	}
	
	void OnCollisionEnter(Collision c) {
		CancelInvoke("DestroySelf");
		if (c.collider.tag == Ship.TAG) {
			play.DamageShip(DAMAGE);
//			Debug.Log ("HIT Ship");
		} else if (c.collider.tag == Enemy.TAG) {
			Debug.Log ("HIT Enemy");
			play.DamageEnemy(DAMAGE, c.collider.GetComponent<Enemy>(), c.contacts[0].point);
		}
		Destroy(gameObject);
	}	

	private void DestroySelf() {
		Destroy(gameObject);
	}
	
}
