using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour {
	public static string TAG = "Ship";
	
	public GameObject shipHUDPrefab;
	public Game game;
	
	public int health;
	public int shield;
	public int healthPercentage;
	public int shieldPercentage;
	private int maxHealth;
	private int maxShield;
	public int currentPrimaryWeapon;
	public int currentSecondaryWeapon;
	
	public float firepowerPerSecond;
	public float lastMoveTime;
		
	private Play play;
	private GameInput gameInput;
	
	// ship components
	private GameObject shipHUD;
	private ShipSteering shipSteering;
	private ShipControl shipControl;
	private Transform headlight;
	private RaycastHit hit;
	private Transform cameraTransform;
	public Camera shipCamera;
	
	private bool isHeadlightOn;
	private int cameraPosition;
	public MissileLockMode missileLockMode;
	public Enemy lockedEnemy;
	public float missileLockTime;
		
	private static float FORCE_MOVE = 25.0f;
	private static float FORCE_TURN = 5.0f;
	private static float FORCE_YAW = 3.5f;
	
//	private static int HEALTH = 200;
//	private static int SHIELD = 50;
	
	private static Vector3[] CAMERA_POSITIONS = new Vector3[] {Vector3.zero, new Vector3(0, 3.0f, -12.0f), new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f), new Vector3(0, 0f, 5.0f), new Vector3(0, 12f, 0f)};
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(-1.014f, 0f, 1.664f), new Vector3(1.014f, 0f, 1.664f), new Vector3(0, -0.37f, 1.65f)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] { new Vector3(0,0,90f),  new Vector3(0,0,-90f),  new Vector3(0,0,180f)};
	
	public static int WEAPON_POSITION_WING_LEFT = 0;
	public static int WEAPON_POSITION_WING_RIGHT = 1;
	public static int WEAPON_POSITION_CENTER = 2;
	
	private static int CAMERA_POSITION_COCKPIT = 0;
	private static int CAMERA_POSITION_BEHIND = 1;
	private static int CAMERA_POSITION_LEFT = 2;
	private static int CAMERA_POSITION_RIGHT = 3;
	private static int CAMERA_POSITION_FRONT = 4;
		
	public Weapon[] primaryWeapons = new Weapon[8];
	public Weapon[] secondaryWeapons = new Weapon[8];
	
	public enum MissileLockMode { None=0, Aiming=1, Locked=2 }
	
	public static int MISSILE_LOCK_DURATION = 2;
	
