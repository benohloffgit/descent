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
	public static string CLAZZ_K = "k";
	public static string CLAZZ_L = "l";
	public static int CLAZZ_A0 = 0; // bull
	public static int CLAZZ_B1 = 1; // spile
	public static int CLAZZ_C2 = 2;
	public static int CLAZZ_D3 = 3;
	public static int CLAZZ_E4 = 4;
	public static int CLAZZ_F5 = 5;
	public static int CLAZZ_G6 = 6;
	public static int CLAZZ_H7 = 7;
	public static int CLAZZ_K8 = 8;
	public static int CLAZZ_L9 = 9;
	
	public static string[] CLAZZES = new string[] {CLAZZ_A, CLAZZ_B, CLAZZ_C, CLAZZ_D, CLAZZ_E, CLAZZ_F, CLAZZ_G, CLAZZ_H, CLAZZ_K, CLAZZ_L};
	
	public static int CLAZZ_STEP = 100;
	public static int MODEL_MIN = 0;
	public static int MODEL_MAX = 98;
	public static int CLAZZ_MIN = 0;
	public static int CLAZZ_MAX = 9;
	
	public Game game;
	public Play play;
	private Spawn spawn;
	
	public string clazz; // (A-L)
	public int clazzNum; // 0-9 
	public int model; // 0-98 (1-99)
	public int modelNum; // 0 - 998
	public int health;
	public int shield;
	public float firepowerPerSecond;
	protected float size;
	protected float aggressiveness; // between 0 (no at all) and 1.0 (attacks 100% of time)
	protected float movementForce;
	protected float turningForce;
	protected int lookAtRange;
	protected float chaseRange;
	protected float shootingRange;
	protected float chasingRange;
	protected int roamMinRange;
	protected int roamMaxRange;
	protected float lookAtToleranceAiming;
	protected float lookAtToleranceRoaming;
	
	protected List<Weapon> weapons = new List<Weapon>();

	protected Rigidbody myRigidbody;
	
	public abstract void InitializeWeapon(int ix, int w, int m);
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
	}

	public void Initialize(Play play_, Spawn spawn_, int clazzNum_, int model_, int health_, int shield_,
			float size_, float aggressiveness_, float movementForce_,
			float turningForce_, int lookAtRange_, int chaseRange_,
			int roamMinRange_, int roamMaxRange_, int[] weapons_, int[] models_) {		
		play = play_;
		spawn = spawn_;
		game = play.game;
		clazzNum = clazzNum_;
		clazz = CLAZZES[clazzNum];
		model = model_;
		modelNum = clazzNum * CLAZZ_STEP + model;
		health = health_;
		shield = shield_;
		size = size_;
		aggressiveness = aggressiveness_;
		movementForce = movementForce_;
		turningForce = turningForce_;
		lookAtRange = lookAtRange_;
		chaseRange = chaseRange_;
		roamMinRange = roamMinRange_;
		roamMaxRange = roamMaxRange_;
		
		firepowerPerSecond = 0;
		for (int i=0; i<weapons_.Length; i++) {
			InitializeWeapon(i, weapons_[i], models_[i]);
			firepowerPerSecond += weapons[i].damage / weapons[i].frequency;
		}
		
		// derived values
		shootingRange = RoomMesh.MESH_SCALE * lookAtRange;
		chasingRange = RoomMesh.MESH_SCALE * chaseRange;
		lookAtToleranceAiming = 0.5f;
		lookAtToleranceRoaming = 20.0f;
		
		transform.localScale *= size;
	}
	
	public void Initialize(Play play_, Spawn spawn_, string clazz_, int model_, int health_, int shield_,
			float size_, float aggressiveness_, float movementForce_,
			float turningForce_, int lookAtRange_, int chaseRange_,
			int roamMinRange_, int roamMaxRange_, int[] weapons_, int[] models_) {
		Initialize(play_, spawn_, CLAZZ_NUM(clazz_), model_, health_, shield_,
			size_, aggressiveness_, movementForce_,
			turningForce_, lookAtRange_, chaseRange_,
			roamMinRange_, roamMaxRange_, weapons_, models_);		
	}
	
	public void Damage(int damage, Vector3 contactPos) {
		play.DisplayHit(contactPos, play.ship.transform.rotation);

		if (shield > 0) {
			shield -= damage * 2;
			if (shield < 0) {
				damage = Mathf.Abs(shield);
				shield = 0;
			} else {
				damage = 0;
			}
		}
		
		if (health-damage <= 0) {
			spawn.LoseHealth(health);
			if (spawn != null) {
				spawn.Die(this);
			}
			Destroy(gameObject);
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
		} else {
			spawn.LoseHealth(damage);
			health -= damage;
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
	
	public static int CLAZZ_NUM(string clazz_) {
		if (clazz_ == CLAZZ_A) {
			return 0;
		} else if (clazz_ == CLAZZ_B) {
			return 1;
		} else if (clazz_ == CLAZZ_C) {
			return 2;
		} else if (clazz_ == CLAZZ_D) {
			return 3;
		} else if (clazz_ == CLAZZ_E) {
			return 4;
		} else if (clazz_ == CLAZZ_F) {
			return 5;
		} else if (clazz_ == CLAZZ_G) {
			return 6;
		} else if (clazz_ == CLAZZ_H) {
			return 7;
		} else if (clazz_ == CLAZZ_K) {
			return 8;
		} else if (clazz_ == CLAZZ_L) {
			return 9;
		}
		return -1;
	}

}
