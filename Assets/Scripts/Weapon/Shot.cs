using System;
using UnityEngine;

public class Shot : MonoBehaviour {
	public static string TAG = "Shot";
	
	public int type;	
	private Play play;
	private Enemy enemy;
	
	public int damage;
	private int source;
	
	private Transform target;
	
	public static int BULLET = 0;
	public static int LASER = 1;
	public static int MISSILE = 2;
	public static int GUIDED_MISSILE = 3;
	public static int MINE_TOUCH = 4;
	public static int LASER_BEAM = 5;
	public static int PHASER = 6;
	public static int GAUSS = 7;
	public static int CHARGED_MISSILE = 8;
	public static int DETONATOR_MISSILE = 9;
	public static int DETONATOR_BOMB = 10;
	
	private static float MISSILE_RADIUS = RoomMesh.MESH_SCALE * 2.5f;
	private static float GUIDED_MISSILE_TORQUE_MAX = 0.02f;
	private static float MISSILE_SPEED = 5.0f;
	private static float MINE_EXPLOSION_POWER = 500.0f;
	private static float MINE_EXPLOSION_RADIUS = 2.0f * RoomMesh.MESH_SCALE;
	
	public void Initialize(Play p, int damage_, int source_, int type_) {
		play = p;
		damage = damage_;
		source = source_;
		type = type_;
	}
	
	void Start() {
		if (type == BULLET) {
			Invoke ("DestroySelf", 15.0f);
		} else if (type == DETONATOR_BOMB) {
			Invoke ("DestroySelf", 2.5f);
		}
	}
	
	void FixedUpdate() {
		if (type == MISSILE || type == GUIDED_MISSILE || type == CHARGED_MISSILE || type == DETONATOR_MISSILE) {
			if (type == GUIDED_MISSILE && target != null) {
	//			Vector3 pos = new Vector3(137f, 61f, 52f);
				rigidbody.AddTorque(Vector3.Cross(transform.forward, (target.position - transform.position)).normalized * GUIDED_MISSILE_TORQUE_MAX);
	//			Debug.Log ("correcting flight " + corrected.magnitude);
			}
			rigidbody.AddForce(transform.forward * MISSILE_SPEED);
		}
	}
	
	void OnCollisionEnter(Collision c) {
		if (type == MINE_TOUCH) {
			if (c.collider.tag == Ship.TAG || c.collider.tag == Shot.TAG) {
				play.DisplayExplosion(transform.position, play.ship.transform.rotation);
				c.rigidbody.AddExplosionForce(MINE_EXPLOSION_POWER, transform.position, MINE_EXPLOSION_RADIUS);
				if (c.collider.tag == Ship.TAG) {
					play.ship.Damage(damage);
					play.DamageShip(source);
				}
				((MineBuilder)enemy).MineDestroyed();
				Destroy(gameObject);
			}
		} else if (type != LASER_BEAM) {
/*			if (c.collider.tag == Ship.TAG) {
				play.DisplayHit(c.contacts[0].point, play.ship.transform.rotation);
				play.ship.Damage(damage);
				play.DamageShip(source);
			}
		} else {*/
			CancelInvoke("DestroySelf");
			if (type == MISSILE || type == GUIDED_MISSILE || type == CHARGED_MISSILE || type == DETONATOR_MISSILE) {
				play.DisplayExplosion(c.contacts[0].point, play.ship.transform.rotation);
				if (type == DETONATOR_MISSILE) {
					ReleaseDetonatorBombShots();
				} else {
					ApplyCollateralDamage(c.contacts[0].point);
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
	}
	
	private void ReleaseDetonatorBombShots() {
		play.ship.isDetonatorMissileExploded = true;
		for (int i=0; i<4; i++) {
			Shot s = play.game.CreateFromPrefab().CreateDetonatorBombShot(transform.position + transform.TransformDirection(Weapon.DETONATOR_BOMB_DIRECTIONS[i]), transform.rotation, damage, Game.SHIP);
			s.rigidbody.AddExplosionForce(10f, transform.position + transform.TransformDirection(Weapon.DETONATOR_EXPL_DIRECTIONS[i]), 0f);
		}
	}
	
	private void ApplyCollateralDamage(Vector3 pos) {
		Collider[] missileHits = Physics.OverlapSphere(pos, MISSILE_RADIUS, Game.LAYER_MASK_MOVEABLES);
		foreach (Collider col in missileHits) {
			if (col.gameObject.layer == Game.LAYER_ENEMIES) {
				col.GetComponent<Enemy>().Damage(Mathf.RoundToInt(damage/2.0f));
			} else {
				play.ship.Damage(Mathf.RoundToInt(damage/4.0f));
			}
			play.DisplayHit(col.transform.position, play.ship.transform.rotation);
		}
	}
	
	public void ExecuteLaserBeamDamage(Vector3 point) {
		play.DisplayHit(point, play.ship.transform.rotation);
		play.ship.Damage(damage);
		play.DamageShip(source);
	}
	
	public void Detonate() {
		CancelInvoke("DestroySelf");
		play.DisplayExplosion(transform.position, play.ship.transform.rotation);
		ReleaseDetonatorBombShots();
		Destroy(gameObject);
	}
	
	// mine is not a rigidbody
/*	private void OnTriggerEnter(Collider c) {
		if (type == MINE_TOUCH) {
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
			c.rigidbody.AddExplosionForce(MINE_EXPLOSION_POWER, transform.position, MINE_EXPLOSION_RADIUS);
			play.ship.Damage(damage);
			play.DamageShip(source);
		}
		Destroy(gameObject);
	}*/
	
	public void LockOn(Transform target_) {
		target = target_;
	}
	
	public void SetParentEnemy(Enemy e) {
		enemy = e;
	}
	
	private void DestroySelf() {
		if (type == BULLET) {
			play.DamageNothing(source);
		} else if (type == DETONATOR_BOMB) {
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
			ApplyCollateralDamage(transform.position);
		}
		Destroy(gameObject);
	}
	
}