//	private Vector3 collisionPoint = Vector3.zero;
//	private Vector3 collisionNormal = Vector3.zero;
	
	void Awake() {
//		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
//		Debug.Log (Screen.dpi);
		
		InstantiateShipHUD();
		shipSteering = transform.GetComponent<ShipSteering>();
		shipControl = new ShipControl(); //transform.GetComponent<ShipControl>();
		headlight = transform.FindChild("Headlight");
		cameraTransform = transform.FindChild("Camera");
		shipCamera = cameraTransform.GetComponent<Camera>();
	}
	
	void Start() {
		play.playGUI.DisplayPrimaryWeapon(primaryWeapons[currentPrimaryWeapon]);
		play.playGUI.DisplaySecondaryWeapon(secondaryWeapons[currentSecondaryWeapon]);
	}
	
	public void Initialize(Play p, Game g) {
		play = p;
		game = g;
		gameInput = game.gameInput;
		
		shipSteering.Initialize(this, play, gameInput);
		shipControl.Initialize(this, game, play, gameInput);
		
		isHeadlightOn = true;
		SwitchHeadlight();
		cameraPosition = CAMERA_POSITION_COCKPIT;
		maxHealth = CalculateHealth();
		maxShield = maxHealth / 2;
		health = maxHealth;
		shield = maxShield;
		healthPercentage = 100;
		shieldPercentage = 100;
		missileLockMode = MissileLockMode.None;
		
		AddWeapons();
		
		lastMoveTime = Time.time;
	}
	
	void FixedUpdate() {
		if (secondaryWeapons[currentSecondaryWeapon] != null) {
			secondaryWeapons[currentSecondaryWeapon].IsReloaded();
		}
		// Enemy target info
		if (Physics.Raycast(transform.position, transform.forward, out hit, Game.MAX_VISIBILITY_DISTANCE, Game.LAYER_MASK_ENEMIES_CAVE)) {
			if (hit.collider.tag == Enemy.TAG) {
				Enemy e = hit.transform.GetComponent<Enemy>();
				play.playGUI.EnemyInSight(e);
				if (	(secondaryWeapons[currentSecondaryWeapon].type == Weapon.TYPE_GUIDED_MISSILE
						|| secondaryWeapons[currentSecondaryWeapon].type == Weapon.TYPE_DETONATOR_MISSILE)
						&& secondaryWeapons[currentSecondaryWeapon].ammunition > 0
					) {
					if (missileLockMode == MissileLockMode.None || lockedEnemy != e) {
//						Debug.Log ("Aiming at enemy");
						lockedEnemy = e;
						missileLockTime = Time.time;
						missileLockMode = MissileLockMode.Aiming;
					} else {
						if (missileLockMode == MissileLockMode.Aiming && Time.time > missileLockTime + MISSILE_LOCK_DURATION) {
//							Debug.Log ("Enemy locked after 3 seconds");
							missileLockMode = MissileLockMode.Locked;
						}
					}
				}
			} else if (missileLockMode != MissileLockMode.None) {
				missileLockMode = MissileLockMode.None;
//				Debug.Log ("Lock lost");
			}
		}
	}
	
	void OnCollisionEnter(Collision c) {
		//Move(c.impactForceSum*5f);
		 //Vector3.Dot(col.contacts[0].normal,col.relativeVelocity) * rigidbody.mass
//		rigidbody.freezeRotation = true;
//    	Debug.Log("First point that collided: " + c.contacts[0].normal + " / " + c.contacts[0].point);
//		collisionPoint = c.contacts[0].point;
//		collisionNormal = c.contacts[0].normal;
	}
	
	void OnTriggerEnter(Collider collider) {
//		Debug.Log ("OnTriggerEnter");
	}
	
	void OnTriggerExit(Collider collider) {
//		Debug.Log ("OnTriggerExit");
	}

	public void DispatchGameInput() {
		shipSteering.DispatchGameInput();
		shipControl.DispatchGameInput();
	}
	
	public void Move(Vector3 direction) {
		rigidbody.AddRelativeForce(direction * FORCE_MOVE);
		lastMoveTime = Time.time;
	}
	
	public void Turn(Vector3 direction) {
		rigidbody.AddRelativeTorque(direction * FORCE_TURN);
	}

	public void Yaw(Vector3 direction) {
		rigidbody.AddRelativeTorque(direction * FORCE_YAW);
	}
	
	public void Damage(int damage) {
		if (shield > 0) {
			shield -= damage;
			if (shield < 0) {
				damage = Mathf.Abs(shield);
				shield = 0;
			} else {
				damage = 0;
			}
		}
		health -= damage;
		healthPercentage = Mathf.CeilToInt( (health/(float)maxHealth) * 100f);
		shieldPercentage = Mathf.CeilToInt( (shield/(float)maxShield) * 100f);
		if (health == 0) {
			// stop game
			Time.timeScale = 0f;
		}
		//Debug.Log (damage +" " + shield + " " +  maxShield+ " "+ health + " " + healthPercentage + " " +  shieldPercentage);
	}
	
	public void Heal(int amount) { // percentage of maxHealth
//		Debug.Log (amount + " " + Mathf.RoundToInt(maxHealth * ((float)amount/100f)) + " " + health);
		health = Mathf.Min(maxHealth, health + Mathf.RoundToInt(maxHealth * ((float)amount/100f)));
		healthPercentage = Mathf.CeilToInt( (health/(float)maxHealth) * 100f);
	}

	public void Shield(int amount) { // percentage of maxHealth
		shield = Mathf.Min(maxShield, shield + Mathf.RoundToInt(maxShield * ((float)amount/100f)));
		shieldPercentage = Mathf.CeilToInt( (shield/(float)maxShield) * 100f);
	}
	
	public Vector3 IsVisibleFrom(Vector3 fromPos) {
		Vector3 result = Vector3.zero;
		Vector3 direction = (play.GetShipPosition()-fromPos).normalized;
		if (Physics.Raycast(fromPos, direction, out hit, Game.MAX_VISIBILITY_DISTANCE, Game.LAYER_MASK_SHIP_CAVE)) {
			if (hit.collider.tag == TAG) {
				result = (play.GetShipPosition() - fromPos);
			}
		}				
		return result;
	}
	
	public void SwitchHeadlight() {
		isHeadlightOn = (isHeadlightOn) ? false : true;
		headlight.gameObject.SetActiveRecursively(isHeadlightOn);
	}
	
	public void CycleCamera() {
		cameraPosition++;
		
		if (cameraPosition == CAMERA_POSITIONS.Length) {
			cameraPosition = CAMERA_POSITION_COCKPIT;
		}
		cameraTransform.localPosition = CAMERA_POSITIONS[cameraPosition];

		if (cameraPosition == CAMERA_POSITION_COCKPIT) {
			shipCamera.cullingMask = Game.LAYER_MASK_CAMERA_WITHOUT_SHIP;
			cameraTransform.localRotation = Quaternion.identity;
		} else if (cameraPosition == CAMERA_POSITION_BEHIND) {
			shipCamera.cullingMask = Game.LAYER_MASK_CAMERA_WITH_SHIP;
			cameraTransform.localRotation = Quaternion.identity;
		} else {
			shipCamera.cullingMask = Game.LAYER_MASK_CAMERA_WITH_SHIP;
			cameraTransform.LookAt(transform, transform.up);
		}
		
	}
		
