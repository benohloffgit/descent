using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour {
	public static string TAG = "Enemy";
	
	public static string CLAZZ_A = "a"; // bull
	public static string CLAZZ_B = "b"; // spile
	public static string CLAZZ_C = "c";
	public static string CLAZZ_D = "d";
	public static string CLAZZ_E = "e";
	public static string CLAZZ_F = "f";
	public static string CLAZZ_G = "g";
	public static string CLAZZ_H = "h";
	
	public Game game;
	public Play play;
	//public EnemyDistributor enemyDistributor;
	private Spawn spawn;
	
	public string clazz;
	public int number;
	public int health;
	public int shield;
	protected float size;
	protected float aggressiveness; // between 0 (no at all) and 1.0 (attacks 100% of time)
	protected float movementForce;
	protected float turningForce;
	protected int lookAtRange;
	protected float chaseRange;
	protected float shootingRange;
	protected float chasingRange;
	protected float lookAtToleranceAiming;
	protected float lookAtToleranceRoaming;
	
	protected List<Weapon> weapons = new List<Weapon>();

	protected Rigidbody myRigidbody;
	
	public abstract void InitializeWeapon(int ix, int w, int m);
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
	}

	public void Initialize(Play play_, Spawn spawn_, string clazz_, int number_, int health_, int shield_,
			float size_, float aggressiveness_, float movementForce_,
			float turningForce_, int lookAtRange_, float lookAtToleranceAiming_, float lookAtToleranceRoaming_, int chaseRange_,
			int[] weapons_, int[] models_) {
//		enemyDistributor = enemyDistributor_;
		play = play_;
		spawn = spawn_;
		game = play.game;
		clazz = clazz_;
		number = number_;
		health = health_;
		shield = shield_;
		size = size_;
		aggressiveness = aggressiveness_;
		movementForce = movementForce_;
		turningForce = turningForce_;
		lookAtRange = lookAtRange_;
		chaseRange = chaseRange_;
		lookAtToleranceAiming = lookAtToleranceAiming_;
		lookAtToleranceRoaming = lookAtToleranceRoaming_;
		
		for (int i=0; i<weapons_.Length; i++) {
			InitializeWeapon(i, weapons_[i], models_[i]);
		}
		
		// derived values
		shootingRange = RoomMesh.MESH_SCALE * lookAtRange;
		chasingRange = RoomMesh.MESH_SCALE * chaseRange;
		
		transform.localScale *= size;
	}
	
	public void Damage(int damage, Vector3 contactPos) {
		Debug.Log (health);
		health -= damage;
		play.DisplayHit(contactPos, play.ship.transform.rotation);
		if (health <= 0) {
			if (spawn != null) {
				spawn.Die(this);
			}
			Destroy(gameObject);
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
		}
	}
	
	protected void Shoot() {
		if (aggressiveness > 0) {
			foreach (Weapon w in weapons) {
				if (Time.time > w.lastShotTime + w.frequency) {
					w.Shoot();
					w.lastShotTime = Time.time;
				}
				
			}
		}
	}	

}
