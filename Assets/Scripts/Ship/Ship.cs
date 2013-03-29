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
	public bool hasSecondaryWeapon;
	
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
	
	public static int SECONDARY_WEAPON_MISSILE_START = 10;
	public static int SECONDARY_WEAPON_MISSILE_GUIDED_START = 20;
	public static int SECONDARY_WEAPON_MISSILE_CHARGED_START = 30;
	
	public List<Weapon> primaryWeapons = new List<Weapon>();
	public List<Weapon> secondaryWeapons = new List<Weapon>();
	
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
		
		AddWeapons();

		firepowerPerSecond = 0;
		foreach (Weapon w1 in primaryWeapons) {
			firepowerPerSecond += w1.damage;// / w1.frequency; // we assume 1 shot per second ALWAYS
		}
		
		lastMoveTime = Time.time;
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
		int lastLowestTypePrimary = Weapon.SHIP_PRIMARY_WEAPON_TYPES[zone5] + 1;
		int lastLowestTypeSecondary = Weapon.SHIP_SECONDARY_WEAPON_TYPES[zone5] + 1;
		for (int i=zone5; i >= 0; i--) {
			if (Weapon.SHIP_PRIMARY_WEAPON_TYPES[i] < lastLowestTypePrimary && Weapon.SHIP_PRIMARY_WEAPON_TYPES[i] != 0) {
				Weapon w = new Weapon(transform, play, Weapon.SHIP_PRIMARY_WEAPON_TYPES[i], Weapon.SHIP_PRIMARY_WEAPON_MODELS[i], WEAPON_POSITIONS[WEAPON_POSITION_WING_LEFT], Game.SHIP);
				primaryWeapons.Add(w);
				w.weaponTransform.localEulerAngles = WEAPON_ROTATIONS[WEAPON_POSITION_WING_LEFT];
				lastLowestTypePrimary = Weapon.SHIP_PRIMARY_WEAPON_TYPES[i];
				Debug.Log ("adding primary weapon type/model " + Weapon.SHIP_PRIMARY_WEAPON_TYPES[i]+"/"+Weapon.SHIP_PRIMARY_WEAPON_MODELS[i]);
			}	
			if (Weapon.SHIP_SECONDARY_WEAPON_TYPES[i] < lastLowestTypeSecondary && Weapon.SHIP_SECONDARY_WEAPON_TYPES[i] != 0) {
				Weapon w = new Weapon(transform, play, Weapon.SHIP_SECONDARY_WEAPON_TYPES[i], Weapon.SHIP_SECONDARY_WEAPON_MODELS[i], WEAPON_POSITIONS[WEAPON_POSITION_CENTER], Game.SHIP, 5);
				secondaryWeapons.Add(w);
				w.weaponTransform.localEulerAngles = WEAPON_ROTATIONS[WEAPON_POSITION_CENTER];
				lastLowestTypeSecondary = Weapon.SHIP_SECONDARY_WEAPON_TYPES[i];
				Debug.Log ("adding secondary weapon type/model " + Weapon.SHIP_SECONDARY_WEAPON_TYPES[i]+"/"+Weapon.SHIP_SECONDARY_WEAPON_MODELS[i]);
			}
		}
		
		currentPrimaryWeapon = 0;
		currentSecondaryWeapon = 0;
	}
	
	private int CalculateHealth() {
		int zone5 = Zone.GetZone5StepID(play.zoneID);
		
		return zone5 * 180;
	}
	
	public void ShootPrimary() {
		if (Time.time > primaryWeapons[currentPrimaryWeapon].lastShotTime + primaryWeapons[currentPrimaryWeapon].frequency) {
			primaryWeapons[currentPrimaryWeapon].Shoot();
			primaryWeapons[currentPrimaryWeapon].lastShotTime = Time.time;
		}
	}

	public void ShootSecondary() {
		if (secondaryWeapons.Count > 0
				&& Time.time > secondaryWeapons[currentSecondaryWeapon].lastShotTime + secondaryWeapons[currentSecondaryWeapon].frequency
				&& secondaryWeapons[currentSecondaryWeapon].ammunition > 0) {
			secondaryWeapons[currentSecondaryWeapon].Shoot();
			secondaryWeapons[currentSecondaryWeapon].lastShotTime = Time.time;
		}
	}
	
	public void CyclePrimary() {
		currentPrimaryWeapon++;
		if (currentPrimaryWeapon == primaryWeapons.Count) {
			currentPrimaryWeapon = 0;
		}
		play.playGUI.DisplayPrimaryWeapon(primaryWeapons[currentPrimaryWeapon]);
	}

	public void CycleSecondary() {
		currentSecondaryWeapon++;
		if (currentSecondaryWeapon == secondaryWeapons.Count) {
			currentSecondaryWeapon = 0;
		}
		play.playGUI.DisplayPrimaryWeapon(secondaryWeapons[currentSecondaryWeapon]);
	}
	
}


