using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour {
	
	private GameInput gameInput;
	private Ship ship;
	private Play play;
	private Transform miniMapShip;
	private Camera miniMapCamera;
	private float mouseSensitivity;
	private bool isCameraRotatingToFollow;
	private float cameraFollowDistance;
	private IntTriple currentRoomPos;
	
	private Transform[] roomConnectors = new Transform[MAX_EXITS];
	private List<GameObject> breadcrumbs = new List<GameObject>();
	
	private static int MATERIAL_MINI_MAP = 0;
	private static int MATERIAL_MINI_MAP_SHIP = 1;
	private static int MATERIAL_MINI_MAP_ROOM_NOT_ENTERED = 2;
	private static int MATERIAL_MINI_MAP_ROOM_ENTERED = 3;
	private static int MATERIAL_MINI_MAP_BREADCRUMB = 4;
	private static int MATERIAL_MINI_MAP_ENTRYEXIT = 5;
		
	private Mode mode;
	private Follow follow;
//	private int moveBitwise; // either roll in 1finger mode, or shift in 2finger mode
/*	private int moveZoomIn = 1;
	private int moveZoomOut = 2;
	private int moveYawLeft = 4;
	private int moveYawRight = 8;
	private int moveTiltUp = 16;
	private int moveTiltDown = 32;
	private int moveTurnLeft = 64;
	private int moveTurnRight = 128;*/
	
//	private int primaryTouchFinger;
//	private int secondaryTouchFinger;
/*	private Vector2 touchPosition1;
	private Vector2 touchPosition2;
	private float touchTime1;
	private float touchTime2;
	private Vector3 touchDelta1;
	private Vector3 touchDelta2;*/
//	private bool usesMouse;
	
	private static int MAX_EXITS = 5;
	
	private static float MINI_MAP_SCALE = 0.2f;
	private static float ZOOM_MODIFIER = 1.0f;
	private static float ZOOM_MAX = 5.0f;
	private static float ZOOM_MIN = 0.5f;
	private static float CAMERA_FOLLOW_ROTATION_SPEED = 10.0f;

	public enum Mode { Off=0, On=1 }
	public enum Follow { Off=0, On=1 }
	
	void Awake() {
		miniMapShip = transform.Find("Ship");
		transform.localScale *= MINI_MAP_SCALE;
	}
	
	void Start() {
		SwitchFollowOn();
	}
		
	public void Initialize(Ship s, Play p, GameInput gI, Camera c) {
		ship = s;
		gameInput = gI;
		play = p;
		miniMapCamera = c;
		
//		moveBitwise = 0;
//		primaryTouchFinger = 0;
//		secondaryTouchFinger = 1;
//		usesMouse = true;
		isCameraRotatingToFollow = false;
		follow = Follow.Off;
		mouseSensitivity = play.game.state.GetPreferenceMiniMapMouseSensitivity();

		for (int i=0; i<MAX_EXITS; i++) {
			roomConnectors[i] = (GameObject.Instantiate(play.game.miniMapRoomConnectorPrefab, Vector3.zero, Quaternion.identity) as GameObject).transform;
			roomConnectors[i].parent = transform;
		}
		
		SwitchOff();
	}
	
	void FixedUpdate() {
		if (!play.isPaused) {
			if (play.isShipInPlayableArea) {
				if (play.GetRoomOfShip().pos != currentRoomPos) {
					ReadRoomData();
					currentRoomPos = play.GetRoomOfShip().pos;
				}
			} else {
				Reset();
				if (mode == Mode.On) {
					play.SwitchMiniMap();
				}
			}
			
			if (mode == Mode.On) {
				UpdatePosition();
				UpdateRotation();
			}
		}
	}
	
	public void Reset() {
		currentRoomPos = IntTriple.MINUSONE;
	}
	
	public void SwitchOn() {
		UpdatePosition();
		mode = Mode.On;
		miniMapCamera.enabled = true;
	}
	
	public void SwitchOff() {
		mode = Mode.Off;
		miniMapCamera.enabled = false;
	}
	
	public void SwitchFollowOn() {
		follow = Follow.On;
		isCameraRotatingToFollow = true;
		cameraFollowDistance = (miniMapShip.position - miniMapCamera.transform.position).magnitude;
	}
	
	public void SwitchFollowOff() {
		follow = Follow.Off;
		isCameraRotatingToFollow = false;
	}
	
	private void UpdatePosition() {
		miniMapShip.position = (play.GetShipPosition() / RoomMesh.MESH_SCALE) * MINI_MAP_SCALE;
	}
	
	private void UpdateRotation() {
		Vector3 camPos = miniMapCamera.transform.position;
		Quaternion camRot = miniMapCamera.transform.rotation;
		miniMapShip.rotation = ship.transform.rotation;
		if (follow == Follow.Off) {
			// preserve camera angle
			miniMapCamera.transform.position = camPos;
			miniMapCamera.transform.rotation = camRot;
		} else if (isCameraRotatingToFollow) {
			Vector3 rotateTo = miniMapShip.position - (miniMapShip.forward * cameraFollowDistance);
//			Debug.Log (rotateTo + " " + (miniMapShip.forward * toShip.magnitude).magnitude);
			miniMapCamera.transform.position = Vector3.Lerp(miniMapCamera.transform.position, rotateTo, CAMERA_FOLLOW_ROTATION_SPEED * Time.deltaTime);
			if (miniMapCamera.transform.position == rotateTo) {
				isCameraRotatingToFollow = false;
			}
			miniMapCamera.transform.LookAt(miniMapShip.position, miniMapShip.up);
		}
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
	
	private void ReadRoomData() {
		Room r = play.GetRoomOfShip();
		Mesh m = new Mesh();
		m.vertices = r.roomMesh.mesh.vertices;
		m.triangles = r.roomMesh.mesh.triangles;
		m.uv = r.roomMesh.mesh.uv;
		GetComponent<MeshFilter>().mesh = m;
//		Debug.Log ("MiniMap ReadRoomData room " + r.id);
		System.Collections.Generic.Dictionary<IntTriple, Cell>.Enumerator en = r.exits.GetEnumerator();
		en.MoveNext();
		for (int i=0; i<MAX_EXITS; i++) {
			if (r.exits.Count > i) {
				roomConnectors[i].position = (r.pos.GetVector3() * Game.DIMENSION_ROOM + en.Current.Value.pos.GetVector3()) * MINI_MAP_SCALE;
				roomConnectors[i].rotation = Quaternion.identity;
				play.cave.Align(roomConnectors[i], en.Current.Key);
				roomConnectors[i].renderer.enabled = true;
				if (en.Current.Value.hasTransitionedBetweenRooms) {
//					Debug.Log ("case 0 " + en.Current.Key + " " + en.Current.Value + " " + currentRoomPos);
					roomConnectors[i].renderer.material = play.game.miniMapRoomConnectorMaterials[MATERIAL_MINI_MAP_ROOM_ENTERED];
				} else {
					if ( 	currentRoomPos == IntTriple.MINUSONE && en.Current.Key == IntTriple.BACKWARD //entry door
						|| 	currentRoomPos == IntTriple.MINUSONE && en.Current.Key == play.cave.secretCaveRoomPos-r.pos // secret chamber door
						) {
//						Debug.Log ("case 1 " + en.Current.Key + " " + en.Current.Value + " " + currentRoomPos);
						roomConnectors[i].renderer.material = play.game.miniMapRoomConnectorMaterials[MATERIAL_MINI_MAP_ROOM_ENTERED];
						en.Current.Value.hasTransitionedBetweenRooms = true;
					} else if (currentRoomPos != IntTriple.MINUSONE && en.Current.Key == currentRoomPos-r.pos) {
//						Debug.Log ("case 2 " + en.Current.Key + " " + en.Current.Value + " " + currentRoomPos);
						roomConnectors[i].renderer.material = play.game.miniMapRoomConnectorMaterials[MATERIAL_MINI_MAP_ROOM_ENTERED];
						en.Current.Value.hasTransitionedBetweenRooms = true;
						// set opposite cell also to true
						play.cave.zone.rooms[currentRoomPos.x, currentRoomPos.y, currentRoomPos.z].exits[r.pos-currentRoomPos].hasTransitionedBetweenRooms = true;
					} else {
//						Debug.Log ("case 3 " + en.Current.Key + " " + en.Current.Value + " " + currentRoomPos);
						roomConnectors[i].renderer.material = play.game.miniMapRoomConnectorMaterials[MATERIAL_MINI_MAP_ROOM_NOT_ENTERED];
					}
				}
				en.MoveNext();
			} else {
				roomConnectors[i].renderer.enabled = false;
			}
		}
		int j=0;
		foreach (Breadcrumb b in play.breadcrumbs) {
			if (b.roomID == r.id) {
				breadcrumbs[j].renderer.enabled = true;
			} else {
				breadcrumbs[j].renderer.enabled = false;
			}
			j++;
		}
	}
	
	public void SetBreadcrumb(Vector3 pos) {
		GameObject b = play.game.CreateFromPrefab().CreateMiniMapBreadcrumb((pos/RoomMesh.MESH_SCALE) * MINI_MAP_SCALE, Quaternion.identity);
		b.transform.parent = transform;
		b.transform.localScale *= MINI_MAP_SCALE;
		breadcrumbs.Add(b);
	}
	
	public void RemoveBreadcrumb() {
		Destroy(breadcrumbs[0]);
		breadcrumbs.RemoveAt(0);
	}
	
	public void DestroyAllBreadcrumbs() {
		foreach (GameObject gO in breadcrumbs) {
			Destroy(gO);
		}
		breadcrumbs.Clear();
	}

	public void DispatchGameInput() {
		if (mode == Mode.On) {
			if (gameInput.isMobile) {
			} else {
				if (Input.GetKey(KeyCode.LeftAlt)) {
					Zoom();
					Rotate();
					if (follow == Follow.On) {
						play.SwitchMiniMapFollow();
					}
				}				
			}
		}
	}
	
}
