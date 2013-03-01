using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Play : MonoBehaviour {	
	public GameObject roomPrefab;
	public GameObject roomMeshPrefab;
	public GameObject testCubePrefab;
	public GameObject shipPrefab;
	public GameObject wallGunPrefab;
	public GameObject wallLaserPrefab;
	public GameObject mineTouchPrefab;
	public GameObject mineBuilderPrefab;
	public GameObject lightBulbPrefab;
	public GameObject pyramidPrefab;
	public GameObject spikePrefab;
	public GameObject bullPrefab;
	public GameObject roomEntryPrefab;
	public GameObject roomConnectorPrefab;
	public GameObject miniMapPrefab;
	
	public Game game;
	public Cave cave;
	public Movement movement;
	public Ship ship;
	public bool isShipInvincible;
	public bool isMiniMapOn;
	public bool isMiniMapFollowOn;
	private MiniMap miniMap;
	private PlayGUI playGUI;
	
//	private GameInput gI;
	private State state;
	private EnemyDistributor enemyDistributor;
	private RaycastHit hit;
	private AStarThreadState aStarThreadState = new AStarThreadState();

	private GridPosition testPathStart;
	private GridPosition testPathEnd;
	
	// cached values
//	private Vector3 shipPosition;
	private GridPosition shipGridPosition;

	private static float MAX_RAYCAST_DISTANCE = 100.0f;
			
	void Update() {
		// editor commands
		if (Application.platform == RuntimePlatform.WindowsEditor) {
			if (Input.GetKeyDown(KeyCode.Alpha1)) {				
				MineTouch mineTouch = enemyDistributor.CreateMineTouch();
				mineTouch.transform.position = GetShipPosition();
				Debug.Log ("Adding Mine Touch (Editor mode)");
			}
			if (Input.GetKeyDown(KeyCode.Alpha2)) {				
				MineBuilder mineBuilder = enemyDistributor.CreateMineBuilder();
				mineBuilder.transform.position = GetShipPosition();
				Debug.Log ("Adding Mine Builder (Editor mode)");
			}
			if (Input.GetKeyDown(KeyCode.Alpha3)) {				
				LightBulb lightBulb = enemyDistributor.CreateLightBulb();
				lightBulb.transform.position = GetShipPosition();
				Debug.Log ("Adding Light Bulb (Editor mode)");
			}
			if (Input.GetKeyDown(KeyCode.Alpha4)) {				
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					WallGun wallGun = enemyDistributor.CreateWallGun();
					enemyDistributor.PlaceOnWall(wallGun.gameObject, hit);
					Debug.Log ("Adding Wall Gun (Editor mode)");
				}
			}
			if (Input.GetKeyDown(KeyCode.Alpha5)) {				
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					WallLaser wallLaser = enemyDistributor.CreateWallLaser();
					enemyDistributor.PlaceOnWall(wallLaser.gameObject, hit);
					Debug.Log ("Adding Wall Laser (Editor mode)");
				}
			}
			if (Input.GetKeyDown(KeyCode.Alpha6)) {				
				Pyramid pyramid = enemyDistributor.CreatePyramid();
				pyramid.transform.position = GetShipPosition();
				Debug.Log ("Adding Pyramid (Editor mode)");
			}
			if (Input.GetKeyDown(KeyCode.Alpha7)) {				
				Spike spike = enemyDistributor.CreateSpike();
				spike.transform.position = GetShipPosition(); //new Vector3(130f, 205f, 67f)
				Debug.Log ("Adding Spike (Editor mode)");
			}
			if (Input.GetKeyDown(KeyCode.Alpha8)) {				
				Bull bull = enemyDistributor.CreateBull();
				bull.transform.position = GetShipPosition(); //new Vector3(130f, 205f, 67f)
				Debug.Log ("Adding Bull (Editor mode)");
			}
			if (Input.GetKeyDown(KeyCode.Alpha0)) {
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					int triangleIndex = hit.triangleIndex * 3;
					Debug.Log ("Triangle Index: " + triangleIndex);
					Mesh m = GetRoomOfShip().roomMesh.mesh;
					int[] vertexIndices = new int[3];
					vertexIndices[0] = m.triangles[triangleIndex];
					vertexIndices[1] = m.triangles[triangleIndex+1];
					vertexIndices[2] = m.triangles[triangleIndex+2];
					Vector3[] normals = new Vector3[3];
					normals[0] = m.normals[vertexIndices[0]];
					normals[1] = m.normals[vertexIndices[1]];
					normals[2] = m.normals[vertexIndices[2]];
					Debug.Log ("Vertex index is " + vertexIndices[0] +","+vertexIndices[1]+","+vertexIndices[2]);
					Debug.Log ("Vertex is " + m.vertices[vertexIndices[0]] +","+m.vertices[vertexIndices[1]]+","+m.vertices[vertexIndices[2]]);
					Debug.Log ("Triangle (Editor mode): " + hit.normal + " (" + normals[0]+","+normals[1]+","+normals[2]+ ")");
				}
			}
