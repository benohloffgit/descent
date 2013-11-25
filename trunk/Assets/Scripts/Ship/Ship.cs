using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour {
	public static string TAG = "Ship";
	
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
	private int highestPrimaryWeapon;
	private int highestSecondaryWeapon;
	
	public float firepowerPerSecond;
	public float lastMoveTime;
		
	private Play play;
	private ExitHelper exitHelper;
	private GameInput gameInput;
	private AudioSource audioSource;
	
	// ship components
	private ShipSteering shipSteering;
	private ShipControl shipControl;
	private Transform headlight;
	private RaycastHit hit;
	private Transform cameraTransform;
	public Camera shipCamera;
	
	public bool isExitHelperLaunched;
	public bool isHeadlightOn;
	public bool isBoosterOn;
	public bool isBoosterLoading;
	public bool isCloakOn;
	public bool isCloakLoading;
	public bool isInvincibleOn;
	public bool hasBeenInvincibleInThisZone;
	public bool[] hasSpecial;
	private int cameraPosition;
	public MissileLockMode missileLockMode;
	public Enemy lockedEnemy;
	public float missileLockTime;
	private float chargedMissileTimer;
	private int chargedMissileShieldDeducted;
	public bool isDetonatorMissileExploded;
	private float boostTimer;
	private float cloakTimer;
	public float invincibleTimer;
	private ParticleSystem powerUpParticleSystem;
	private ParticleSystem powerUpParticleSystemOneParticle;
	//private PowerUpBoostParticle powerUpBoostParticle;
			
	private static float FORCE_MOVE = 65.0f;
	private static float FORCE_TURN = 24f; // 5.0f
	private static float FORCE_YAW = 16f; // 3.5f
	private static float FORCE_BOOST = 40f;
	
	private static float BOOST_DURATION = 10f;
	private static float BOOST_INTERVAL = 60f;
	private static float CLOAK_DURATION = 30f;
	private static float CLOAK_INTERVAL = 300f;
	private static float INVINCIBLE_DURATION = 15f;
	
	public static int SPECIAL_LIGHTS = 0;
	public static int SPECIAL_BOOST = 1;
	public static int SPECIAL_CLOAK = 2;
	public static int SPECIAL_INVINCIBLE = 3;
	
	public static int NO_HULL = -1;
	
	private static float CHARGED_MISSILE_SHIELD_MAX = 50f;
	private static float CHARGED_MISSILE_TIME_MAX = 2.5f; // seconds
	private static float CHARGED_MISSILE_DEDUCTION = 20f; // deducted shield per second
	
	private static Vector3[] CAMERA_POSITIONS = new Vector3[] {Vector3.zero, new Vector3(0, 3.0f, -12.0f), new Vector3(-5f, 0f, 0f), new Vector3(5f, 0f, 0f), new Vector3(0, 0f, 5.0f), new Vector3(0, 12f, 0f)};
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(-1.014f, 0f, 1.664f), new Vector3(1.014f, 0f, 1.664f), new Vector3(0, -0.37f, 1.65f)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] { new Vector3(0,0,90f),  new Vector3(0,0,-90f),  new Vector3(0,0,180f)};
	
	public static int[] HEALTH = new int[] { 100, 112, 126, 135, 148, 162, 180, 200 };
//	public static int[] SHIELD = new int[] { 45, 50, 56, 60, 66, 72, 80, 85 };
	public static int[] SHIELD = new int[] { 100, 125, 150, 175, 200, 225, 250, 400 };
	public static int[] HULL_POWER_UP = new int[] {0,2,5,9,14,19,25,31}; // {0,5,12,20,29,39,50,62}
	public static int[] SPECIAL_POWER_UP = new int[] {6,12,18,28};

	public static string[] HULL_TYPES = new string[] {"Armor", "Improved Armor", "Keflar Armor", "Magnetic Armor", "Plasma Armor", "Nano Armor", "? Armor", "Higgs Armor"};
	public static string[] SPECIAL_NAMES = new string[] {"Light", "Boost", "Cloak", "Indestructable"};
	
	private static int BULLET_IMPACT_MIN = 28;
	private static int BULLET_IMPACT_MAX = 30;
	
	private static int CAMERA_POSITION_COCKPIT = 0;
	private static int CAMERA_POSITION_BEHIND = 1;
	private static Vector3 EXIT_HELPER_LAUNCH_POSITION = new Vector3(0f, 0f, 2f);
	private static float SPHERE_CAST_RADIUS = RoomMesh.MESH_SCALE/3f;
		
	public Weapon[] primaryWeapons = new Weapon[8];
	public Weapon[] secondaryWeapons = new Weapon[4];
		
	public enum MissileLockMode { None=0, Aiming=1, Locked=2 }
	
	public static int MISSILE_LOCK_DURATION = 2;
	
