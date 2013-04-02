using System;
using UnityEngine;

public class Shot : MonoBehaviour {
	public int type;	
	private Play play;
	
	private int damage;
	private int source;
	
	private Transform target;
	
	public static int BULLET = 0;
	public static int MISSILE = 1;
	public static int GUIDED = 2;
	
	private static float MISSILE_RADIUS = RoomMesh.MESH_SCALE * 2.5f;
	private static float GUIDED_MISSILE_TORQUE_MAX = 0.02f;
	private static float MISSILE_SPEED = 5.0f;
	
	public void Initialize(Play p, int damage_, int source_, int type_) {
		play = p;
		damage = damage_;
		source = source_;
		type = type_;
	}
	
	void Start() {
		Invoke ("DestroySelf", 15.0f);
	}
	
	void FixedUpdate() {
		if (type != BULLET) {
			if (type == GUIDED && target != null) {
	//			Vector3 pos = new Vector3(137f, 61f, 52f);
				rigidbody.AddTorque(Vector3.Cross(transform.forward, (target.position - transform.position)).normalized * GUIDED_MISSILE_TORQUE_MAX);
	//			Debug.Log ("correcting flight " + corrected.magnitude);
			}
			rigidbody.AddForce(transform.forward * MISSILE_SPEED);
		}
	}
	
	void OnCollisionEnter(Collision c) {
		CancelInvoke("DestroySelf");
		if (type != BULLET) {
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
	
	public void LockOn(Transform target_) {
		target = target_;
	}
	
	private void DestroySelf() {
		play.DamageNothing(source);
		Destroy(gameObject);
	}
	
}
