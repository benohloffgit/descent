using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Play : MonoBehaviour {	
	public GameObject guiPrefab;
	public GameObject roomPrefab;
	public GameObject testCubePrefab;
	public GameObject shipPrefab;
	public GameObject wallGunPrefab;
	public GameObject mineTouchPrefab;
	public GameObject mineBuilderPrefab;

	public Room room;
	public Movement movement;
	public Ship ship;
	public bool isShipInvincible;
	
	public static int ROOM_SIZE = 16;

	private Game game;
//	private GameInput gI;
	private State state;
	private MyGUI gui;
	private EnemyDistributor enemyDistributor;
	private RaycastHit hit;

	private int container;
	private int dialogContainer;
	private EnemyDistributor.IntTriple testPathStart;
	private EnemyDistributor.IntTriple testPathEnd;
		

	private static float MAX_RAYCAST_DISTANCE = 100.0f;
	
	void Awake() {
		isShipInvincible = false;
	}
	
	void Update() {
		// editor commands
		if (Application.platform == RuntimePlatform.WindowsEditor) {
			if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.G)) {				
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					WallGun wallGun = enemyDistributor.CreateWallGun();
					enemyDistributor.PlaceOnWall(wallGun.gameObject, hit);
					Debug.Log ("Adding Gun (Editor mode)");
				}
			}
			if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.M)) {				
				Vector3 cubePositionOfShip = Room.GetCubePosition(ship.transform.position);
				MineTouch mineTouch = enemyDistributor.CreateMineTouch();
				mineTouch.transform.position = cubePositionOfShip * Room.MESH_SCALE;
				Debug.Log ("Adding Mine Touch (Editor mode)");
			}
			if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.U)) {				
				Vector3 cubePositionOfShip = Room.GetCubePosition(ship.transform.position);
				MineBuilder mineBuilder = enemyDistributor.CreateMineBuilder();
				mineBuilder.transform.position = cubePositionOfShip * Room.MESH_SCALE;
				Debug.Log ("Adding Mine Builder (Editor mode)");
			}
			if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.I)) {
				isShipInvincible = (isShipInvincible) ? false : true;
				Debug.Log ("Setting ship invincible: " + isShipInvincible);
			}
			if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.O)) {
				testPathStart = new EnemyDistributor.IntTriple(Room.GetCubePosition(ship.transform.position));
				Debug.Log ("Setting AStar path start at : " + Room.GetCubePosition(ship.transform.position));
			}
			if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.P)) {
				Debug.Log ("Setting AStar path end at : " + Room.GetCubePosition(ship.transform.position));
				testPathEnd = new EnemyDistributor.IntTriple(Room.GetCubePosition(ship.transform.position));
				LinkedList<AStarNode> path = movement.AStarPath(testPathStart, testPathEnd);
				foreach (AStarNode n in path) {
					PlaceTestCube(n.position);
				}
			}
			if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.L)) {
				Debug.Log(Room.GetCubePosition(ship.transform.position));
			}
			if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.I)) {
				testPathStart = new EnemyDistributor.IntTriple(new Vector3(5f, 6f, 1f));
				testPathEnd = new EnemyDistributor.IntTriple(new Vector3(4f, 7f, 1f));
				Debug.Log ("Setting AStar path from/to at : " + testPathStart.GetVector3() + "/" + testPathEnd.GetVector3());
				LinkedList<AStarNode> path = movement.AStarPath(testPathStart, testPathEnd);
				foreach (AStarNode n in path) {
					PlaceTestCube(n.position);
				}
			}
		}
	}
	
	public void Restart() {
		room = (GameObject.Instantiate(roomPrefab) as GameObject).GetComponent<Room>();
		ship = (GameObject.Instantiate(shipPrefab) as GameObject).GetComponent<Ship>();
		ship.Initialize(this, game);
		room.Initialize(ship.transform, ROOM_SIZE);
		movement = new Movement(this);
//		PlaceTestCubes();
		PlaceEnemies();
	}
	
	public void Initialize(Game g, GameInput input) {
		game = g;
		state = game.state;
	}
		
	private void CloseDialog() {
		Destroy(gui.containers[dialogContainer].gameObject);
		gui.ResetGameInputZLevel();
		gui.DeleteGUIInFocus();
	}
	
	void onDisable() {
		CancelInvoke();
		Destroy(ship.gameObject);
		Destroy(room.gameObject);
	}
	
	public void DispatchGameInput() {
		ship.DispatchGameInput();
	}
	
	private void PlaceEnemies() {
		enemyDistributor = new EnemyDistributor(game, this, room, ROOM_SIZE);
		enemyDistributor.Distribute();
	}
	
	private void PlaceTestCubes() {
		for (var i=0; i<ROOM_SIZE; i++) {
			for (var j=0; j<ROOM_SIZE; j++) {
				for (var k=0; k<ROOM_SIZE; k++) {
					if (room.cubeDensity[i,j,k] == CaveDigger.DENSITY_EMPTY) {
						Instantiate(testCubePrefab, Room.GetPositionFromCube(new Vector3(i,j,k)), Quaternion.identity);
					}
				}
			}
		}
	}
	
	public void PlaceTestCube(Vector3 position) {
		Instantiate(testCubePrefab, Room.GetPositionFromCube(position), Quaternion.identity);
	}
	
	public Vector3 GetShipPosition() {
		// TODO cache
		return ship.transform.position;
	}
}

