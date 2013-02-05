using System;
using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour {
	
	private GameInput gameInput;
	private Ship ship;
	private Play play;
	private Transform map;
	
	private Vector3 setoffMap; // each mesh's vertices are set off depending on the position of its room in the zone
	private Vector3 setoffRoom;
	
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
//	private static float TOUCH_THRESHOLD = Screen.dpi * 0.2f;
//	private static float TOUCH_SENSITIVITY = Screen.dpi * 0.5f;
//	private static float TOUCH_TIME_THRESHOLD = 0.3f;

	public enum Mode { Off=0, On=1 }
		
	public void Initialize(Ship s, Play p, GameInput gI) {
		ship = s;
		gameInput = gI;
		play = p;
		
		moveBitwise = 0;
		primaryTouchFinger = 0;
		secondaryTouchFinger = 1;
		usesMouse = true;
		mode = Mode.Off;
	}
	
	void FixedUpdate() {
		if (mode == Mode.On) {
			UpdatePosition();
		}
	}
	
	public void SwitchOn() {
		Debug.Log ("Switching mini map on");
		Room r = play.GetRoomOfShip();
		GameObject mO = ship.game.CreateFromPrefab().CreateMiniMap(Vector3.zero, Quaternion.identity);
		map = mO.transform;
		map.localScale *= MINI_MAP_SCALE;
		UpdatePosition();
		Mesh m = new Mesh();
		m.vertices = r.roomMesh.mesh.vertices;
		m.triangles = r.roomMesh.mesh.triangles;
		m.uv = r.roomMesh.mesh.uv;
		map.GetComponent<MeshFilter>().mesh = m;
		mode = Mode.On;
	}
	
	public void SwitchOff() {
		Debug.Log ("Switching mini map off");
		mode = Mode.Off;
	}
	
	private void UpdatePosition() {
		Room r = play.GetRoomOfShip();
		Vector3 setoff = r.pos.GetVector3() * Game.DIMENSION_ROOM;
		Vector3 setoffShipUnscaled = (ship.transform.position - (r.roomMesh.transform.position - setoff) ) / RoomMesh.MESH_SCALE;
		Debug.Log ("setoffShipUnscaled:"+setoffShipUnscaled);
//		map.localPosition = Vector3.zero - setoffShipUnscaled * MINI_MAP_SCALE;
	}
	
	public void DispatchGameInput() {
		if (gameInput.isMobile) {
		}
	}
	
}
