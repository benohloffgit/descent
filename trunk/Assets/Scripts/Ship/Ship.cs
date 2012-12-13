using System;
using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	public GameObject shipHUDPrefab;
	
	private Game game;
	private Play play;
	private GameInput gameInput;
	
	// ship components
	private GameObject shipHUD;
	private ShipSteering shipSteering;
	private ShipShooting shipShooting;
	
	private static float FORCE_MOVE = 25.0f;
	private static float FORCE_TURN = 1.5f;
	
//	private Vector3 collisionPoint = Vector3.zero;
//	private Vector3 collisionNormal = Vector3.zero;
	
	void Awake() {
//		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		Debug.Log (Screen.dpi);
		
		InstantiateShipHUD();
		shipSteering = transform.GetComponent<ShipSteering>();
		shipShooting = transform.GetComponent<ShipShooting>();
	}
	
	public void Initialize(Play p, Game g) {
		play = p;
		game = g;
		gameInput = game.gameInput;
		
		shipSteering.Initialize(this, gameInput);
		shipShooting.Initialize(this, gameInput);
	}
	
	void OnCollisionEnter(Collision c) {
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
		//shipSteering.DispatchGameInput();
		shipShooting.DispatchGameInput();
	}
	
	public void Move(Vector3 direction) {
		rigidbody.AddRelativeForce(direction * FORCE_MOVE);
	}
	
	public void Turn(Vector3 direction) {
		rigidbody.AddRelativeTorque(direction * FORCE_TURN);
	}
	
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


