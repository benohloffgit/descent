using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour {
	public GameObject shipHUDPrefab;
	public Game game;
	
	public int health;
	public int shield;
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
	private Camera shipCamera;
	
	private bool isHeadlightOn;
	private int cameraPosition;
	
	public static string TAG = "Ship";
	
	private static float FORCE_MOVE = 25.0f;
	private static float FORCE_TURN = 5.0f;
	private static float FORCE_YAW = 3.5f;
	
	private static int HEALTH = 200;
	private static int SHIELD = 50;
	
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

	public List<Weapon> weapons = new List<Weapon>();
	
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
	
	public void Initialize(Play p, Game g) {
		play = p;
		game = g;
		gameInput = game.gameInput;
		
		shipSteering.Initialize(this, play, gameInput);
		shipControl.Initialize(this, game, play, gameInput);
		
		isHeadlightOn = true;
		SwitchHeadlight();
		cameraPosition = CAMERA_POSITION_COCKPIT;
		health = HEALTH;
		shield = SHIELD;
		
		AddWeapons();

		firepowerPerSecond = 0;
		foreach (Weapon w1 in weapons) {
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
	
/*	void FixedUpdate () {
		if (rigidbody.freezeRotation) {
			rigidbody.freezeRotation = false;
		}
	}*/
					
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
			
		Weapon w = new Weapon(transform, play, Weapon.SHIP_PRIMARY_WEAPON_TYPES[zone5], Weapon.SHIP_PRIMARY_WEAPON_MODELS[zone5], WEAPON_POSITIONS[WEAPON_POSITION_WING_LEFT], Game.SHIP);
		weapons.Add(w);
		w.weaponTransform.localEulerAngles = WEAPON_ROTATIONS[WEAPON_POSITION_WING_LEFT]; // turn upside down on center slot
/*		w = new Weapon(transform, play, Weapon.TYPE_LASER, 2, WEAPON_POSITIONS[WEAPON_POSITION_WING_RIGHT], Game.SHIP);
		weapons.Add(w);
		w.weaponTransform.localEulerAngles = WEAPON_ROTATIONS[WEAPON_POSITION_WING_RIGHT]; // turn upside down on center slot*/
	}
}