/*			if (Input.GetKeyDown(KeyCode.H)) {				
				ship.health -= 10;
				playGUI.SetHealth(ship.health);
			}*/
	/*		if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.LeftArrow)) {
				Debug.Log ("Changing Vertex");
				Mesh m = GetRoomOfShip().roomMesh.mesh;
				Vector3[] vertices = m.vertices;
				int i = 462; // 462,403,469
				vertices[i].z += 0.1f;
				m.vertices = vertices;
				//m.RecalculateNormals();
			}*/
			
			if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.I)) {
				isShipInvincible = (isShipInvincible) ? false : true;
				Debug.Log ("Setting ship invincible: " + isShipInvincible);
			}
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.C)) {
				PlaceTestCubes();
				Debug.Log ("Placing test cubes");
			}
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.V)) {
				RemoveTestCubes();
				Debug.Log ("Removing test cubes");
			}
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.I)) {
				testPathStart = new GridPosition(new IntTriple(7,2,2), new IntTriple(1,2,0));
				testPathEnd = new GridPosition(new IntTriple(9,2,10), new IntTriple(1,2,0));
				movement.AStarPath(aStarThreadState, testPathStart, testPathEnd);
				Debug.Log ("Setting AStar path from/to : " + testPathStart + " / "  + testPathEnd);
			}
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.O)) {
				testPathStart = cave.GetGridFromPosition(ship.transform.position);
				Debug.Log ("Setting AStar path start at : " + testPathStart);
			}
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P)) {
				testPathEnd = cave.GetGridFromPosition(ship.transform.position);
				Debug.Log ("Setting AStar path end at : " + testPathEnd);
				movement.AStarPath(aStarThreadState, testPathStart, testPathEnd);
//				Debug.Log (Time.frameCount);
			}
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.K)) {
				Debug.Log("Ship Grid: " + shipGridPosition + ", " + cave.GetCellDensity(shipGridPosition));
			}
