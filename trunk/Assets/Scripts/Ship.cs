using System;
using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	private Game game;
	private Play play;
	private GameInput gameInput;
	
	private Rigidbody rigidbody;
	private Mode mode;
	private FlyingMode flyingMode;
	
	private Vector3 forceMove;
	private Vector3 forceTurn;
	private Quaternion calibration;
	private bool isCalibrated;
	private Vector3 gyro;
	
	private static float FORCE_MOVE = 1200.0f;
	private static float FORCE_TURN = 150.0f;
	private static float accelerometerThreshold = 0.05f;

	public enum Mode { Stopped=0, Flying=1 }
	public enum FlyingMode { Stopped=0, Forward=1, Backward=2, Left=3, Right=4 }
	
	void Awake() {
		Application.targetFrameRate = 60;
//		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		rigidbody = GetComponent<Rigidbody>();
		forceMove = Vector3.zero;
		forceTurn = Vector3.zero;
		gyro = Vector3.zero;
		
		isCalibrated = false;		
	}
	
	public void Initialize(Play p, Game g) {
		play = p;
		game = g;
		gameInput = game.gameInput;
		if (gameInput.isMobile) {
			Input.gyro.enabled = true;
		}
		mode = Mode.Stopped;
	}
	
	void OnTriggerEnter(Collider collider) {
//		Debug.Log ("OnTriggerEnter");
	}
	
	void OnTriggerExit(Collider collider) {
//		Debug.Log ("OnTriggerExit");
	}

	public void DispatchGameInput() {
		if (gameInput.isMobile) {
			if (gameInput.isTouchDown[0] && !gameInput.isGUIClicked[0] && mode == Mode.Stopped) {
				mode = Mode.Flying;
				flyingMode = FlyingMode.Stopped;
			}
			if (gameInput.isTouchUp[0] && mode == Mode.Flying) {
				mode = Mode.Stopped;
			}
		}
	}

	void Update () {
		if (!isCalibrated) Calibrate();
	}
	
	void LateUpdate() {
	}
	
	void FixedUpdate () {
		if (gameInput.isMobile) {
//			Debug.Log (Input.acceleration + " " + Input.gyro.rotationRate + " " + Input.gyro.attitude);
			Quaternion calibrated = Quaternion.Inverse(calibration) * Input.gyro.attitude;
/*			Vector3 gyroLast = Input.gyro.rotationRate;
			
			float upDown = (gyroNew.y < 0) ? Mathf.Min(gyro.y, gyroNew.y) : Mathf.Max(gyro.y, gyroNew.y);
			float leftRight = (gyroNew.x < 0) ? Mathf.Min(gyro.x, gyroNew.x) : Mathf.Max(gyro.x, gyroNew.x);
			float bank = (gyroNew.z < 0) ? Mathf.Min(gyro.z, gyroNew.z) : Mathf.Max(gyro.z, gyroNew.z);*/
			float upDown = calibrated.x;
			float leftRight = calibrated.y;
//			float bank = calibrated.z;
			if (Mathf.Abs(upDown) > accelerometerThreshold) {
				float upDownDelta = Mathf.Clamp ( (Mathf.Abs(upDown)-accelerometerThreshold) * 10.0f, 0, 1.0f);
				rigidbody.AddRelativeTorque(-Vector3.right * Time.deltaTime * FORCE_TURN * upDownDelta * Mathf.Sign(upDown));
			}
			if (Mathf.Abs(leftRight) > accelerometerThreshold) {
				float leftRightDelta = Mathf.Clamp ( (Mathf.Abs(leftRight)-accelerometerThreshold) * 10.0f, 0, 1.0f);
				rigidbody.AddRelativeTorque(Vector3.up * Time.deltaTime * FORCE_TURN * leftRightDelta * Mathf.Sign(leftRight));
			}
/*			if (Mathf.Abs(bank) > accelerometerThreshold) {
				float bankDelta = Mathf.Clamp ( (Mathf.Abs(bank)-accelerometerThreshold) * 10.0f, 0, 1.0f);
				rigidbody.AddRelativeTorque(Vector3.forward * Time.deltaTime * FORCE_TURN * bankDelta * Mathf.Sign(bank));
			}*/
			
		} else {
			if (Input.GetKey(KeyCode.A)) {
	//				transform.Translate(-0.1f,0f,0f);
				rigidbody.AddRelativeForce(-Vector3.right * Time.deltaTime * FORCE_MOVE);
			}
			if (Input.GetKey(KeyCode.D)) {
	//				transform.Translate(0.1f,0f,0f);
				rigidbody.AddRelativeForce(Vector3.right * Time.deltaTime * FORCE_MOVE);
			}
			if (Input.GetKey(KeyCode.UpArrow)) {
	//				transform.Rotate(1f,0f,0f);
				rigidbody.AddRelativeTorque(Vector3.right * Time.deltaTime * FORCE_TURN);
			}
			if (Input.GetKey(KeyCode.DownArrow)) {
	//				transform.Rotate(-1f,0f,0f);
				rigidbody.AddRelativeTorque(-Vector3.right * Time.deltaTime * FORCE_TURN);
			}
			if (Input.GetKey(KeyCode.W)) {
				rigidbody.AddRelativeForce(Vector3.forward * Time.deltaTime * FORCE_MOVE);
	//				transform.Translate(0f,0f,0.1f);
			}
			if (Input.GetKey(KeyCode.S)) {
	//				transform.Translate(0f,0f,-0.1f);
				rigidbody.AddRelativeForce(-Vector3.forward * Time.deltaTime * FORCE_MOVE);
			}
			if (Input.GetKey(KeyCode.Q)) {
				rigidbody.AddRelativeTorque(Vector3.forward * Time.deltaTime * FORCE_TURN);
	//				transform.Translate(0f,0f,0.1f);
			}
			if (Input.GetKey(KeyCode.E)) {
				rigidbody.AddRelativeTorque(-Vector3.forward * Time.deltaTime * FORCE_TURN);
	//				transform.Translate(0f,0f,0.1f);
			}
			if (Input.GetKey(KeyCode.LeftArrow)) {
	//			transform.Rotate(0f,-1,0f);
				rigidbody.AddRelativeTorque(-Vector3.up * Time.deltaTime * FORCE_TURN);
			}
			if (Input.GetKey(KeyCode.RightArrow)) {
	//			transform.Rotate(0f,1f,0f);
				rigidbody.AddRelativeTorque(Vector3.up * Time.deltaTime * FORCE_TURN);
			}
	//		Debug.Log ("velocity " + rigidbody.velocity);
		}
	}
	
	void OnGUI() {
		if (GUI.RepeatButton  (new Rect (60,400,50,50), "Exit"))
			Application.Quit();
/*
		if (GUI.RepeatButton  (new Rect (60,10,50,50), "W"))
			transform.Translate(0,0,0.1f);
		if (GUI.RepeatButton  (new Rect (10,60,50,50), "A"))
			transform.Translate(-0.1f,0,0);
		if (GUI.RepeatButton  (new Rect (60,60,50,50), "S"))
			transform.Translate(0,0,-0.1f);
		if (GUI.RepeatButton  (new Rect (110,60,50,50), "D"))
			transform.Translate(0.1f,0,0);
		if (GUI.RepeatButton  (new Rect (640,420,50,50), "L"))
			transform.Rotate(0,-1f,0);
		if (GUI.RepeatButton  (new Rect (690,420,50,50), "D"))
			transform.Rotate(-1f,0,0);
		if (GUI.RepeatButton  (new Rect (740,420,50,50), "R"))
			transform.Rotate(0,1f,0);
		if (GUI.RepeatButton  (new Rect (690,370,50,50), "U"))
			transform.Rotate(1f,0,0);*/
	}
	
	private void Calibrate() {
		if (Input.acceleration != Vector3.zero) {
//			calibration = Quaternion.FromToRotation(Input.acceleration, new Vector3(0,0,-1.0f));
			calibration = Input.gyro.attitude;
			isCalibrated = true;
		}
//		Debug.Log ("Calibration " + Input.acceleration +" " + calibration);
	}
	
}

