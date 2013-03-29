using System;
using UnityEngine;

public class GameInput : MonoBehaviour {
	
	public bool isMobile = false;
	public RaycastHit hit;
	
	public int fingerCount = 0;
	public int[] fingerIDs = new int[maxTouchFingers];
	public bool[] isTouchDown = new bool[maxTouchFingers];
	public bool[] wasTouchDown = new bool[maxTouchFingers];
	public bool[] isTouchMoved = new bool[maxTouchFingers];
	public bool[] isTouchUp = new bool[maxTouchFingers];
	public Vector2[] touchPosition = new Vector2[maxTouchFingers];
	public Vector2[] touchPositionDelta = new Vector2[maxTouchFingers];
	public Vector2[] oldTouchPosition = new Vector2[maxTouchFingers];
	public Vector2[] startTouchPosition = new Vector2[maxTouchFingers];
	public bool[] isGUIClicked = new bool[maxTouchFingers];
	
	public Transform dragged = null;
	public Transform selected = null;
	public Transform crossSelected = null;
	public Transform hitTransform = null;
	
	private Game game;
	private State state;
//	private MyGUI myGUI;
	private Camera guiCamera;
	
	private float guiRaycastLength;
	private bool isGUIRegistered;
//	private float frameCount;
//	private float frameTimer;
	
	private static int maxTouchFingers = 2;
//	private static int collidableLayer = 9;
//	private static int selectableLayer = 10;
	private static int gui3DLayer = 12;
	
	void Awake() {	
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			isMobile = true;
		}
		guiCamera = GameObject.Find("/GUI Camera").GetComponent<Camera>();
		SetGUIRaycastLength(0f);
		isGUIRegistered = false;
	}
	
	void Start() {
		game =  GameObject.Find("/Game(Clone)").GetComponent<Game>();
		state = game.state;
//		frameTimer = Time.time;
	}
	
	void Update () {
/*		if (frameTimer + 1.0f < Time.time) {
			frameCount = 1.0f / Time.deltaTime;
			frameTimer = Time.time;
		}*/
		
		for (int i=0; i< maxTouchFingers; i++) {
			isTouchMoved[i] = false;
			isTouchDown[i] = false;
			isTouchUp[i] = false;
		}
		hitTransform = null;
	
		if (isMobile) {
			foreach (Touch touch in Input.touches) {
				// we accept only maxTouchFingers
				if (touch.fingerId < maxTouchFingers) {
					if (touch.phase == TouchPhase.Began) {
						isTouchDown[touch.fingerId] = true;
						wasTouchDown[touch.fingerId] = true;
						touchPosition[touch.fingerId] = touch.position;
						startTouchPosition[touch.fingerId] = touch.position;
						if (fingerCount+1 <= maxTouchFingers) fingerCount++;
					}
					if (touch.phase == TouchPhase.Moved) {
						isTouchMoved[touch.fingerId] = true;
						touchPosition[touch.fingerId] = touch.position;
						touchPositionDelta[touch.fingerId] = touch.deltaPosition;
					}
/*					if (touch.phase == TouchPhase.Stationary) {
						isTouchDown[touch.fingerId] = true;
						wasTouchDown[touch.fingerId] = true;
						touchPosition[touch.fingerId] = touch.position;
					}*/
					if (touch.phase == TouchPhase.Ended) {
						isTouchUp[touch.fingerId] = true;
						touchPosition[touch.fingerId] = touch.position;
						wasTouchDown[touch.fingerId] = false;
						fingerCount--;
						// to prevent strange phenomenons after level loading - some touch events seem to linger on
						if (fingerCount < 0) fingerCount = 0;
					}
				}
			}
		} else {
			touchPosition[0] = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			if (Input.GetButtonDown("Fire1")) {
				isTouchDown[0] = true;
				wasTouchDown[0] = true;
				startTouchPosition[0] = touchPosition[0];
				if (fingerCount+1 <= maxTouchFingers) fingerCount++;
				Screen.lockCursor = true;
			}
			if (Input.GetButtonUp("Fire1")) {
				isTouchUp[0] = true;
				wasTouchDown[0] = false;
				fingerCount--;
			}
			if (wasTouchDown[0]) {
				if (oldTouchPosition[0] == Vector2.zero) {
					oldTouchPosition[0] = touchPosition[0];
				}
				touchPositionDelta[0] = touchPosition[0] - oldTouchPosition[0];
				if ( touchPositionDelta[0] != Vector2.zero ) {
					isTouchMoved[0] = true;
				}
			}
		}
	
		// check for selection
		for (int i=0; i< maxTouchFingers; i++) {
			if (isTouchDown[i]) {
				isGUIClicked[i] = IsGUIClicked(i);
				if (!isGUIClicked[i]) {
					if (state.gameMode == Game.Mode.Dialog) {
					//	game.NonGUIClickInDialog();
					}
				}
			}
			
			oldTouchPosition[i] = touchPosition[i];
		}
		
		game.DispatchGameInput();
		
/*		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			state.lang++;
			if (state.lang == 10) {
				state.lang = 0;
			}
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			state.lang--;
			if (state.lang == -1) {
				state.lang = 9;
			}
		}*/
	}
	
	private bool IsGUIClicked(int finger) {
		bool result = false;
//		Debug.Log (guiRaycastLength);
		if (isGUIRegistered && Physics.Raycast(guiCamera.ScreenPointToRay(touchPosition[finger]), out hit, guiRaycastLength, 1 << gui3DLayer)) {
			if (hit.collider != null) {
				if (hit.collider.tag == "Select1Up") {
//					myGUI.SendTouchDown(hit.collider.transform.parent.gameObject, finger);
					result = true;
				} else if (hit.collider.tag == "Select") {
//					myGUI.SendTouchDown(hit.collider.gameObject, finger);
					result = true;
				} else if (hit.collider.tag == "Knob") {
					hit.collider.gameObject.SendMessage("Select", finger);
					result = true;
				}
			}
		}	
		return result;
	}
	
	public void SetGUIRaycastLength(float maxZ) {
		guiRaycastLength = Mathf.Abs(guiCamera.transform.position.z - maxZ);
//		Debug.Log ("guiRaycastLength " + guiRaycastLength + ", " + maxZ);
	}
	
	public void RegisterGUI(MyGUI mG) {
//		myGUI = mG;
		isGUIRegistered = true;
	}
	
	public void DeRegisterGUI() {
		isGUIRegistered = false;
	}
	
/*	void OnGUI () {
	    GUI.TextField(new Rect (10,10,100,25), "" + frameCount);
	}*/

}