/*			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.L)) {
				Debug.Log("Nearest empty grid: " + cave.GetNearestEmptyGridPositionFrom(shipGridPosition));
			}*/
			
			if (aStarThreadState.IsFinishedNow()) {
				aStarThreadState.Complete();
//				Debug.Log (Time.frameCount);
				foreach (AStarNode n in aStarThreadState.roomPath) {
					PlaceTestCube(n.gridPos);
				}
			}
		}
		
		playGUI.DispatchUpdate();
	}
	
	public void Restart() {
		playGUI = new PlayGUI(this);
		
		// game setup
		ship = (GameObject.Instantiate(shipPrefab) as GameObject).GetComponent<Ship>();
		ship.Initialize(this, game);
		playGUI.Initialize();
//		playGUI.DisplayHealth(new int[] { MyGUI.GetDigitOfNumber(0, ship.health), MyGUI.GetDigitOfNumber(1, ship.health), MyGUI.GetDigitOfNumber(2, ship.health)});
//		playGUI.DisplayShield(new int[] { MyGUI.GetDigitOfNumber(0, ship.shield), MyGUI.GetDigitOfNumber(1, ship.shield), MyGUI.GetDigitOfNumber(2, ship.shield)});
//		playGUI.currentHealth = ship.health;
		
		GameObject newMiniMap = GameObject.Instantiate(miniMapPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		miniMap = newMiniMap.GetComponent<MiniMap>() as MiniMap;
		miniMap.Initialize(ship, this, game.gameInput, newMiniMap.GetComponentInChildren<Camera>());
		
		int seed = 1922614; //123456789;
//		int seed = UnityEngine.Random.Range(1000000,9999999);
		Debug.Log ("Seed: " + seed);
		UnityEngine.Random.seed = seed;
		cave = new Cave(this);
		PlaceShip();
		movement = new Movement(this);
//		PlaceTestCubes();
		PlaceEnemies();
	}
	
	public void Initialize(Game g, GameInput input) {
		game = g;
		state = game.state;
		isShipInvincible = false;
		isMiniMapOn = false;
		isMiniMapFollowOn = false;
	}
		
	void onDisable() {
		CancelInvoke();
		Destroy(ship.gameObject);
//		Destroy(cave.gameObject);
	}
	
	public void DispatchGameInput() {
		ship.DispatchGameInput();
		miniMap.DispatchGameInput();
	}
	
	private void PlaceShip() {
		ship.transform.position = cave.GetCaveEntryPosition();
	}
	
	private void PlaceEnemies() {
		enemyDistributor = new EnemyDistributor(game, this);
//		enemyDistributor.Distribute();
	}
	
	private void PlaceTestCubes() {
		Room room = GetRoomOfShip();
		for (var i=0; i<Game.DIMENSION_ROOM; i++) {
			for (var j=0; j<Game.DIMENSION_ROOM; j++) {
				for (var k=0; k<Game.DIMENSION_ROOM; k++) {
					IntTriple cellPos = new IntTriple(i,j,k);
					if (room.GetCellDensity(cellPos) == Cave.DENSITY_EMPTY) {
						PlaceTestCube(new GridPosition(cellPos, room.pos));
					}
				}
			}
		}
	}
	
	private void RemoveTestCubes() {
		GameObject[] gOs = GameObject.FindGameObjectsWithTag("TestCube");
		for (int i=0; i<gOs.Length; i++) {
			Destroy(gOs[i]);
		}
	}
	
	public void PlaceTestCube(GridPosition pos) {
		Instantiate(testCubePrefab, cave.GetPositionFromGrid(pos), Quaternion.identity);
	}
	
	public void CachePositionalDataOfShip(Vector3 pos) {
		shipGridPosition = cave.GetGridFromPosition(pos);
//		shipGridPosition = cave.GetClosestEmptyGridFromPosition(pos);
	}
	
	public Vector3 GetShipPosition() {
		return ship.transform.position;
	}

	public GridPosition GetShipGridPosition() {
		return shipGridPosition;
	}
	
	public Room GetRoomOfShip() {
		return cave.GetCurrentZone().GetRoom(shipGridPosition);
	}	
	
	public void SwitchMiniMap() {
		if (isMiniMapOn) {
			isMiniMapOn = false;
			miniMap.SwitchOff();
		} else {
			isMiniMapOn = true;
			miniMap.SwitchOn();
		}
	}
	
	public void SwitchMiniMapFollow() {
		if (isMiniMapFollowOn) {
			isMiniMapFollowOn = false;
			miniMap.SwitchFollowOff();
		} else {
			isMiniMapFollowOn = true;
			miniMap.SwitchFollowOn();
		}
	}
	
	public void DamageShip(int damage) {
		if (ship.shield > 0) {
			ship.shield -= damage * 2;
			if (ship.shield < 0) {
				damage = Mathf.Abs(ship.shield);
				ship.shield = 0;
			} else {
				damage = 0;
			}
		}
		ship.health -= damage;
//		playGUI.SetHealth(ship.health);
//		playGUI.SetShield(ship.shield);
	}
	
	public void DamageEnemy(int damage, Enemy e) {
		e.Damage(damage);
	}
}

