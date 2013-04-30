using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour {
	public static string TAG = "Enemy";
	
	/*   Wall gunner
		Light bulb	
		Mine dropper
	*/
	public static string CLAZZ_A = "a"; // bull
	public static string CLAZZ_B = "b"; // spike
	public static string CLAZZ_C = "c";
	public static string CLAZZ_D = "d";
	public static string CLAZZ_E = "e";
	public static string CLAZZ_F = "f";
	public static string CLAZZ_G = "g";
	public static string CLAZZ_H = "h";
	
	public static string CLAZZ_BUG = "z";
	public static string CLAZZ_SNAKE = "y";
	public static string CLAZZ_MINEBUILDER = "x";
	
//	public static string CLAZZ_K = "k";
//	public static string CLAZZ_L = "l";
	public static int CLAZZ_A0 = 0; // bull
	public static int CLAZZ_B1 = 1; // spike
	public static int CLAZZ_C2 = 2;
	public static int CLAZZ_D3 = 3;
	public static int CLAZZ_E4 = 4;
	public static int CLAZZ_F5 = 5;
	public static int CLAZZ_G6 = 6;
	public static int CLAZZ_H7 = 7;
	public static int CLAZZ_BUG8 = 8;
	public static int CLAZZ_SNAKE9 = 9;
	public static int CLAZZ_MINEBUILDER10 = 10;
//	public static int CLAZZ_K8 = 8;
//	public static int CLAZZ_L9 = 9;
	
	public static string[] CLAZZES = new string[] {CLAZZ_A, CLAZZ_B, CLAZZ_C, CLAZZ_D, CLAZZ_E, CLAZZ_F, CLAZZ_G, CLAZZ_H, CLAZZ_BUG, CLAZZ_SNAKE, CLAZZ_MINEBUILDER};
	
//	public static int CLAZZ_STEP = 99;
	public static int MODEL_MIN = 0;
	public static int MODEL_MAX = 63;
	public static int CLAZZ_MIN = 0;
	public static int CLAZZ_MAX = 7;
	public static int DISTRIBUTION_CLAZZ_MAX = 8; // 8 clazzes
	public static float AGGRESSIVENESS_ON = 1.0f;
	public static float AGGRESSIVENESS_OFF = 0f;
	public static float AGGRESSIVENESS_DECREASE = 0.0016f;
	
	public static float BOSS_DAMAGE_MODIFIER = 2.0f;
	public static float BOSS_ACCURACY_MODIFIER = 1.0f;
	public static float BOSS_FREQUENCY_MODIFIER = 1.0f;
	
	private static float DEACTIVATION_TIME = 5.0f;
	
	public Game game;
	public Play play;
	protected Spawn spawn;
	
	public string clazz; // (A-L)
	public int clazzNum; // 0-7 
	public int model; // 0-98 (1-99)
	public int displayModel;
//	public int modelNum; // 0 - 998
	public int modelClazzAEquivalent;
	public int health;
	public int shield;
	public float firepowerPerSecond;
	protected float size;
	protected float aggressiveness; // between 0 (no at all) and 1.0 (attacks 100% of time)
	protected float movementForce;
	protected float turningForce;
	protected int lookAtRange;
//	protected float chaseRange;
	protected float shootingRange;
	protected float chasingRange;
	protected int roamMinRange;
	protected int roamMaxRange;
	protected float lookAtToleranceAiming;
	protected float lookAtToleranceRoaming;
	public int currentPrimaryWeapon;
	public int currentSecondaryWeapon;
	
	public bool isActive;
	private float lastTimeShipVisible;
	public float lastTimeHUDInfoRequested;
	public float radius;
	protected bool canBeDeactivated;
	
	public List<Weapon> primaryWeapons = new List<Weapon>();
	public List<Weapon> secondaryWeapons = new List<Weapon>();

	protected Rigidbody myRigidbody;
	
	public abstract void InitializeWeapon(int ix, int w, int m);
	public abstract void DispatchFixedUpdate(Vector3 isShipVisible);
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
		isActive = false;
		radius = collider.bounds.extents.magnitude;
//		Debug.Log (radius);
	}

	public void Initialize(Play play_, Spawn spawn_, int clazzNum_, int model_, int enemyEquivalentClazzAModel_, int health_, int shield_,
			float size_, float aggressiveness_, float movementForce_,
			float turningForce_, int lookAtRange_,
			int roamMinRange_, int roamMaxRange_) {		
		play = play_;
		spawn = spawn_;
		game = play.game;
		clazzNum = clazzNum_;
		clazz = CLAZZES[clazzNum];
		model = model_;
		displayModel = model +1;
//		modelNum = clazzNum * CLAZZ_STEP + model;
		modelClazzAEquivalent = enemyEquivalentClazzAModel_;
		health = spawn.isBoss ? health_ * 2 : health_;
		shield = spawn.isBoss ? shield_ * 2 : shield_;
		size = spawn.isBoss ? 2.5f : size_;
		aggressiveness = 0;//aggressiveness_;
		movementForce = movementForce_;
		turningForce = turningForce_;
		lookAtRange = lookAtRange_;
//		chaseRange = chaseRange_;
		roamMinRange = roamMinRange_;
		roamMaxRange = roamMaxRange_;
		
		canBeDeactivated = true;
		firepowerPerSecond = 0;
		
		if (clazzNum != CLAZZ_BUG8 && clazzNum != CLAZZ_SNAKE9 && clazzNum != CLAZZ_MINEBUILDER10) {
			InitializeWeapon(Weapon.PRIMARY, Weapon.SHIP_PRIMARY_WEAPON_TYPES[modelClazzAEquivalent], Weapon.SHIP_PRIMARY_WEAPON_MODELS[modelClazzAEquivalent]);
		
			firepowerPerSecond += primaryWeapons[0].damage / primaryWeapons[0].frequency;
		} else if (clazzNum == CLAZZ_MINEBUILDER10) {
			InitializeWeapon(Weapon.SECONDARY, Weapon.TYPE_MINE_TOUCH, modelClazzAEquivalent);
		}
		
		currentPrimaryWeapon = 0;
		currentSecondaryWeapon = 0;
		
		if (primaryWeapons.Count > 0) {
			primaryWeapons[currentPrimaryWeapon].Mount();
		}
		if (secondaryWeapons.Count > 0) {
			secondaryWeapons[currentSecondaryWeapon].Mount();
		}
		
		// derived values
		shootingRange = RoomMesh.MESH_SCALE * lookAtRange;
		chasingRange = RoomMesh.MESH_SCALE * (lookAtRange-2);//RoomMesh.MESH_SCALE * chaseRange;
		lookAtToleranceAiming = 0.5f;
		lookAtToleranceRoaming = 20.0f;
		
		transform.localScale *= size;
	}
	
	public void Initialize(Play play_, Spawn spawn_, string clazz_, int model_, int enemyEquivalentClazzAModel_, int health_, int shield_,
			float size_, float aggressiveness_, float movementForce_,
			float turningForce_, int lookAtRange_,
			int roamMinRange_, int roamMaxRange_) {
		
		Initialize(play_, spawn_, CLAZZ_NUM(clazz_), model_, enemyEquivalentClazzAModel_, health_, shield_,
			size_, aggressiveness_, movementForce_,
			turningForce_, lookAtRange_,
			roamMinRange_, roamMaxRange_);		
	}
	
	void FixedUpdate() {
		ShootSecondary();
		
		Vector3 isShipVisible = play.ship.IsVisibleFrom(transform.position);
		if (isShipVisible != Vector3.zero) {
			if (!isActive) {
				lastTimeShipVisible = Time.time;
				spawn.ActivateEnemy(this);
			}
			DispatchFixedUpdate(isShipVisible);
		} else if (isActive) {
			if (Time.time > lastTimeShipVisible + DEACTIVATION_TIME && canBeDeactivated) {
				spawn.DeactivateEnemy(this);
			} else if (!canBeDeactivated) {
				DispatchFixedUpdate(isShipVisible);
			}
		}
		if (aggressiveness > AGGRESSIVENESS_OFF) {
			if (isShipVisible != Vector3.zero) {
				ShootPrimary();
			}
			aggressiveness -= AGGRESSIVENESS_DECREASE;
		}
	}
	
	public void Damage(int damage) {
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
			if (spawn != null) {
				spawn.LoseHealth(this, health);
				spawn.Die(this);
			}
			health = 0;
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
			Destroy(gameObject);
		} else {
			spawn.LoseHealth(this, damage);
			health -= damage;
			aggressiveness = AGGRESSIVENESS_ON;
		}
	}
	
	protected void ShootPrimary() {
		if (aggressiveness > AGGRESSIVENESS_OFF) {
			if (primaryWeapons[currentPrimaryWeapon].IsReloaded()) {
				primaryWeapons[currentPrimaryWeapon].Shoot();
			}				
		} else {
			aggressiveness = AGGRESSIVENESS_OFF;
		}
	}
	
	protected void ShootSecondary() {
		if (secondaryWeapons.Count > 0 && secondaryWeapons[currentSecondaryWeapon].IsReloaded()) {
			secondaryWeapons[currentSecondaryWeapon].Shoot();
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
		} else if (clazz_ == CLAZZ_BUG) {
			return 8;
		} else if (clazz_ == CLAZZ_SNAKE) {
			return 9;
		} else if (clazz_ == CLAZZ_MINEBUILDER) {
			return 10;
		}
		return -1;
	}

}
