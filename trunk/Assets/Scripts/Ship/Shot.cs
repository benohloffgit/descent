using System;
using UnityEngine;

public class Shot : MonoBehaviour {
	public int type;	
	private Play play;
	
	private int damage;
	private int source;
	
	public static int BULLET = 0;
	public static int MISSILE = 1;
	
	private static float MISSILE_RADIUS = RoomMesh.MESH_SCALE * 2.5f;
	
	public void Initialize(Play p, int damage_, int source_, int type_) {
		play = p;
		damage = damage_;
		source = source_;
		type = type_;
	}
	
	void Start() {
		Invoke ("DestroySelf", 10.0f);
	}
	
	void OnCollisionEnter(Collision c) {
		CancelInvoke("DestroySelf");
		if (type == MISSILE) {
			play.DisplayExplosion(c.contacts[0].point, play.ship.transform.rotation);
			Collider[] missileHits = Physics.OverlapSphere(c.contacts[0].point, MISSILE_RADIUS, 1 << Game.LAYER_ENEMIES);
			foreach (Collider col in missileHits) {
				col.GetComponent<Enemy>().Damage(Mathf.RoundToInt(damage/2.0f));
				play.DisplayHit(col.transform.position, play.ship.transform.rotation);
			}
		} else {
			play.DisplayHit(c.contacts[0].point, play.ship.transform.rotation);
		}
		
		if (c.collider.tag == Ship.TAG) {
			play.ship.Damage(damage);
			play.DamageShip(source);
		} else if (c.collider.tag == Enemy.TAG) {
			c.collider.GetComponent<Enemy>().Damage(damage);
			play.DamageEnemy(source);
		} else {
			if (type == BULLET) {
				play.DamageNothing(source);
			}
		}
		Destroy(gameObject);
	}	
	
	private void DestroySelf() {
		play.DamageNothing(source);
		Destroy(gameObject);
	}
	
}
