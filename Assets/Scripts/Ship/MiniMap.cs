using System;
using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour {
	
	private GameInput gameInput;
	private Ship ship;
	private Play play;
	private Transform miniMapShip;
	private Camera miniMapCamera;
	private float mouseSensitivity;
		
	private Mode mode;
	private int moveBitwise; // either roll in 1finger mode, or shift in 2finger mode
	private int moveZoomIn = 1;
	private int moveZoomOut = 2;
	private int moveYawLeft = 4;
	private int moveYawRight = 8;
	private int moveTiltUp = 16;
	private int moveTiltDown = 32;
	private int moveTurnLeft = 64;
	private int moveTurnRight = 128;
	
	private int primaryTouchFinger;
	private int secondaryTouchFinger;
	private Vector2 touchPosition1;
	private Vector2 touchPosition2;
	private float touchTime1;
	private float touchTime2;
	private Vector3 touchDelta1;
	private Vector3 touchDelta2;
	private bool usesMouse;
	
	private static float MINI_MAP_SCALE = 0.2f;
	private static Vector3 MINI_MAP_POSITION = new Vector3(0f, 0f, 2.0f);
	private static float ZOOM_MODIFIER = 1.0f;
	private static float ZOOM_MAX = 5.0f;
	private static float ZOOM_MIN = 0.5f;
//	private static float TOUCH_THRESHOLD = Screen.dpi * 0.2f;
//	private static float TOUCH_SENSITIVITY = Screen.dpi * 0.5f;
//	private static float TOUCH_TIME_THRESHOLD = 0.3f;

	public enum Mode { Off=0, On=1 }
	
	void Awake() {
		miniMapShip = transform.Find("Ship");
		transform.localScale *= MINI_MAP_SCALE;
	}
		
	public void Initialize(Ship s, Play p, GameInput gI, Camera c) {
		ship = s;
		gameInput = gI;
		play = p;
		miniMapCamera = c;
		
		moveBitwise = 0;
		primaryTouchFinger = 0;
		secondaryTouchFinger = 1;
		usesMouse = true;
		mouseSensitivity = play.game.state.GetPreferenceMiniMapMouseSensitivity();
		SwitchOff();
	}
	
	void FixedUpdate() {
		if (mode == Mode.On) {
			UpdatePosition();
		}
	}
	
	public void SwitchOn() {
		UpdatePosition();
		Room r = play.GetRoomOfShip();
		Mesh m = new Mesh();
		m.vertices = r.roomMesh.mesh.vertices;
		m.triangles = r.roomMesh.mesh.triangles;
		m.uv = r.roomMesh.mesh.uv;
		GetComponent<MeshFilter>().mesh = m;
		mode = Mode.On;
		miniMapCamera.enabled = true;
	}
	
	public void SwitchOff() {
		mode = Mode.Off;
		miniMapCamera.enabled = false;
	}
	
	private void UpdatePosition() {
		miniMapShip.position = (play.GetShipPosition() / RoomMesh.MESH_SCALE) * MINI_MAP_SCALE;
	}
	
	private void Rotate() {
		miniMapCamera.transform.RotateAround(miniMapShip.position, miniMapCamera.transform.TransformDirection(Vector3.up), -Input.GetAxis ("Mouse X") * mouseSensitivity * Time.deltaTime);
		miniMapCamera.transform.RotateAround(miniMapShip.position,miniMapCamera.transform.TransformDirection(Vector3.right), -Input.GetAxis ("Mouse Y") * mouseSensitivity * Time.deltaTime);
	}
	
	private void Zoom() {
		float zoom = Input.GetAxis("Mouse ScrollWheel");
		if (zoom != 0) {
			Vector3 toShip = miniMapShip.position - miniMapCamera.transform.position;
			float move = Mathf.Clamp(toShip.magnitude + zoom * ZOOM_MODIFIER, ZOOM_MIN, ZOOM_MAX);
			miniMapCamera.transform.position = miniMapShip.position;
			miniMapCamera.transform.Translate(-Vector3.forward * move);
		}
	}
	
	public void DispatchGameInput() {
		if (mode == Mode.On) {
			if (gameInput.isMobile) {
			} else {
				if (Input.GetKey(KeyCode.LeftAlt)) {
					Rotate();
					Zoom();
				}				
			}
		}
	}
	
}
