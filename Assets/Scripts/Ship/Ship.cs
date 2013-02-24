using System;
using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	public GameObject shipHUDPrefab;
	public Game game;
	
	public int health;
	
	private Play play;
	private GameInput gameInput;
	
	// ship components
	private GameObject shipHUD;
	private ShipSteering shipSteering;
	private ShipControl shipControl;
	private Transform headlight;
	private RaycastHit hit;
	
	private bool isHeadlightOn;
	
	public static string TAG = "Ship";
	
	private static float FORCE_MOVE = 25.0f;
	private static float FORCE_TURN = 5.0f;
	private static float FORCE_YAW = 3.5f;
	
	private static int HEALTH = 200;
	
//	private Vector3 collisionPoint = Vector3.zero;
//	private Vector3 collisionNormal = Vector3.zero;
	
	void Awake() {
//		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
//		Debug.Log (Screen.dpi);
		
		InstantiateShipHUD();
		shipSteering = transform.GetComponent<ShipSteering>();
		shipControl = new ShipControl(); //transform.GetComponent<ShipControl>();
		headlight = transform.FindChild("Headlight");
	}
	
	public void Initialize(Play p, Game g) {
		play = p;
		game = g;
		gameInput = game.gameInput;
		
		shipSteering.Initialize(this, play, gameInput);
		shipControl.Initialize(this, game, play, gameInput);
		
		isHeadlightOn = true;
		health = HEALTH;
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
	
/*	void FixedUpdate () {
		if (rigidbody.freezeRotation) {
			rigidbody.freezeRotation = false;
		}
	}*/
			
	void OnGUI() {
		if (GUI.RepeatButton  (new Rect (60,400,50,50), "Exit"))
			Application.Quit();
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
}


