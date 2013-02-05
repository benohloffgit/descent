using System;
using UnityEngine;
using System.Collections;

public class ShipSteering : MonoBehaviour {
	
	private GameInput gameInput;
	private Ship ship;
	
	private Rigidbody myRigidbody;
	private FlyingMode flyingMode;
	private int flyingBitwise; // either roll in 1finger mode, or shift in 2finger mode
	private int flyingLeft = 1;
	private int flyingRight = 2;
	private int flyingUp = 4;
	private int flyingDown = 8;
	private int flyingShiftLeft = 16;
	private int flyingShiftRight = 32;
	private int flyingShiftUp = 64;
	private int flyingShiftDown = 128;
	private int flyingYawLeft = 256;
	private int flyingYawRight = 512;
	private int directionBitwise;
	private int directionFinger1Down = 1;
	private int directionFinger2Down = 2;
	private int directionForward = 4;
	private int directionBackward = 8;
	private int directionTurn = 16;
	private int directionShift = 32;
	
	private int primaryTouchFinger;
	private int secondaryTouchFinger;
	private Vector2 touchPosition1;
	private Vector2 touchPosition2;
	private float touchTime1;
	private float touchTime2;
	private Vector3 touchDelta1;
	private Vector3 touchDelta2;
	private bool usesMouse;

	private static float TOUCH_THRESHOLD = Screen.dpi * 0.2f;
	private static float TOUCH_SENSITIVITY = Screen.dpi * 0.5f;
	private static float TOUCH_TIME_THRESHOLD = 0.3f;

	public enum DirectionMode { Stopped=0, Forward=1, Backward=2 }
	public enum FlyingMode { None=0, ShiftLeft=5, ShiftRight=6, ShiftUp=7, ShiftDown=8, BankLeft=9, BankRight=10 }
	
	void Awake() {
		myRigidbody = GetComponent<Rigidbody>();
	}
	
	public void Initialize(Ship s, GameInput gI) {
		ship = s;
		gameInput = gI;
		
		flyingBitwise = 0;
		directionBitwise = 0;
		primaryTouchFinger = 0;
		secondaryTouchFinger = 1;
		usesMouse = true;
	}
	