//	private Vector3 collisionPoint = Vector3.zero;
//	private Vector3 collisionNormal = Vector3.zero;
	
	void Awake() {
//		Screen.sleepTimeout = SleepTimeout.NeverSleep;	
//		Debug.Log (Screen.dpi);
//		InstantiateShipHUD();
		shipSteering = new ShipSteering();//transform.GetComponent<ShipSteering>();
		shipControl = new ShipControl(); //transform.GetComponent<ShipControl>();
		headlight = transform.FindChild("Headlight");
		cameraTransform = transform.FindChild("Camera");
		shipCamera = cameraTransform.GetComponent<Camera>();
		audioSource = GetComponent<AudioSource>();
		currentPrimaryWeapon = -1;
		currentSecondaryWeapon = -1;
		powerUpParticleSystem = transform.FindChild("Camera/PowerUpParticleEffect").particleSystem;
		powerUpParticleSystemOneParticle = transform.FindChild("Camera/PowerUpParticleEffectOneParticle").particleSystem;
//		powerUpBoostParticle = transform.FindChild("Camera/PowerUpBoostParticle").GetComponent<PowerUpBoostParticle>();
//		powerUpBoostParticle.Initialize(this);
		
	}
	
	public void Initialize(Play play_, ExitHelper exitHelper_) {
		play = play_;
		game = play.game;
		gameInput = game.gameInput;
		exitHelper = exitHelper_;
		hasSpecial = new bool[SPECIAL_NAMES.Length];
		
		shipSteering.Initialize(this, play, gameInput);
		shipControl.Initialize(this, game, play, gameInput);
		
	}
	
	public void Reset() {
		enabled = true;
		shipCamera.enabled = true;
		isExitHelperLaunched = false;
		isBoosterOn = false;
		isBoosterLoading = false;
		isCloakOn = false;
		isCloakLoading = false;
		isInvincibleOn = false;
		hasBeenInvincibleInThisZone = false;
		boostTimer = -BOOST_INTERVAL;
		cloakTimer = -CLOAK_INTERVAL;
		lastMoveTime = Time.fixedTime;
		chargedMissileTimer = -1f;
		isDetonatorMissileExploded = true;
		cameraPosition = CAMERA_POSITION_COCKPIT;
		isHeadlightOn = false;
		headlight.gameObject.SetActiveRecursively(isHeadlightOn);
		missileLockMode = MissileLockMode.None;
		transform.position = play.cave.GetCaveEntryPosition();
		transform.rotation = Quaternion.identity;
	}
	
	public void Deactivate() {
		shipCamera.enabled = false;
		enabled = false;
		SwitchOffPowerUps();
	}
	
	public void DispatchFixedUpdate() {
		shipSteering.DispatchFixedUpdate();
//		Debug.Log (Time.timeScale + " " + Time.fixedDeltaTime);
		play.CachePositionalDataOfShip(transform.position);
		
		play.playGUI.shipHealthProgressBar.SetBar(health/(float)maxHealth);
		play.playGUI.shipShieldProgressBar.SetBar(shield/(float)maxShield);
		
		if (cameraPosition != CAMERA_POSITION_COCKPIT) {
			PositionCamera();
		}
		
		if (isBoosterOn && Time.fixedTime > boostTimer + BOOST_DURATION) {
			BoostOff();
		} else if (isBoosterLoading) {
			if (Time.fixedTime > boostTimer + BOOST_INTERVAL) {
				isBoosterLoading = false;
				play.playGUI.SwitchShipBoost();
			} else {
				play.playGUI.boosterProgressBar.SetBar((Time.fixedTime-boostTimer)/BOOST_INTERVAL);
			}
		}

		if (isCloakOn && Time.fixedTime > cloakTimer + CLOAK_DURATION) {
			CloakOff();
		} else if (isCloakLoading) {
			if (Time.fixedTime > cloakTimer + CLOAK_INTERVAL) {
				isCloakLoading = false;
				play.playGUI.SwitchShipCloak();
			} else {
				play.playGUI.cloakProgressBar.SetBar((Time.fixedTime-cloakTimer)/CLOAK_INTERVAL);
			}
		}

		if (isInvincibleOn && Time.fixedTime > invincibleTimer + INVINCIBLE_DURATION) {
			InvincibleOff();
		}
		
		if (currentSecondaryWeapon != -1) {
			secondaryWeapons[currentSecondaryWeapon].IsReloaded();
			if (chargedMissileTimer != -1f && shield > 0) {
				float timeDelta = Time.fixedTime - chargedMissileTimer;
				float fullDeduction = Mathf.Min(CHARGED_MISSILE_SHIELD_MAX, Mathf.Min(timeDelta, CHARGED_MISSILE_TIME_MAX) * CHARGED_MISSILE_DEDUCTION);
				int newDeduction = Mathf.Min(Mathf.FloorToInt(fullDeduction - chargedMissileShieldDeducted), shield);
//				Debug.Log (newDeduction);
				shield -= newDeduction;
				shieldPercentage = Mathf.CeilToInt( (shield/(float)maxShield) * 100f);
				chargedMissileShieldDeducted += newDeduction;
				play.playGUI.chargedMissileProgressBar.SetBar(fullDeduction/CHARGED_MISSILE_SHIELD_MAX);
			}
		}
		// Enemy target info
		if (Physics.SphereCast(transform.position, SPHERE_CAST_RADIUS, transform.forward, out hit, Game.MAX_VISIBILITY_DISTANCE, Game.LAYER_MASK_ENEMIES_CAVE)) {
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
						missileLockTime = Time.fixedTime;
						missileLockMode = MissileLockMode.Aiming;
					} else {
						if (missileLockMode == MissileLockMode.Aiming && Time.fixedTime > missileLockTime + MISSILE_LOCK_DURATION) {
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
	
	public void DispatchGameInput() {
		shipSteering.DispatchGameInput();
		shipControl.DispatchGameInput();
	}
	
	public void Move(Vector3 direction) {
		rigidbody.AddRelativeForce(direction * (FORCE_MOVE + (isBoosterOn ? FORCE_BOOST : 0)));
		lastMoveTime = Time.fixedTime;
	}
	
	public void Turn(Vector3 direction) {
		rigidbody.AddRelativeTorque(direction * FORCE_TURN);
	}

	public void Yaw(Vector3 direction) {
		rigidbody.AddRelativeTorque(direction * FORCE_YAW);
	}
	
	public void Damage(int damage, Vector3 worldPos, int shotType) {
		if (!isInvincibleOn) {
			if (shotType < 4) {
				PlaySound(Game.SOUND_TYPE_VARIOUS, UnityEngine.Random.Range(BULLET_IMPACT_MIN, BULLET_IMPACT_MAX+1));
			} else {
				PlaySound(Game.SOUND_TYPE_VARIOUS, 31);
			}
			if (shield > 0) {
				shield -= damage;
				if (shield < 0) {
					damage = Mathf.Abs(shield);
					shield = 0;
				} else {
					damage = 0;
				}
			}
			play.playGUI.IndicateDamage(worldPos);
			health = Mathf.Max (0, health-damage);
			healthPercentage = Mathf.CeilToInt( (health/(float)maxHealth) * 100f);
			shieldPercentage = Mathf.CeilToInt( (shield/(float)maxShield) * 100f);
			if (health == 0) {
				// stop game
				play.RepeatZone();
			}
			//Debug.Log (damage +" " + shield + " " +  maxShield+ " "+ health + " " + healthPercentage + " " +  shieldPercentage);
		}
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
		if (!play.isShipInPlayableArea || isCloakOn) {
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

//	private void InstantiateShipHUD() {
//		shipHUD = GameObject.Instantiate(shipHUDPrefab) as GameObject;
//		Transform crossHair = shipHUD.transform.Find("Cross Hair");
//		crossHair.localScale /= (crossHair.renderer.material.mainTexture.width/(float)Screen.width) / (32.0f/(float)Screen.width);
//	}
	
	public void AddPrimaryWeapon(int wType) {
		if (currentPrimaryWeapon == -1) {
			play.playGUI.DisplayCrossHair();
		} else {
			primaryWeapons[currentPrimaryWeapon].Unmount();
		}
		Weapon w = new Weapon(null, Weapon.PRIMARY, transform, play, wType,
			WEAPON_POSITIONS, WEAPON_ROTATIONS, Game.SHIP, false);
		primaryWeapons[wType] = w;
		currentPrimaryWeapon = wType;
		highestPrimaryWeapon = currentPrimaryWeapon;
		primaryWeapons[currentPrimaryWeapon].Mount();
		firepowerPerSecond = primaryWeapons[currentPrimaryWeapon].damage;// / w1.frequency; // we assume 1 shot per second ALWAYS
		play.playGUI.DisplayPrimaryWeapon(primaryWeapons[currentPrimaryWeapon]);		
		Game.MyDebug ("adding primary weapon type: " + wType);
	}

	public void AddSecondaryWeapon(int wType) {
		if (currentSecondaryWeapon != -1) {
			secondaryWeapons[currentSecondaryWeapon].Unmount();
		}
		int ammunition = 2;
		if (secondaryWeapons[wType] != null) {
			ammunition += secondaryWeapons[wType].ammunition;
		}
		Weapon w = new Weapon(null, Weapon.SECONDARY, transform, play, wType, WEAPON_POSITIONS,
			WEAPON_ROTATIONS, Game.SHIP, false, ammunition);
		secondaryWeapons[wType] = w;
		currentSecondaryWeapon = wType;
		highestSecondaryWeapon = currentSecondaryWeapon;
		secondaryWeapons[currentSecondaryWeapon].Mount();
		play.playGUI.DisplaySecondaryWeapon();
		Game.MyDebug ("adding secondary weapon type: " + wType);
	}
	
	public void AddWeapons() {
//		Debug.Log ("adding weapons");
		currentPrimaryWeapon = -1;
		currentSecondaryWeapon = -1;
		for (int i=0; i < 8; i++) {
			//if (play.zoneID > Weapon.SHIP_PRIMARY_WEAPON_AVAILABILITY_MAX[i]) {
			if (game.state.HasPowerUp(Game.POWERUP_PRIMARY_WEAPON, i) || Game.IS_DEBUG_ON && play.zoneID > Weapon.SHIP_PRIMARY_WEAPON_AVAILABILITY_MAX[i]) { //Game.IS_DEBUG_ON && 
				AddPrimaryWeapon(i);
				primaryWeapons[currentPrimaryWeapon].Unmount();
			}	
		}
		for (int i=0; i < 4; i++) {
			if (game.state.HasPowerUp(Game.POWERUP_SECONDARY_WEAPON, i) || Game.IS_DEBUG_ON && play.zoneID > Weapon.SHIP_SECONDARY_WEAPON_AVAILABILITY_MAX[i]) {//Game.IS_DEBUG_ON && 
//			if (play.zoneID > Weapon.SHIP_SECONDARY_WEAPON_AVAILABILITY_MAX[i]) {
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
	
	public void RemoveWeapons() {
		for (int i=0; i<8; i++) {
			if (primaryWeapons[i] != null) {
				Destroy(primaryWeapons[i].weaponTransform.gameObject);
				primaryWeapons[i] = null;
			}
		}
		for (int i=0; i<4; i++) {
			if (secondaryWeapons[i] != null) {
				Destroy (secondaryWeapons[i].weaponTransform.gameObject);
				secondaryWeapons[i] = null;
			}
		}
	}
	
	public void AddSpecials() {
		for (int i=0; i<SPECIAL_NAMES.Length; i++) {
			if (game.state.HasPowerUp(Game.POWERUP_SPECIAL, i) || Game.IS_DEBUG_ON && play.zoneID > SPECIAL_POWER_UP[i]) { //Game.IS_DEBUG_ON && 
				hasSpecial[i] = true;
			}
		}
	}
	
	public void AddSpecial(int id) {
		hasSpecial[id] = true;
		if (id == SPECIAL_LIGHTS) {
			play.playGUI.SwitchHeadlight();
		} else if (id == SPECIAL_BOOST) {
			play.playGUI.SwitchShipBoost();
		} else if (id == SPECIAL_CLOAK) {
			play.playGUI.SwitchShipCloak();
		} else if (id == SPECIAL_INVINCIBLE) {
			play.playGUI.SwitchShipInvincible();
		}
	}
	
	public void CalculateHullClazz() {
		hullCLazz = NO_HULL;
		for (int i=0; i<8; i++) {
//			if (play.zoneID > HULL_POWER_UP[i]) {
			if (game.state.HasPowerUp(Game.POWERUP_HULL, i) || Game.IS_DEBUG_ON && play.zoneID > HULL_POWER_UP[i]) {
				hullCLazz = i;
			}
		}
		SetHealthAndShield();
	}
	
	public void SetHealthAndShield() {
		if (hullCLazz != NO_HULL) {
			maxHealth = HEALTH[hullCLazz];
			maxShield = SHIELD[hullCLazz];
		} else {
			maxHealth = 1;
			maxShield = 1;
		}
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
				if (currentSecondaryWeapon == Weapon.TYPE_CHARGED_MISSILE) {
					play.playGUI.chargedMissileProgressBar.DisableRenderer();
					if(chargedMissileShieldDeducted != 0) {
						//Debug.Log(chargedMissileShieldDeducted);
						secondaryWeapons[currentSecondaryWeapon].loadedShots[0].damage += chargedMissileShieldDeducted;
						chargedMissileShieldDeducted = 0;
					}
				} else if (currentSecondaryWeapon == Weapon.TYPE_DETONATOR_MISSILE) {
					isDetonatorMissileExploded = false;
				}
				secondaryWeapons[currentSecondaryWeapon].Shoot();
				play.playGUI.DisplaySecondaryWeapon();
			}
			chargedMissileTimer = -1f;
		} else {
			secondaryWeapons[currentSecondaryWeapon].loadedShots[0].Detonate();
			isDetonatorMissileExploded = true;
		}
	}
	
	public void CyclePrimary(int dir) {
		if (currentPrimaryWeapon != -1) {
			int newID = currentPrimaryWeapon+dir;
			if (newID > highestPrimaryWeapon) {
				newID = 0;
			} else if (newID == -1 ) {
				newID = highestPrimaryWeapon;
			}
			SetPrimary (newID);
		}
	}
	
	public void SetPrimary(int id) {
		if (currentPrimaryWeapon != -1 && id <= highestPrimaryWeapon && id != currentPrimaryWeapon) {
			primaryWeapons[currentPrimaryWeapon].Unmount();
			currentPrimaryWeapon = id;
			primaryWeapons[currentPrimaryWeapon].Mount();
			firepowerPerSecond = primaryWeapons[currentPrimaryWeapon].damage;// / w1.frequency; // we assume 1 shot per second ALWAYS
			play.playGUI.DisplayPrimaryWeapon(primaryWeapons[currentPrimaryWeapon]);
			PlaySound(Game.SOUND_TYPE_VARIOUS, 10);
		}
	}

	public void CycleSecondary(int dir) {
		if (currentSecondaryWeapon != -1) {
			int newID = currentSecondaryWeapon+dir;
			if (newID > highestSecondaryWeapon) {
				newID = 0;
			} else if (newID == -1 ) {
				newID = highestSecondaryWeapon;
			}
			SetSecondary(newID);
		}
	}

	public void SetSecondary(int id) {
		if (currentSecondaryWeapon != -1 && id <= highestSecondaryWeapon && id != currentSecondaryWeapon) {
			secondaryWeapons[currentSecondaryWeapon].Unmount();
			currentSecondaryWeapon = id;
			secondaryWeapons[currentSecondaryWeapon].Mount();
			play.playGUI.DisplaySecondaryWeapon();
			if (secondaryWeapons[currentSecondaryWeapon].type != Weapon.TYPE_GUIDED_MISSILE && secondaryWeapons[currentSecondaryWeapon].type != Weapon.TYPE_DETONATOR_MISSILE) {
				missileLockMode = MissileLockMode.None;
			}
			PlaySound(Game.SOUND_TYPE_VARIOUS, 11);
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
			play.playGUI.DisplayNotification(game.state.GetDialog(61));
			chargedMissileTimer = Time.fixedTime;
			chargedMissileShieldDeducted = 0;
			play.playGUI.chargedMissileProgressBar.EnableRenderer();
			play.playGUI.chargedMissileProgressBar.SetBar(0);
		}
	}
	
	public void LaunchExitHelper() {
		if (isExitHelperLaunched) {
			LaunchExitHelper(false);
		} else {
			LaunchExitHelper(true);
		}
	}
	
	public void BoostShip() {
		if (!isCloakOn && !isInvincibleOn && !isBoosterOn) {
			if (!isBoosterLoading) {
				isBoosterOn = true;
				PlaySound(Game.SOUND_TYPE_VARIOUS, 27);
				boostTimer = Time.fixedTime;
				play.playGUI.SwitchShipBoost();
				powerUpParticleSystemOneParticle.renderer.material = game.powerUpParticleMaterials[Game.POWERUP_PARTICLE_MATERIAL_BOOST];
				powerUpParticleSystemOneParticle.Play();
			} else {
				play.playGUI.DisplayNotification(game.state.GetDialog(53));
			}
		} else {
			play.playGUI.DisplayNotification(game.state.GetDialog(52));
		}
	}
	
	private void BoostOff() {
		if (isBoosterOn) {
			isBoosterOn = false;
			isBoosterLoading = true;
			boostTimer = Time.fixedTime;
			play.playGUI.SwitchShipBoost();
			powerUpParticleSystemOneParticle.Stop();
			PlaySound(Game.SOUND_TYPE_VARIOUS, 46);
//			powerUpBoostParticle.renderer.enabled = false;
//			powerUpBoostParticle.enabled = false;
		}
	}		

	public void CloakShip() {
		if (!isCloakOn && !isBoosterOn && !isInvincibleOn) {
			if (!isCloakLoading) {
				isCloakOn = true;
				PlaySound(Game.SOUND_TYPE_VARIOUS, 48);
				cloakTimer = Time.fixedTime;
				play.playGUI.SwitchShipCloak();
				powerUpParticleSystem.renderer.material = game.powerUpParticleMaterials[Game.POWERUP_PARTICLE_MATERIAL_CLOAK];
				powerUpParticleSystem.Play();
				play.playGUI.HideCrossHair();
			} else {
				play.playGUI.DisplayNotification(game.state.GetDialog(53));
			}
		} else {
			play.playGUI.DisplayNotification(game.state.GetDialog(52));
		}
	}
	
	private void CloakOff() {
		if (isCloakOn) {
			isCloakOn = false;
			isCloakLoading = true;
			cloakTimer = Time.fixedTime;
			play.playGUI.SwitchShipCloak();
			powerUpParticleSystem.Stop();
			play.playGUI.DisplayCrossHair();
			PlaySound(Game.SOUND_TYPE_VARIOUS, 46);
		}
	}

	public void InvincibleShip() {
		if (!isBoosterOn && !isCloakOn) {
			if (!hasBeenInvincibleInThisZone) {
				isInvincibleOn = true;
				hasBeenInvincibleInThisZone = true;
				invincibleTimer = Time.fixedTime;
				PlaySound(Game.SOUND_TYPE_VARIOUS, 47);
				play.playGUI.SwitchShipInvincible();
				powerUpParticleSystemOneParticle.renderer.material = game.powerUpParticleMaterials[Game.POWERUP_PARTICLE_MATERIAL_INVINCIBLE];
				powerUpParticleSystemOneParticle.Play();
			} else {
				play.playGUI.DisplayNotification(game.state.GetDialog(54));
			}
		} else {
			play.playGUI.DisplayNotification(game.state.GetDialog(52));
		}
	}
	
	private void InvincibleOff() {
		if (isInvincibleOn) {
			isInvincibleOn = false;
			play.playGUI.SwitchShipInvincible();
			powerUpParticleSystemOneParticle.Stop();
			PlaySound(Game.SOUND_TYPE_VARIOUS, 46);
		}
	}
	
	private void SwitchOffPowerUps() {
		BoostOff();
		CloakOff();
		InvincibleOff();
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
			if (isExitHelperLaunched) {
				PlaySound(Game.SOUND_TYPE_VARIOUS, 36);
			}
			isExitHelperLaunched = false;
		}
		play.playGUI.SwitchExitHelper();
	}
	
	public void PlaySound(int weaponMountAsType, int type) {
		game.PlaySound(audioSource, weaponMountAsType, type);
	}
	
}