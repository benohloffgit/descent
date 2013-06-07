using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour {
	public static string TAG = "Ship";
	
	public GameObject shipHUDPrefab;
	public Game game;
	
	public int hullCLazz;
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
	private ExitHelper exitHelper;
	private GameInput gameInput;
	private AudioSource audioSource;
	
	// ship components
	private GameObject shipHUD;
	private ShipSteering shipSteering;
	private ShipControl shipControl;
	private Transform headlight;
	private RaycastHit hit;
	private Transform cameraTransform;
	public Camera shipCamera;
	
	public bool isExitHelperLaunched;
	public bool isHeadlightOn;
	private int cameraPosition;
	public MissileLockMode missileLockMode;
	public Enemy lockedEnemy;
	public float missileLockTime;
	private float chargedMissileTimer;
	private int chargedMissileShieldDeducted;
	public bool isDetonatorMissileExploded;
			
	private static float FORCE_MOVE = 65.0f;
	private static float FORCE_TURN = 24f; // 5.0f
	private static float FORCE_YAW = 16f; // 3.5f
	
	private static int CHARGED_MISSILE_SHIELD_MAX = 50;
	private static float CHARGED_MISSILE_TIME_MAX = 2.5f; // seconds
	private static float CHARGED_MISSILE_DEDUCTION = 20f; // deducted shield per second
	
	private static Vector3[] CAMERA_POSITIONS = new Vector3[] {Vector3.zero, new Vector3(0, 3.0f, -12.0f), new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f), new Vector3(0, 0f, 5.0f), new Vector3(0, 12f, 0f)};
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(-1.014f, 0f, 1.664f), new Vector3(1.014f, 0f, 1.664f), new Vector3(0, -0.37f, 1.65f)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] { new Vector3(0,0,90f),  new Vector3(0,0,-90f),  new Vector3(0,0,180f)};
	
	public static int[] HEALTH = new int[] { 100, 112, 126, 135, 148, 162, 180, 200 };
	public static int[] SHIELD = new int[] { 45, 50, 56, 60, 66, 72, 80, 85 };
	public static int[] HULL_POWER_UP = new int[] {0,5,12,20,29,39,50,62};
	public static int[] SPECIAL_POWER_UP = new int[] {7,19,41,58};

	public static string[] HULL_TYPES = new string[] {"Armor", "Improved Armor", "Bla Armor", "Blubb Armor", "Keflar Armor", "Iridium Armor", "Nano Armor", "Bloob Armor"};
	public static string[] SPECIAL_TYPES = new string[] {"Light", "Boost", "Cloak", "Invincible"};
	
	private static int CAMERA_POSITION_COCKPIT = 0;
	private static int CAMERA_POSITION_BEHIND = 1;
	private static Vector3 EXIT_HELPER_LAUNCH_POSITION = new Vector3(0f, 0f, 2f);
		
	public Weapon[] primaryWeapons = new Weapon[8];
	public Weapon[] secondaryWeapons = new Weapon[4];
	
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
		audioSource = GetComponent<AudioSource>();
	}
	
	public void Initialize(Play play_, ExitHelper exitHelper_) {
		play = play_;
		game = play.game;
		gameInput = game.gameInput;
		exitHelper = exitHelper_;
		
		shipSteering.Initialize(this, play, gameInput);
		shipControl.Initialize(this, game, play, gameInput);
		
		isExitHelperLaunched = false;
		isHeadlightOn = false;
		headlight.gameObject.SetActiveRecursively(isHeadlightOn);
		cameraPosition = CAMERA_POSITION_COCKPIT;
		missileLockMode = MissileLockMode.None;
		
		AddWeapons();
		
		lastMoveTime = Time.time;
		chargedMissileTimer = -1f;
		isDetonatorMissileExploded = true;
	}
	
	public void Activate() {
		shipCamera.enabled = true;
	}
	
	public void Deactivate() {
		shipCamera.enabled = false;
	}
	
	void FixedUpdate() {
		if (!play.isPaused) {
			play.CachePositionalDataOfShip(transform.position);
			
			if (cameraPosition != CAMERA_POSITION_COCKPIT) {
				PositionCamera();
			}
			
			if (currentSecondaryWeapon != -1) {
				secondaryWeapons[currentSecondaryWeapon].IsReloaded();
				if (chargedMissileTimer != -1f && shield > 0) {
					float timeDelta = Time.time - chargedMissileTimer;
					int fullDeduction = Mathf.Min(CHARGED_MISSILE_SHIELD_MAX, Mathf.FloorToInt(Mathf.Min(timeDelta, CHARGED_MISSILE_TIME_MAX) * CHARGED_MISSILE_DEDUCTION));
					int newDeduction = Mathf.Min(fullDeduction - chargedMissileShieldDeducted, shield);
	//				Debug.Log (newDeduction);
					shield -= newDeduction;
					shieldPercentage = Mathf.CeilToInt( (shield/(float)maxShield) * 100f);
					chargedMissileShieldDeducted = fullDeduction;
				}
			}
			// Enemy target info
			if (Physics.Raycast(transform.position, transform.forward, out hit, Game.MAX_VISIBILITY_DISTANCE, Game.LAYER_MASK_ENEMIES_CAVE)) {
				if (hit.collider.tag == Enemy.TAG) {
					Enemy e = hit.transform.GetComponent<Enemy>();
					play.playGUI.EnemyInSight(e);
					if (	currentSecondaryWeapon != -1
							&& secondaryWeapons[currentSecondaryWeapon].type == Weapon.TYPE_GUIDED_MISSILE
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
		health = Mathf.Max (0, health-damage);
		healthPercentage = Mathf.CeilToInt( (health/(float)maxHealth) * 100f);
		shieldPercentage = Mathf.CeilToInt( (shield/(float)maxShield) * 100f);
		if (health == 0) {
			// stop game
			play.RepeatZone();
		}
		//Debug.Log (damage +" " + shield + " " +  maxShield+ " "+ health + " " + healthPercentage + " " +  shieldPercentage);
	}
	
	public void Heal(int amount) {
//		Debug.Log (amount + " " + Mathf.RoundToInt(maxHealth * ((float)amount/100f)) + " " + health);
		health = Mathf.Min(maxHealth, health + amount); //Mathf.Min(maxHealth, health + Mathf.RoundToInt(maxHealth * ((float)amount/100f)));
		healthPercentage = Mathf.CeilToInt( (health/(float)maxHealth) * 100f);
	}

	public void Shield(int amount) { // percentage of maxHealth
		shield = Mathf.Min(maxShield, shield + amount); //Mathf.Min(maxShield, shield + Mathf.RoundToInt(maxShield * ((float)amount/100f)));
		shieldPercentage = Mathf.CeilToInt( (shield/(float)maxShield) * 100f);
	}
	
	public Vector3 IsVisibleFrom(Vector3 fromPos) {
		Vector3 result = Vector3.zero;
		if (!play.isShipInPlayableArea) {
			return result;
		}
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
		play.playGUI.SwitchHeadlight();
	}
	
	public void CycleCamera() {
		cameraPosition++;
		
		if (cameraPosition == CAMERA_POSITIONS.Length) {
			cameraPosition = CAMERA_POSITION_COCKPIT;
		}
		PositionCamera();

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
	
	private void PositionCamera() {
		Vector3 toCamera = transform.TransformDirection(CAMERA_POSITIONS[cameraPosition]);
		Vector3 pos = transform.position + toCamera;
		if (Physics.Raycast(transform.position, toCamera, out hit,	toCamera.magnitude, 1 << Game.LAYER_CAVE)) {
			pos = Vector3.Lerp(transform.position, hit.point, 0.9f);
		}
		cameraTransform.position = pos;
	}

	private void InstantiateShipHUD() {
		shipHUD = GameObject.Instantiate(shipHUDPrefab) as GameObject;
		Transform crossHair = shipHUD.transform.Find("Cross Hair");
		crossHair.localScale /= (crossHair.renderer.material.mainTexture.width/(float)Screen.width) / (32.0f/(float)Screen.width);
	}
	
	public void AddPrimaryWeapon(int wType) {
		Weapon w = new Weapon(null, Weapon.PRIMARY, transform, play, wType,
			WEAPON_POSITIONS, WEAPON_ROTATIONS, Game.SHIP, false);
		primaryWeapons[wType] = w;
		currentPrimaryWeapon = wType;
		primaryWeapons[currentPrimaryWeapon].Mount();
		firepowerPerSecond = primaryWeapons[currentPrimaryWeapon].damage;// / w1.frequency; // we assume 1 shot per second ALWAYS
		play.playGUI.DisplayPrimaryWeapon(primaryWeapons[currentPrimaryWeapon]);		
		Debug.Log ("adding primary weapon type: " + wType);
	}

	public void AddSecondaryWeapon(int wType) {
		int ammunition = 1;
		if (secondaryWeapons[wType] != null) {
			ammunition += secondaryWeapons[wType].ammunition;
		}
		Weapon w = new Weapon(null, Weapon.SECONDARY, transform, play, wType, WEAPON_POSITIONS,
			WEAPON_ROTATIONS, Game.SHIP, false, ammunition);
		secondaryWeapons[wType] = w;
		currentSecondaryWeapon = wType;
		secondaryWeapons[currentSecondaryWeapon].Mount();
		play.playGUI.DisplaySecondaryWeapon(secondaryWeapons[currentSecondaryWeapon]);
		Debug.Log ("adding secondary weapon type: " + wType);
	}
	
	private void AddWeapons() {			
		currentPrimaryWeapon = -1;
		currentSecondaryWeapon = -1;
		for (int i=0; i < 8; i++) {
			if (play.zoneID > Weapon.SHIP_PRIMARY_WEAPON_AVAILABILITY_MAX[i]) {
				AddPrimaryWeapon(i);
				primaryWeapons[currentPrimaryWeapon].Unmount();
			}	
		}
		for (int i=0; i < 4; i++) {
			if (play.zoneID > Weapon.SHIP_SECONDARY_WEAPON_AVAILABILITY_MAX[i]) {
				AddSecondaryWeapon(i);
				secondaryWeapons[currentSecondaryWeapon].Unmount();
			}
		}
		
		if (currentPrimaryWeapon != -1) {
			primaryWeapons[currentPrimaryWeapon].Mount();
			firepowerPerSecond = primaryWeapons[currentPrimaryWeapon].damage;// / w1.frequency; // we assume 1 shot per second ALWAYS
		}
		if (currentSecondaryWeapon != -1) {
			secondaryWeapons[currentSecondaryWeapon].Mount();
		}
	}
	
	public void AddSpecial(int id) {
		Debug.Log ("Adding special capability TODO " + id);
	}
	
	public void CalculateHullClazz() {
		for (int i=0; i<8; i++) {
			if (play.zoneID > HULL_POWER_UP[i]) {
				hullCLazz = i;
			}
		}
		SetHealthAndShield();
	}
	
	public void SetHealthAndShield() {
		maxHealth = HEALTH[hullCLazz];
		maxShield = SHIELD[hullCLazz];
		health = maxHealth;
		shield = maxShield;
		healthPercentage = 100;
		shieldPercentage = 100;
	}
	
	public void SetHull(int newHullClazz) {
		hullCLazz = newHullClazz;
		SetHealthAndShield();
	}
		
	public void ShootPrimary() {
		if (play.isShipInPlayableArea && currentPrimaryWeapon != -1 && primaryWeapons[currentPrimaryWeapon].IsReloaded()) {
			primaryWeapons[currentPrimaryWeapon].Shoot();
		}
	}

	public void ShootSecondary() {
		if (isDetonatorMissileExploded) {
			if (play.isShipInPlayableArea && currentSecondaryWeapon != -1 && secondaryWeapons[currentSecondaryWeapon].IsReloaded()) {
				if (currentSecondaryWeapon == Weapon.TYPE_CHARGED_MISSILE && chargedMissileShieldDeducted != 0) {
					//Debug.Log(chargedMissileShieldDeducted);
					secondaryWeapons[currentSecondaryWeapon].loadedShots[0].damage += chargedMissileShieldDeducted;
					chargedMissileShieldDeducted = 0;
				} else if (currentSecondaryWeapon == Weapon.TYPE_DETONATOR_MISSILE) {
					isDetonatorMissileExploded = false;
				}
				secondaryWeapons[currentSecondaryWeapon].Shoot();
				play.playGUI.DisplaySecondaryWeapon(secondaryWeapons[currentSecondaryWeapon]);
			}
			chargedMissileTimer = -1f;
		} else {
			secondaryWeapons[currentSecondaryWeapon].loadedShots[0].Detonate();
			isDetonatorMissileExploded = true;
		}
	}
	
	public void CyclePrimary() {
		if (currentPrimaryWeapon != -1) {
			primaryWeapons[currentPrimaryWeapon].Unmount();
			currentPrimaryWeapon++;
			if (currentSecondaryWeapon == 8 || primaryWeapons[currentPrimaryWeapon] == null) {
				currentPrimaryWeapon = 0;
			}
			primaryWeapons[currentPrimaryWeapon].Mount();
			firepowerPerSecond = primaryWeapons[currentPrimaryWeapon].damage;// / w1.frequency; // we assume 1 shot per second ALWAYS
			play.playGUI.DisplayPrimaryWeapon(primaryWeapons[currentPrimaryWeapon]);
		}
	}

	public void CycleSecondary() {
		if (currentSecondaryWeapon != -1) {
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
	
	public void StartChargedMissileTimer() {
		if (secondaryWeapons[currentSecondaryWeapon].IsReloaded()) {
			chargedMissileTimer = Time.time;
			chargedMissileShieldDeducted = 0;
		}
	}
	
	public void LaunchExitHelper() {
		if (isExitHelperLaunched) {
			LaunchExitHelper(false);
		} else {
			LaunchExitHelper(true);
		}
	}
	
	public void LaunchExitHelper(bool toLaunch) {
		if (toLaunch) {
			Vector3 pos = transform.position + transform.TransformDirection(EXIT_HELPER_LAUNCH_POSITION);
			if (Physics.Raycast(transform.position, transform.forward, out hit,
					transform.TransformDirection(EXIT_HELPER_LAUNCH_POSITION).magnitude, 1 << Game.LAYER_CAVE)) {
				pos = hit.point;
			}
			exitHelper.transform.position = pos;
			exitHelper.Activate();
			isExitHelperLaunched = true;
		} else {
			exitHelper.transform.position = new Vector3(9000f, 9000f, 9000f);
			exitHelper.Deactivate();
			isExitHelperLaunched = false;
		}
		play.playGUI.SwitchExitHelper();
	}
	
	public void PlaySound(int weaponMountAsType, int type) {
		game.PlaySound(audioSource, weaponMountAsType, type);
	}
	
}