using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour {
	public static string TAG = "Enemy";
	
	public static string CLAZZ_A = "Spider"; // TYPE_GUN 
	public static string CLAZZ_B = "Bat"; // TYPE_LASER 
	public static string CLAZZ_C = "Gazelle"; // TYPE_TWIN_GUN
	public static string CLAZZ_D = "Wombat"; // NO PRIMARY / Missile
	public static string CLAZZ_E = "Manta"; // TYPE_TWIN_LASER
	public static string CLAZZ_F = "Pike"; // TYPE_GAUSS
	public static string CLAZZ_G = "Bull"; // TYPE_TWIN_PHASER
	public static string CLAZZ_H = "Rhino"; // TYPE_TWIN_GAUSS + Missile
	
	public static string CLAZZ_BUG = "Bug";
	public static string CLAZZ_SNAKE = "Eel";
	public static string CLAZZ_MINEBUILDER = "Wasp";
	public static string CLAZZ_WALLLASER = "Cricket";
	public static string CLAZZ_HORNET = "Hornet";
	public static string CLAZZ_BULB = "Bulb";
	public static string CLAZZ_WALLGUN = "Cricket";
	
	public static int CLAZZ_A0 = 0; // spider
	public static int CLAZZ_B1 = 1; // bat
	public static int CLAZZ_C2 = 2; // gazelle
	public static int CLAZZ_D3 = 3; // wombat
	public static int CLAZZ_E4 = 4; // manta
	public static int CLAZZ_F5 = 5; // pike
	public static int CLAZZ_G6 = 6; // bull
	public static int CLAZZ_H7 = 7; // rhino
	public static int CLAZZ_BUG8 = 8;
	public static int CLAZZ_SNAKE9 = 9;
	public static int CLAZZ_MINEBUILDER10 = 10;
	public static int CLAZZ_WALLLASER11 = 11;
	public static int CLAZZ_HORNET12 = 12;
	public static int CLAZZ_BULB13 = 13;
	public static int CLAZZ_WALLGUN14 = 14;
	
	public static string[] CLAZZES = new string[] {CLAZZ_A, CLAZZ_B, CLAZZ_C, CLAZZ_D, CLAZZ_E, CLAZZ_F, CLAZZ_G, CLAZZ_H, CLAZZ_BUG, CLAZZ_SNAKE, CLAZZ_MINEBUILDER, CLAZZ_WALLLASER, CLAZZ_HORNET, CLAZZ_BULB, CLAZZ_WALLGUN};
	
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
	protected Cave cave;
	protected Spawn spawn;
	
	public string clazz; // name
	public int clazzNum; // 0-7 
	public int model; // 0-98 (1-99)
	public int displayModel;
	public int modelClazzAEquivalent;
	public int health;
	public int maxHealth;
	public float firepowerPerSecond;
	protected float size;
	protected float aggressiveness; // between 0 (no at all) and 1.0 (attacks 100% of time)
	protected float movementForce;
	protected float turningForce;
	protected int lookAtRange;
	protected float shootingRange;
	protected float chasingRange;
	protected int roamMinRange;
	protected int roamMaxRange;
	protected float lookAtToleranceAiming;
	protected float lookAtToleranceRoaming;
	protected float currentAngleUp;
	protected float dotProductLookAt;
	public int currentPrimaryWeapon;
	public int currentSecondaryWeapon;
	public bool flaggedForDestruction;
	
	public bool isActive;
	private float lastTimeShipVisible;
	public float lastTimeHUDInfoRequested;
	public float radius;
	protected bool canBeDeactivated;
	public GridPosition currentGridPosition;
	
	private Renderer myRenderer;
	
	public List<Weapon> primaryWeapons = new List<Weapon>();
	public List<Weapon> secondaryWeapons = new List<Weapon>();

	protected Rigidbody myRigidbody;
	
	public abstract void InitializeWeapon(int weaponMount, int weaponType);
	public abstract void DispatchFixedUpdate(Vector3 isShipVisible);
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
		isActive = false;
	}
	
	void OnEnable() {
		myRenderer = GetComponentInChildren<Renderer>();
	}

	public void Initialize(Play play_, Spawn spawn_, int clazzNum_, int model_, int enemyEquivalentClazzAModel_, int health_,
			float size_, float aggressiveness_, float movementForce_,
			float turningForce_, int lookAtRange_,
			int roamMinRange_, int roamMaxRange_) {
		play = play_;
		cave = play.cave;
		spawn = spawn_;
		game = play.game;
		clazzNum = clazzNum_;
		clazz = CLAZZES[clazzNum];
		model = model_;
		displayModel = model + 1;
		modelClazzAEquivalent = enemyEquivalentClazzAModel_;
		health = spawn.isBoss ? health_ * 2 : health_;
		maxHealth = health;
		size = spawn.isBoss ? 2.5f : size_;
		aggressiveness = 0;//aggressiveness_;
		movementForce = movementForce_;
		turningForce = turningForce_;
		lookAtRange = lookAtRange_;
		roamMinRange = roamMinRange_;
		roamMaxRange = roamMaxRange_;
		currentAngleUp = 0;
		dotProductLookAt = 0;
		
		flaggedForDestruction = false;
		canBeDeactivated = true;
		firepowerPerSecond = 0;
		
		if (clazzNum < DISTRIBUTION_CLAZZ_MAX) {
			InitializeWeapon(Weapon.PRIMARY, clazzNum);
			InitializeWeapon(Weapon.SECONDARY, clazzNum);
		} else if (clazzNum == CLAZZ_MINEBUILDER10) {
			InitializeWeapon(Weapon.SECONDARY, Weapon.TYPE_MINE_TOUCH);
		} else if (clazzNum == CLAZZ_WALLLASER11) {
			InitializeWeapon(Weapon.SECONDARY, Weapon.TYPE_LASER_BEAM);
		} else if (clazzNum == CLAZZ_HORNET12) {
			InitializeWeapon(Weapon.SECONDARY, Weapon.TYPE_MINE_SUICIDAL);
		} else if (clazzNum == CLAZZ_WALLGUN14) {
			InitializeWeapon(Weapon.PRIMARY, Weapon.TYPE_LASER);
		}
		
		currentPrimaryWeapon = 0;
		currentSecondaryWeapon = 0;
		
		if (primaryWeapons.Count > 0) {
			primaryWeapons[currentPrimaryWeapon].Mount();
			firepowerPerSecond += primaryWeapons[0].damage / primaryWeapons[0].frequency;
		}
		if (secondaryWeapons.Count > 0) {
			secondaryWeapons[currentSecondaryWeapon].Mount();
		}
		
		// derived values
		shootingRange = RoomMesh.MESH_SCALE * lookAtRange;
		chasingRange = RoomMesh.MESH_SCALE * (lookAtRange-2);
		lookAtToleranceAiming = 0.5f;
		lookAtToleranceRoaming = 20.0f;
		
		transform.localScale *= size;
		radius = collider.bounds.extents.magnitude;
	}
	
	public void Initialize(Play play_, Spawn spawn_, string clazz_, int model_, int enemyEquivalentClazzAModel_, int health_,
			float size_, float aggressiveness_, float movementForce_,
			float turningForce_, int lookAtRange_,
			int roamMinRange_, int roamMaxRange_) {
		
		Initialize(play_, spawn_, CLAZZ_NUM(clazz_), model_, enemyEquivalentClazzAModel_, health_,
			size_, aggressiveness_, movementForce_,
			turningForce_, lookAtRange_,
			roamMinRange_, roamMaxRange_);		
	}
	
	public void DispatchFixedUpdateGeneral() {
		currentGridPosition = cave.GetGridFromPosition(transform.position);
		Vector3 isShipVisible = play.ship.IsVisibleFrom(transform.position);
		if (myRenderer.isVisible || isShipVisible != Vector3.zero || play.GetShipGridPosition().roomPosition == currentGridPosition.roomPosition) {
			if (!isActive) {
				lastTimeShipVisible = Time.fixedTime;
				spawn.ActivateEnemy(this);
			}
			DispatchFixedUpdate(isShipVisible);
		} else if (isActive) {
			if (canBeDeactivated) {
				if (Time.fixedTime > lastTimeShipVisible + DEACTIVATION_TIME && play.GetShipGridPosition().roomPosition != currentGridPosition.roomPosition) {
					spawn.DeactivateEnemy(this);
				}
			} else {
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
	
	public void SetPaused(bool toPause) {
		if (toPause) {
		} else {
		}
	}
		
	public void Damage(int damage) {
		if (health-damage <= 0) {
			if (!flaggedForDestruction) {
				if (spawn != null) {
					spawn.LoseHealth(this, health);
					spawn.Die(this);
				}
				health = 0;
				play.DisplayExplosion(transform.position, play.ship.transform.rotation);
				flaggedForDestruction = true; // this is necessary because the following Destroy does not get executed immediately which leads to problems with twin projectiles
				play.RemoveEnemyOnDeath(this);
				Destroy(gameObject);
			}
		} else {
			spawn.LoseHealth(this, damage);
			health -= damage;
			aggressiveness = AGGRESSIVENESS_ON;
		}
	}
	
	protected void ShootPrimary() {
		if (primaryWeapons.Count > 0 && aggressiveness > AGGRESSIVENESS_OFF) {
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
	
	public void RenderWithGlow() {
		myRenderer.material = game.enemyMaterials[Game.MATERIAL_GLOW];
	}
	
	public void RenderNormal() {
		myRenderer.material = game.enemyMaterials[Game.MATERIAL_NO_GLOW];
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
		} else if (clazz_ == CLAZZ_WALLLASER) {
			return 11;
		} else if (clazz_ == CLAZZ_HORNET) {
			return 12;
		} else if (clazz_ == CLAZZ_BULB) {
			return 13;
		} else if (clazz_ == CLAZZ_WALLGUN) {
			return 14;
		}
		return -1;
	}
	
	void OnDisable() {
		AudioSourcePool.DecoupleAudioSource(GetComponentsInChildren<PooledAudioSource>());
	}

}