/*	private void Calibrate() {
		if (Input.acceleration != Vector3.zero) {
//			calibration = Quaternion.FromToRotation(Input.acceleration, new Vector3(0,0,-1.0f));
			calibration = Input.gyro.attitude;
			isCalibrated = true;
		}
//		Debug.Log ("Calibration " + Input.acceleration +" " + calibration);
	}*/

	private void InstantiateShipHUD() {
		shipHUD = GameObject.Instantiate(shipHUDPrefab) as GameObject;
		Transform crossHair = shipHUD.transform.Find("Cross Hair");
		crossHair.localScale /= (crossHair.renderer.material.mainTexture.width/(float)Screen.width) / (32.0f/(float)Screen.width);
	}
	
	private void AddWeapons() {
		int zone5 = Zone.GetZone5StepID(play.zoneID);
			
		// secondary weapons
//		int lastLowestTypePrimary = Weapon.SHIP_PRIMARY_WEAPON_TYPES[zone5] + 1;
//		int lastLowestTypeSecondary = Weapon.SHIP_SECONDARY_WEAPON_TYPES[zone5] + 1;
		for (int i=zone5; i >= 0; i--) {
			if (Weapon.SHIP_PRIMARY_WEAPON_TYPES[i] != 0 && primaryWeapons[Weapon.SHIP_PRIMARY_WEAPON_TYPES[i]-1] == null) {
				Weapon w = new Weapon(Weapon.PRIMARY, transform, play, Weapon.SHIP_PRIMARY_WEAPON_TYPES[i], Weapon.SHIP_PRIMARY_WEAPON_MODELS[i], WEAPON_POSITIONS[WEAPON_POSITION_WING_LEFT], Game.SHIP);
				primaryWeapons[Weapon.SHIP_PRIMARY_WEAPON_TYPES[i]-1] = w;
				w.weaponTransform.localEulerAngles = WEAPON_ROTATIONS[WEAPON_POSITION_WING_LEFT];
//				lastLowestTypePrimary = Weapon.SHIP_PRIMARY_WEAPON_TYPES[i];
				Debug.Log ("adding primary weapon type/model " + Weapon.SHIP_PRIMARY_WEAPON_TYPES[i]+"/"+Weapon.SHIP_PRIMARY_WEAPON_MODELS[i]);
			}	
			if (Weapon.SHIP_SECONDARY_WEAPON_TYPES[i] != 0 && secondaryWeapons[Weapon.SHIP_SECONDARY_WEAPON_TYPES[i]-1] == null) {
				Weapon w = new Weapon(Weapon.SECONDARY, transform, play, Weapon.SHIP_SECONDARY_WEAPON_TYPES[i], Weapon.SHIP_SECONDARY_WEAPON_MODELS[i], WEAPON_POSITIONS[WEAPON_POSITION_CENTER], Game.SHIP, 5);
				secondaryWeapons[Weapon.SHIP_SECONDARY_WEAPON_TYPES[i]-1] = w;
				w.weaponTransform.localEulerAngles = WEAPON_ROTATIONS[WEAPON_POSITION_CENTER];
//				lastLowestTypeSecondary = Weapon.SHIP_SECONDARY_WEAPON_TYPES[i];
				Debug.Log ("adding secondary weapon type/model " + Weapon.SHIP_SECONDARY_WEAPON_TYPES[i]+"/"+Weapon.SHIP_SECONDARY_WEAPON_MODELS[i]);
			}
		}
		
		currentPrimaryWeapon = Weapon.SHIP_PRIMARY_WEAPON_TYPES[zone5]-1;
		currentSecondaryWeapon = Weapon.SHIP_SECONDARY_WEAPON_TYPES[zone5]-1;
		primaryWeapons[currentPrimaryWeapon].Mount();
		firepowerPerSecond = primaryWeapons[currentPrimaryWeapon].damage;// / w1.frequency; // we assume 1 shot per second ALWAYS
		if (secondaryWeapons[0] != null) {
			secondaryWeapons[currentSecondaryWeapon].Mount();
		}
	}
	
	private int CalculateHealth() {
		int zone5 = Zone.GetZone5StepID(play.zoneID);
		
		return zone5 * 180;
	}
	
	public void ShootPrimary() {
		if (primaryWeapons[currentPrimaryWeapon].IsReloaded()) {
			primaryWeapons[currentPrimaryWeapon].Shoot();
		}
	}

	public void ShootSecondary() {
		if (secondaryWeapons[currentSecondaryWeapon] != null && secondaryWeapons[currentSecondaryWeapon].IsReloaded()) {
			secondaryWeapons[currentSecondaryWeapon].Shoot();
			play.playGUI.DisplaySecondaryWeapon(secondaryWeapons[currentSecondaryWeapon]);
		}
	}
	
	public void CyclePrimary() {
		primaryWeapons[currentPrimaryWeapon].Unmount();
		currentPrimaryWeapon++;
		if (currentSecondaryWeapon == 8 || primaryWeapons[currentPrimaryWeapon] == null) {
			currentPrimaryWeapon = 0;
		}
		primaryWeapons[currentPrimaryWeapon].Mount();
		firepowerPerSecond = primaryWeapons[currentPrimaryWeapon].damage;// / w1.frequency; // we assume 1 shot per second ALWAYS
		play.playGUI.DisplayPrimaryWeapon(primaryWeapons[currentPrimaryWeapon]);
	}

	public void CycleSecondary() {
		if (secondaryWeapons[currentSecondaryWeapon] != null) {
			secondaryWeapons[currentSecondaryWeapon].Unmount();
			currentSecondaryWeapon++;
			if (currentSecondaryWeapon == 4 || secondaryWeapons[currentSecondaryWeapon] == null) {
				currentSecondaryWeapon = 0;
			}
			secondaryWeapons[currentSecondaryWeapon].Mount();
			play.playGUI.DisplaySecondaryWeapon(secondaryWeapons[currentSecondaryWeapon]);
			if (secondaryWeapons[currentSecondaryWeapon].type != Weapon.TYPE_GUIDED_MISSILE && secondaryWeapons[currentSecondaryWeapon].type != Weapon.TYPE_DETONATOR_MISSILE) {
				missileLockMode = MissileLockMode.None;
			}
		}
	}
	public void RemoveEnemy(Enemy e) {
		if (missileLockMode != MissileLockMode.None && lockedEnemy == e) {
			lockedEnemy = null;
			missileLockMode = MissileLockMode.None;
		}
	}
		
}


