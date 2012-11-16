using System;
using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	private Game game;
	private Play play;
	private GameInput gameInput;
	
	private Rigidbody rigidbody;
	private FlyingMode flyingMode;
//	private DirectionMode directionMode;
	private int flyingBitwise;
	private int flyingLeft = 1;
	private int flyingRight = 2;
	private int flyingUp = 4;
	private int flyingDown = 8;
	private int directionBitwise;
	private int directionFinger1Down = 1;
	private int directionFinger2Down = 2;
	private int directionForward = 4;
	private int directionBackward = 8;
	
	private Vector3 forceMove;
	private Vector3 forceTurn;
	private Quaternion calibration;
	private bool isCalibrated;
	private Vector3 gyro;
	private int primaryTouchFinger;
	private int secondaryTouchFinger;
	private Vector2 touchPosition;
	private float primaryTouchTime;
	private float secondaryTouchTime;
	
	private static float FORCE_MOVE = 10.0f;
	private static float FORCE_TURN = 0.5f;
	private static float accelerometerThreshold = 0.05f;
	private static float TOUCH_THRESHOLD = Screen.dpi * 0.2f;
	private static float TOUCH_TIME_THRESHOLD = 0.3f;

	public enum DirectionMode { Stopped=0, Forward=1, Backward=2 }
	public enum FlyingMode { None=0, ShiftLeft=5, ShiftRight=6, ShiftUp=7, ShiftDown=8, BankLeft=9, BankRight=10 }
	
	void Awake() {
		Application.targetFrameRate = 60;
//		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		rigidbody = GetComponent<Rigidbody>();
		forceMove = Vector3.zero;
		forceTurn = Vector3.zero;
		gyro = Vector3.zero;
		
		isCalibrated = false;
		Debug.Log (Screen.dpi);
	}
	
	public void Initialize(Play p, Game g) {
		play = p;
		game = g;
		gameInput = game.gameInput;
		if (gameInput.isMobile) {
			Input.gyro.enabled = true;
		}
		flyingBitwise = 0;
		directionBitwise = 0;
//		directionMode = DirectionMode.Stopped;
		primaryTouchFinger = 0;
		secondaryTouchFinger = 1;
	}
	
	void OnTriggerEnter(Collider collider) {
//		Debug.Log ("OnTriggerEnter");
	}
	
	void OnTriggerExit(Collider collider) {
//		Debug.Log ("OnTriggerExit");
	}

	public void DispatchGameInput() {
		if (gameInput.isMobile) {
			if (directionBitwise == 0 && gameInput.isTouchDown[primaryTouchFinger] && !gameInput.isGUIClicked[primaryTouchFinger]) {
				//directionMode = DirectionMode.Forward;
				directionBitwise |= directionFinger1Down;
				touchPosition = gameInput.touchPosition[primaryTouchFinger];
				primaryTouchTime = Time.time;
//				Debug.Log("first touch " + directionBitwise);
			}
			if ( (directionBitwise & directionFinger2Down) == directionFinger2Down) {
				if (gameInput.isTouchUp[primaryTouchFinger] && gameInput.isTouchUp[secondaryTouchFinger]) {
					directionBitwise = 0;
				} else if (gameInput.isTouchUp[primaryTouchFinger]) {
//					Debug.Log("before " + directionBitwise);
					directionBitwise &= ~directionFinger2Down; // delete bit
					directionBitwise &= ~directionBackward;
					directionBitwise |= directionForward;
//					Debug.Log("after " + directionBitwise);
					primaryTouchFinger = 1;
					secondaryTouchFinger = 0;
				} else if (gameInput.isTouchUp[secondaryTouchFinger]) {
					directionBitwise &= ~directionFinger2Down;
					directionBitwise &= ~directionBackward;
					directionBitwise |= directionForward;
				}
			} else if ( (directionBitwise & directionFinger1Down) == directionFinger1Down) {
				if (gameInput.isTouchUp[primaryTouchFinger]) {
					directionBitwise = 0;
					flyingBitwise = 0;
					primaryTouchFinger = 0;
					secondaryTouchFinger = 1;
				} else {
					flyingBitwise = 0;
					if (gameInput.touchPosition[primaryTouchFinger].y - touchPosition.y > TOUCH_THRESHOLD) {
//						flyingMode = FlyingMode.Up;
						flyingBitwise |= flyingUp;
					} else if (gameInput.touchPosition[primaryTouchFinger].y - touchPosition.y < -TOUCH_THRESHOLD) {
//						flyingMode = FlyingMode.Down;
						flyingBitwise |= flyingDown;
					}
					if (gameInput.touchPosition[primaryTouchFinger].x - touchPosition.x > TOUCH_THRESHOLD) {
//						flyingMode = FlyingMode.Right;
						flyingBitwise |= flyingRight;
					} else if (gameInput.touchPosition[primaryTouchFinger].x - touchPosition.x < -TOUCH_THRESHOLD) {
//						flyingMode = FlyingMode.Left;
						flyingBitwise |= flyingLeft;
					}
					if (flyingBitwise == 0 && (directionBitwise & directionForward) != directionForward) {
						if (Time.time > primaryTouchTime + TOUCH_TIME_THRESHOLD) {
							directionBitwise |= directionForward;
						}
					} else {
						primaryTouchTime = Time.time;
					}
				}
				if (gameInput.isTouchDown[secondaryTouchFinger] && !gameInput.isGUIClicked[secondaryTouchFinger]) {
					//directionMode = DirectionMode.Backward;
					directionBitwise |= directionFinger2Down;
					directionBitwise |= directionBackward;
					directionBitwise &= ~directionForward;
//					Debug.Log("second touch " + directionBitwise);
				}
			}
		}
	}

	void Update () {
		if (!isCalibrated) Calibrate();
		
		if (!gameInput.isMobile) {
			directionBitwise = 0;
			flyingBitwise = 0;
			if (Input.GetKey(KeyCode.A)) {
//				rigidbody.AddRelativeForce(-Vector3.right * FORCE_MOVE);
			}
			if (Input.GetKey(KeyCode.D)) {
//				rigidbody.AddRelativeForce(Vector3.right * FORCE_MOVE);
			}
			
			if (Input.GetKeyUp(KeyCode.W)) {
				directionBitwise &= ~directionForward;
			}
			if (Input.GetKeyUp(KeyCode.S)) {
//				directionMode = DirectionMode.Stopped;
				directionBitwise &= ~directionBackward;
			}
			if (Input.GetKey(KeyCode.W)) {
//				directionMode = DirectionMode.Forward;
				directionBitwise |= directionForward;
				directionBitwise &= ~directionBackward;
			}
			if (Input.GetKey(KeyCode.S)) {
//				directionMode = DirectionMode.Backward;
				directionBitwise |= directionBackward;
				directionBitwise &= ~directionForward;
			}
			
/*			if (Input.GetKey(KeyCode.Q)) {
				rigidbody.AddRelativeTorque(Vector3.forward * FORCE_TURN);
			}
			if (Input.GetKey(KeyCode.E)) {
				rigidbody.AddRelativeTorque(-Vector3.forward * FORCE_TURN);
			}*/
			
			if (Input.GetKeyUp(KeyCode.LeftArrow)) {
				flyingBitwise &= ~flyingLeft;
			}
			if (Input.GetKeyUp(KeyCode.RightArrow)) {
				flyingBitwise &= ~flyingRight;
			}
			if (Input.GetKeyUp(KeyCode.UpArrow)) {
				flyingBitwise &= ~flyingUp;
			}
			if (Input.GetKeyUp(KeyCode.DownArrow)) {
				flyingBitwise &= ~flyingDown;				
			}
			if (Input.GetKey(KeyCode.LeftArrow)) {
//				flyingMode = FlyingMode.Left;
				flyingBitwise |= flyingLeft;
			}
			if (Input.GetKey(KeyCode.RightArrow)) {
//				flyingMode = FlyingMode.Right;
				flyingBitwise |= flyingRight;
			}
			if (Input.GetKey(KeyCode.UpArrow)) {
				//flyingMode = FlyingMode.Up;
				flyingBitwise |= flyingUp;
			}
			if (Input.GetKey(KeyCode.DownArrow)) {
//				flyingMode = FlyingMode.Down;
				flyingBitwise |= flyingDown;
			}
		}
	}
	
	
	void FixedUpdate () {
		if ((directionBitwise & directionForward) == directionForward) {
			Move(Vector3.forward);
		} else if ((directionBitwise & directionBackward) == directionBackward) {
			Move(-Vector3.forward);
		}
		if ( (flyingBitwise & flyingLeft) == flyingLeft) {
			Turn(-Vector3.up);
		} else if ( (flyingBitwise & flyingRight) == flyingRight) {
			Turn(Vector3.up);
		}
		if ( (flyingBitwise & flyingUp) == flyingUp) {
			Turn(Vector3.right);
		} else if ( (flyingBitwise & flyingDown) == flyingDown) {
			Turn(-Vector3.right);
		}
		
//		if (gameInput.isMobile) {
			/*
			Quaternion calibrated = Quaternion.Inverse(calibration) * Input.gyro.attitude;
			float upDown = calibrated.x;
			float leftRight = calibrated.y;
			if (Mathf.Abs(upDown) > accelerometerThreshold) {
				float upDownDelta = Mathf.Clamp ( (Mathf.Abs(upDown)-accelerometerThreshold) * 10.0f, 0, 1.0f);
				rigidbody.AddRelativeTorque(-Vector3.right * Time.deltaTime * FORCE_TURN * upDownDelta * Mathf.Sign(upDown));
			}
			if (Mathf.Abs(leftRight) > accelerometerThreshold) {
				float leftRightDelta = Mathf.Clamp ( (Mathf.Abs(leftRight)-accelerometerThreshold) * 10.0f, 0, 1.0f);
				rigidbody.AddRelativeTorque(Vector3.up * Time.deltaTime * FORCE_TURN * leftRightDelta * Mathf.Sign(leftRight));
			}*/		
//		}
	}
	
	private void Move(Vector3 direction) {
		rigidbody.AddRelativeForce(direction * FORCE_MOVE);
	}
	
	private void Turn(Vector3 direction) {
		rigidbody.AddRelativeTorque(direction * FORCE_TURN);
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


