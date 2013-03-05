using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {
	public static string TAG = "Enemy";
	
	public Play play;
	public int health;
	
//	public abstract void Damage(int damage);
	
	public void Damage(int damage, Vector3 contactPos) {
		health -= damage;
		play.DisplayHit(contactPos, play.ship.transform.rotation);
		if (health <= 0) {
			Destroy(gameObject);
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
		}
	}

}