	public void DispatchGameInput() {
		if (gameInput.isMobile) {
			if (directionBitwise == 0 && gameInput.isTouchDown[primaryTouchFinger] && !gameInput.isGUIClicked[primaryTouchFinger]) {
				directionBitwise |= directionFinger1Down;
				touchPosition1 = gameInput.touchPosition[primaryTouchFinger];
				touchTime1 = Time.time;
			}
			if ( (directionBitwise & directionFinger2Down) == directionFinger2Down) {
				if (gameInput.isTouchUp[primaryTouchFinger] && gameInput.isTouchUp[secondaryTouchFinger]) {
					directionBitwise = 0;
				} else if (gameInput.isTouchUp[primaryTouchFinger]) {
					directionBitwise &= ~directionFinger2Down; // delete bit
					directionBitwise &= ~directionBackward;
					directionBitwise &= ~directionShift;
					primaryTouchFinger = 1;
					secondaryTouchFinger = 0;
					// reset touchPosition so we don't go directly into any curves
					touchPosition1 = gameInput.touchPosition[secondaryTouchFinger];
				} else if (gameInput.isTouchUp[secondaryTouchFinger]) {
					directionBitwise &= ~directionFinger2Down;
					directionBitwise &= ~directionBackward;
					directionBitwise &= ~directionShift;
					// reset touchPosition so we don't go directly into any curves
					touchPosition1 = gameInput.touchPosition[primaryTouchFinger];
				} else {
					flyingBitwise = 0;
					touchDelta1 = gameInput.touchPosition[primaryTouchFinger] - touchPosition1;
					touchDelta2 = gameInput.touchPosition[secondaryTouchFinger] - touchPosition2;
					if (touchDelta1.y > TOUCH_THRESHOLD) {
						if (touchDelta2.y < -TOUCH_THRESHOLD) {
							if (touchPosition1.x < touchPosition2.x) { // we dont know if right or left finger was touched first
								flyingBitwise |= flyingYawRight;
							} else {
								flyingBitwise |= flyingYawLeft;
							}
						} else if (Mathf.Sign(touchDelta1.y) == Mathf.Sign(touchDelta2.y)) { // only if both fingers move in same direction
							flyingBitwise |= flyingShiftUp;
						}
					} else if (touchDelta1.y < -TOUCH_THRESHOLD) {
						if (touchDelta2.y > TOUCH_THRESHOLD) {
							if (touchPosition1.x < touchPosition2.x) {  // we dont know if right or left finger was touched first
								flyingBitwise |= flyingYawLeft;
							} else {
								flyingBitwise |= flyingYawRight;
							}
						} else if (Mathf.Sign(touchDelta1.y) == Mathf.Sign(touchDelta2.y)) { // only if both fingers move in same direction
							flyingBitwise |= flyingShiftDown;
						}
					}
					if (touchDelta1.x > TOUCH_THRESHOLD) {
						flyingBitwise |= flyingShiftRight;
					} else if (touchDelta1.x < -TOUCH_THRESHOLD) {
						flyingBitwise |= flyingShiftLeft;
					}
					Debug.Log (touchDelta1 +"/"+touchDelta2 + " " + flyingBitwise);
					if (	flyingBitwise != 0 && Time.time < touchTime2 + TOUCH_TIME_THRESHOLD
							&& (directionBitwise & directionForward) != directionForward
							&& (directionBitwise & directionShift) != directionShift) {
						directionBitwise |= directionShift;
					} else if (		(directionBitwise & directionBackward) != directionBackward
									&& (directionBitwise & directionForward) != directionForward
									&& (directionBitwise & directionShift) != directionShift
									&& Time.time > touchTime2 + TOUCH_TIME_THRESHOLD) {
						directionBitwise |= directionBackward;
					}
				}
			} else if ( (directionBitwise & directionFinger1Down) == directionFinger1Down) {
				if (gameInput.isTouchUp[primaryTouchFinger]) {
					directionBitwise = 0;
					flyingBitwise = 0;
					primaryTouchFinger = 0;
					secondaryTouchFinger = 1;
				} else {
					flyingBitwise = 0;
					touchDelta1 = gameInput.touchPosition[primaryTouchFinger] - touchPosition1;
					if (touchDelta1.y > TOUCH_THRESHOLD) {
						flyingBitwise |= flyingUp;
					} else if (touchDelta1.y < -TOUCH_THRESHOLD) {
						flyingBitwise |= flyingDown;
					}
					if (touchDelta1.x > TOUCH_THRESHOLD) {
						flyingBitwise |= flyingRight;
					} else if (touchDelta1.x < -TOUCH_THRESHOLD) {
						flyingBitwise |= flyingLeft;
					}
					if (flyingBitwise != 0 && Time.time < touchTime1 + TOUCH_TIME_THRESHOLD) {
						directionBitwise |= directionTurn;
					} else if ((directionBitwise & directionForward) != directionForward
									&& (directionBitwise & directionTurn) != directionTurn
									&& Time.time > touchTime1 + TOUCH_TIME_THRESHOLD) {
						directionBitwise |= directionForward;
					}
				}
				if (gameInput.isTouchDown[secondaryTouchFinger] && !gameInput.isGUIClicked[secondaryTouchFinger]) {
					directionBitwise |= directionFinger2Down;
					touchTime2 = Time.time;
					touchPosition2 = gameInput.touchPosition[secondaryTouchFinger];
				}
			}
		} else {
			directionBitwise = 0;
			flyingBitwise = 0;
			if (Input.GetKeyUp(KeyCode.A)) {
				flyingBitwise &= ~flyingShiftLeft;
			}
			if (Input.GetKeyUp(KeyCode.D)) {
				flyingBitwise &= ~flyingShiftRight;
			}
			if (Input.GetKeyUp(KeyCode.R)) {
				flyingBitwise &= ~flyingShiftUp;
			}
			if (Input.GetKeyUp(KeyCode.F)) {
				flyingBitwise &= ~flyingShiftDown;
			}
				
			if (Input.GetKey(KeyCode.A)) {
				flyingBitwise |= flyingShiftLeft;
				flyingBitwise &= ~flyingShiftRight;
			}
			if (Input.GetKey(KeyCode.D)) {
				flyingBitwise |= flyingShiftRight;
				flyingBitwise &= ~flyingShiftLeft;
			}
			if (Input.GetKey(KeyCode.R)) {
				flyingBitwise |= flyingShiftUp;
				flyingBitwise &= ~flyingShiftDown;
			}
			if (Input.GetKey(KeyCode.F)) {
				flyingBitwise |= flyingShiftDown;
				flyingBitwise &= ~flyingShiftUp;
			}
			
			if (Input.GetKeyUp(KeyCode.W)) {
				directionBitwise &= ~directionForward;
			}
			if (Input.GetKeyUp(KeyCode.S)) {
				directionBitwise &= ~directionBackward;
			}
			if (Input.GetKey(KeyCode.W)) {
				directionBitwise |= directionForward;
				directionBitwise &= ~directionBackward;
			}
			if (Input.GetKey(KeyCode.S)) {
				directionBitwise |= directionBackward;
				directionBitwise &= ~directionForward;
			}
			
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
				flyingBitwise |= flyingLeft;
				flyingBitwise &= ~flyingRight;
			}
			if (Input.GetKey(KeyCode.RightArrow)) {
				flyingBitwise |= flyingRight;
				flyingBitwise &= ~flyingLeft;
			}
			if (Input.GetKey(KeyCode.UpArrow)) {
				flyingBitwise |= flyingUp;
				flyingBitwise &= ~flyingDown;				
			}
			if (Input.GetKey(KeyCode.DownArrow)) {
				flyingBitwise |= flyingDown;
				flyingBitwise &= ~flyingUp;
			}

			if (Input.GetKey(KeyCode.E)) {
				flyingBitwise |= flyingYawLeft;
				flyingBitwise &= ~flyingYawRight;
			}
			if (Input.GetKey(KeyCode.Q)) {
				flyingBitwise |= flyingYawRight;
				flyingBitwise &= ~flyingYawLeft;
			}
		}
	}

	void Update () {
//		Debug.DrawRay(collisionPoint, collisionNormal*5.0f, Color.white);
//		if (!isCalibrated) Calibrate();
		
		if (!gameInput.isMobile) {

		}
	}
	
	void FixedUpdate () {
		if (gameInput.isMobile) {
			if ((directionBitwise & directionFinger2Down) == directionFinger2Down) {
				if ((directionBitwise & directionBackward) == directionBackward) {
					ship.Move(-Vector3.forward);
				} else if ((directionBitwise & directionForward) == directionForward) {
					ship.Move(Vector3.forward);
				}
				if ( (flyingBitwise & flyingShiftLeft) == flyingShiftLeft) {
					ship.Move(-Vector3.right * Mathf.Clamp01((Mathf.Abs(touchDelta1.x)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY) );
				} else if ( (flyingBitwise & flyingShiftRight) == flyingShiftRight) {
					ship.Move(Vector3.right * Mathf.Clamp01((Mathf.Abs(touchDelta1.x)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				}
				if ( (flyingBitwise & flyingShiftUp) == flyingShiftUp) {
					ship.Move(Vector3.up * Mathf.Clamp01((Mathf.Abs(touchDelta1.y)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				} else if ( (flyingBitwise & flyingShiftDown) == flyingShiftDown) {
					ship.Move(-Vector3.up * Mathf.Clamp01((Mathf.Abs(touchDelta1.y)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				} else if ( (flyingBitwise & flyingYawLeft) == flyingYawLeft) {
					ship.Yaw(-Vector3.forward * Mathf.Clamp01((Mathf.Abs(touchDelta1.y)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				} else if ( (flyingBitwise & flyingYawRight) == flyingYawRight) {
					ship.Yaw(Vector3.forward * Mathf.Clamp01((Mathf.Abs(touchDelta1.y)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				}
			} else if ((directionBitwise & directionFinger1Down) == directionFinger1Down) {
				if ((directionBitwise & directionForward) == directionForward) {
					ship.Move(Vector3.forward);
				} else if ((directionBitwise & directionBackward) == directionBackward) {
					ship.Move(-Vector3.forward);
				}
				if ( (flyingBitwise & flyingLeft) == flyingLeft) {
					ship.Turn(-Vector3.up * Mathf.Clamp01((Mathf.Abs(touchDelta1.x)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				} else if ( (flyingBitwise & flyingRight) == flyingRight) {
					ship.Turn(Vector3.up * Mathf.Clamp01((Mathf.Abs(touchDelta1.x)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				}
				if ( (flyingBitwise & flyingUp) == flyingUp) {
					ship.Turn(Vector3.right * Mathf.Clamp01((Mathf.Abs(touchDelta1.y)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				} else if ( (flyingBitwise & flyingDown) == flyingDown) {
					ship.Turn(-Vector3.right * Mathf.Clamp01((Mathf.Abs(touchDelta1.y)-TOUCH_THRESHOLD)/TOUCH_SENSITIVITY));
				}
			}
		} else {
			if ((directionBitwise & directionBackward) == directionBackward) {
				ship.Move(-Vector3.forward);
			} else if ((directionBitwise & directionForward) == directionForward) {
				ship.Move(Vector3.forward);
			}
			if ( (flyingBitwise & flyingShiftLeft) == flyingShiftLeft) {
				ship.Move(-Vector3.right);
			} else if ( (flyingBitwise & flyingShiftRight) == flyingShiftRight) {
				ship.Move(Vector3.right);
			}
			if ( (flyingBitwise & flyingShiftUp) == flyingShiftUp) {
				ship.Move(Vector3.up);
			} else if ( (flyingBitwise & flyingShiftDown) == flyingShiftDown) {
				ship.Move(-Vector3.up);
			}
			if ( (flyingBitwise & flyingLeft) == flyingLeft) {
				ship.Turn(-Vector3.up);
			} else if ( (flyingBitwise & flyingRight) == flyingRight) {
				ship.Turn(Vector3.up);
			}
			if ( (flyingBitwise & flyingUp) == flyingUp) {
				ship.Turn(Vector3.right);
			} else if ( (flyingBitwise & flyingDown) == flyingDown) {
				ship.Turn(-Vector3.right);
			}
			if ( (flyingBitwise & flyingYawLeft) == flyingYawLeft) {
				ship.Yaw(-Vector3.forward);
			} else if ( (flyingBitwise & flyingYawRight) == flyingYawRight) {
				ship.Yaw(Vector3.forward);
			}
			
			if (usesMouse) {
				ship.Turn(Vector3.up * Input.GetAxis ("Mouse X"));
				ship.Turn(Vector3.right * Input.GetAxis ("Mouse Y"));
			}
		}
		
//		if (gameInput.isMobile) {
			/*
			Quaternion calibrated = Quaternion.Inverse(calibration) * Input.gyro.attitude;
			float upDown = calibrated.x;
			float leftRight = calibrated.y;
			if (Mathf.Abs(upDown) > accelerometerThreshold) {
				float upDownDelta = Mathf.Clamp ( (Mathf.Abs(upDown)-accelerometerThreshold) * 10.0f, 0, 1.0f);
				myRigidbody.AddRelativeTorque(-Vector3.right * Time.deltaTime * FORCE_TURN * upDownDelta * Mathf.Sign(upDown));
			}
			if (Mathf.Abs(leftRight) > accelerometerThreshold) {
				float leftRightDelta = Mathf.Clamp ( (Mathf.Abs(leftRight)-accelerometerThreshold) * 10.0f, 0, 1.0f);
				myRigidbody.AddRelativeTorque(Vector3.up * Time.deltaTime * FORCE_TURN * leftRightDelta * Mathf.Sign(leftRight));
			}*/		
//		}
	}
}