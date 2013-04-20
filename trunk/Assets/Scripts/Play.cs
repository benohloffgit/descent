using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Play : MonoBehaviour {	
	
	public int zoneID;
	
	public Game game;
	public Cave cave;
	public Movement movement;
	public Ship ship;
	public bool isShipInvincible;
	public bool isShipInPlayableArea;
	public bool isMiniMapOn;
	public bool isMiniMapFollowOn;
	private MiniMap miniMap;
	public PlayGUI playGUI;
	private string keyCommand;
	public bool isInKeyboardMode;
	private int caveSeed;
	private int botSeed;
	
	public GridPosition placeShipBeforeExitDoor;
	
//	private GameInput gI;
	private State state;
	private EnemyDistributor enemyDistributor;
	private CollecteablesDistributor collecteablesDistributor;
	private RaycastHit hit;
	private AStarThreadState aStarThreadState = new AStarThreadState();
//	private int currentGravitiyDirection;
//	private float lastGravitiyChange;
	
	private List<GameObject> breadcrumbs = new List<GameObject>();
	
	private List<ShotStats> shipShotStats = new List<ShotStats>();
	private List<ShotStats> enemyShotStats = new List<ShotStats>();
	private float shipHitRatio;
	private float enemyHitRatio;
	private float shipToEnemyHitRatio;
	private float activeEnemiesHealthToShipFPSRatio;
	private float shipHealthToActiveEnemiesFPSRatio;
	private float enemiesToShipFPSRatio;
	
	private GridPosition testPathStart;
	private GridPosition testPathEnd;
	
	// cached values
//	private Vector3 shipPosition;
	private GridPosition shipGridPosition;

	//private static float MAX_RAYCAST_DISTANCE = 100.0f;
	private static float GRAVITY_INTERVAL = 10.0f;
	private static float STATS_INTERVAL = 10.0f;
	private static float STATS_MIN = 0.01f;

	private static Vector3 BREADCRUMB_POSITION = new Vector3(0f, 0f, 2.0f);
	
	void OnGUI() {		
 		if (GUI.RepeatButton  (new Rect (60,400,50,50), "Exit")) {
			Application.Quit();
		}
		GUI.Label(new Rect (20,Screen.height-90,500,80),
				"Active-All: " + enemyDistributor.enemiesActive +"--"+ enemyDistributor.enemiesLiving +
				"\nHealth E: " + enemyDistributor.enemiesHealthActive +"--"+ enemyDistributor.enemiesHealthActiveAvg.ToString("F2") +
				"\nFPS S-E-S/E: " + ship.firepowerPerSecond.ToString("F2") +"--"+ enemyDistributor.enemiesFirepowerPerSecondActive.ToString("F2") +"--"+enemiesToShipFPSRatio.ToString("F2")+
				"\nHealth/FPS S-E: " + activeEnemiesHealthToShipFPSRatio.ToString("F2") +"--"+ shipHealthToActiveEnemiesFPSRatio.ToString("F2") +
				"\nHit/Miss S-E-S/E: " + shipHitRatio.ToString("F2") +"--"+enemyHitRatio.ToString("F2") + "--" + shipToEnemyHitRatio.ToString("F2")
			);
		
		Event e = Event.current;
        if (e.isKey && e.type == EventType.KeyDown) { // keydown and characters can come as seperate events!!!
//			Debug.Log("> " + keyCommand + " "  + e.type + " " +  e.keyCode);
			if (e.keyCode == KeyCode.Backslash) {
				keyCommand = "";
				isInKeyboardMode = true;
			} else if (e.keyCode == KeyCode.Return) {
				ExecuteKeyCommand();
			} else if (isInKeyboardMode) {
				if (e.character != 0) {
//					Debug.Log ("here 2 " + e.character + " " + e.keyCode);
					keyCommand += e.character;
				}
			}
			e.Use();
		}
   	}
	
/*	void FixedUpdate() {
		if (Time.time > lastGravitiyChange + GRAVITY_INTERVAL) {
			currentGravitiyDirection++;
			if (currentGravitiyDirection == Cave.ROOM_DIRECTIONS.Length) {
				currentGravitiyDirection = 0;
			}
			//Physics.gravity = Cave.ROOM_DIRECTIONS[currentGravitiyDirection].GetVector3();
			lastGravitiyChange = Time.time;
		}
	}*/
	
	void Update() {
		// editor commands
		if (Application.platform == RuntimePlatform.WindowsEditor) {
/*			if (Input.GetKeyDown(KeyCode.Alpha1)) {				
				MineTouch mineTouch = enemyDistributor.CreateMineTouch();
				mineTouch.transform.position = GetShipPosition();
				Debug.Log ("Adding Mine Touch (Editor mode)");
			}*/
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
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, Game.MAX_VISIBILITY_DISTANCE, 1 << Game.LAYER_CAVE)) {
					WallGun wallGun = enemyDistributor.CreateWallGun();
					enemyDistributor.PlaceOnWall(wallGun.gameObject, hit);
					Debug.Log ("Adding Wall Gun (Editor mode)");
				}
			}
			if (Input.GetKeyDown(KeyCode.Alpha5)) {	
				ship.transform.position = cave.GetPositionFromGrid(placeShipBeforeExitDoor);
			}
/*			if (Input.GetKeyDown(KeyCode.Alpha5)) {	
				if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
					WallLaser wallLaser = enemyDistributor.CreateWallLaser();
					enemyDistributor.PlaceOnWall(wallLaser.gameObject, hit);
					Debug.Log ("Adding Wall Laser (Editor mode)");
				}
			}*/
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
				
/*			if (Input.GetKeyDown(KeyCode.Alpha0)) {
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
			}*/
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
			if (Input.GetKey(KeyCode.RightAlt) && Input.GetKeyDown(KeyCode.C)) {
				PlaceTestCubes();
				Debug.Log ("Placing test cubes");
			}
			if (Input.GetKey(KeyCode.RightAlt) && Input.GetKeyDown(KeyCode.V)) {
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
			if (Input.GetKey(KeyCode.RightAlt) && Input.GetKeyDown(KeyCode.K)) {
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
	
	void FixedUpdate() {
		playGUI.DispatchFixedUpdate();
	}
	
	public void Restart() {
		botSeed = UnityEngine.Random.Range(0,9999999);
//		caveSeed = 4801662;
		caveSeed = UnityEngine.Random.Range(1000000,9999999);
		
		zoneID = 5;
		isInKeyboardMode = false;
		
		playGUI = new PlayGUI(this);
		
		// game setup
		ship = (GameObject.Instantiate(game.shipPrefab) as GameObject).GetComponent<Ship>();
		ship.Initialize(this, game);
		playGUI.Initialize();
		
		GameObject newMiniMap = GameObject.Instantiate(game.miniMapPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		miniMap = newMiniMap.GetComponent<MiniMap>() as MiniMap;
		miniMap.Initialize(ship, this, game.gameInput, newMiniMap.GetComponentInChildren<Camera>());
		
		cave = new Cave(this);
		collecteablesDistributor = new CollecteablesDistributor(this);
		enemyDistributor = new EnemyDistributor(this);
		movement = new Movement(this);
		
		StartZone ();
		
		ship.transform.position = cave.GetCaveEntryPosition();		
		
//		currentGravitiyDirection = 0;
//		lastGravitiyChange = Time.time;
		
		shipHitRatio = 0;
		enemyHitRatio = 0;
		shipToEnemyHitRatio = 1.0f;
		InvokeRepeating("UpdateStats", 0, 1.0f);
	}
	
	private void StartZone() {
		isShipInPlayableArea = false;
		Debug.Log ("Cave Seed: " + caveSeed);
		UnityEngine.Random.seed = caveSeed;
		cave.AddZone(zoneID);
		UnityEngine.Random.seed = botSeed;
		enemyDistributor.Distribute(zoneID);
	}
	
	public void EndZone() {
		enemyDistributor.RemoveAll();
		cave.RemoveZone();
		playGUI.Reset();
		zoneID++;
		botSeed = UnityEngine.Random.Range(0,9999999);
		UnityEngine.Random.seed = caveSeed;
		caveSeed = UnityEngine.Random.Range(1000000,9999999);
		StartZone();
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
	
	private void ExecuteKeyCommand() {
		Debug.Log ("Key command is: " + keyCommand);
		if (keyCommand.Substring(1, 1) == "e") {
				Enemy e = enemyDistributor.CreateEnemy(null, Enemy.CLAZZ_NUM(keyCommand.Substring(2, 1)), Convert.ToInt32(keyCommand.Substring(3, 2)));
				e.transform.position = GetShipPosition();
				Debug.Log ("Adding Enemy " + keyCommand.Substring(2, 1) + Convert.ToInt32(keyCommand.Substring(3, 2)) + " (Editor mode)");
		} else if (keyCommand.Substring(1, 1) == "m") {
				Mana m = enemyDistributor.CreateMana();
				m.transform.position = GetShipPosition();
				Debug.Log ("Adding Mana (Editor mode)");
		} else if (keyCommand.Substring(1, 1) == "d" && keyCommand.Substring(2, 6) == "health") {
				collecteablesDistributor.DropHealth(GetShipPosition() + ship.transform.forward * RoomMesh.MESH_SCALE);
				Debug.Log ("Adding Health (Editor mode)");
		} else if (keyCommand.Substring(1, 1) == "d" && keyCommand.Substring(2, 6) == "shield") {
				collecteablesDistributor.DropShield(GetShipPosition() + ship.transform.forward * RoomMesh.MESH_SCALE);
				Debug.Log ("Adding Shield (Editor mode)");
		} else if (keyCommand.Substring(1, 1) == "d" && keyCommand.Substring(2, 6) == "scroll") {
				collecteablesDistributor.DropScroll(GetShipPosition() + ship.transform.forward * RoomMesh.MESH_SCALE);
				Debug.Log ("Adding Scroll (Editor mode)");
		} else if (keyCommand.Substring(1, 1) == "s") {
				int clazz = Enemy.CLAZZ_NUM(keyCommand.Substring(2, 1));
				int model = Convert.ToInt32(keyCommand.Substring(3, 2));
				Spawn s = enemyDistributor.CreateSpawn(clazz, model, model + EnemyDistributor.CLAZZ_A_EQUIVALENT_MODEL[clazz], GetShipGridPosition());
				Debug.Log ("Adding Spawn  " + keyCommand.Substring(2, 1) + Convert.ToInt32(keyCommand.Substring(3, 2)) + " (Editor mode)");
		}
				
		isInKeyboardMode = false;
	}
	
	public void PlaceTestCube(GridPosition pos) {
		GameObject o = Instantiate(game.testCubePrefab, cave.GetPositionFromGrid(pos), Quaternion.identity) as GameObject;
		if (pos.cellPosition == IntTriple.ZERO) {
			Debug.Log (pos + " " + o.transform.position);
		}
	}
	
	public void CachePositionalDataOfShip(Vector3 pos) {
		shipGridPosition = cave.GetGridFromPosition(pos);
		if (shipGridPosition.cellPosition.z < 0 || shipGridPosition.roomPosition.z > Game.DIMENSION_ZONE-1) {
			isShipInPlayableArea = false;
		} else {
			isShipInPlayableArea = true;
		}
//		Debug.Log (shipGridPosition);
//		shipGridPosition = cave.GetClosestEmptyGridFromPosition(pos);
	}
	
	public Vector3 GetShipPosition() {
		return ship.transform.position;
	}

	public GridPosition GetShipGridPosition() {
		return shipGridPosition;
	}
	
	public Room GetRoomOfShip() {
		return cave.zone.GetRoom(shipGridPosition);
	}	
	
	public void SwitchMiniMap() {
		if (isMiniMapOn) {
			isMiniMapOn = false;
			miniMap.SwitchOff();
		} else if (isShipInPlayableArea) {
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
	
	private void UpdateStats() {
		shipHitRatio = Mathf.Max(GetShotStats(ref shipShotStats, shipHitRatio), STATS_MIN);
		enemyHitRatio = Mathf.Max(GetShotStats(ref enemyShotStats, enemyHitRatio), STATS_MIN);
		shipToEnemyHitRatio = shipHitRatio / enemyHitRatio;
		activeEnemiesHealthToShipFPSRatio = enemyDistributor.enemiesHealthActive/ship.firepowerPerSecond;
		shipHealthToActiveEnemiesFPSRatio = ship.health/enemyDistributor.enemiesFirepowerPerSecondActive;
		enemiesToShipFPSRatio = enemyDistributor.enemiesFirepowerPerSecondActive/ship.firepowerPerSecond;
	}

	private float GetShotStats(ref List<ShotStats> shotStats, float startValue) {
		float result = startValue;
		int hits = 0;
		foreach (ShotStats s in shotStats) {
			hits += s.val;
		}
		result = (float)hits / shotStats.Count;
		if (shotStats.Count > 10) {
			shotStats.RemoveRange(0, shotStats.Count-10);
		}
		return result;
	}

	/*
	 	private float GetShotStats(ref List<ShotStats> shotStats, float startValue) {
		float result = startValue;
		if (shotStats.Count > 0) {
			int last = 0;
			foreach (ShotStats s in shotStats) {
				if (Time.time > s.time + STATS_INTERVAL) {
					last++;
				} else {
					break;
				}
			}
			shotStats.RemoveRange(0, last);
			if (shotStats.Count > 0) {
				int hits = 0;
				foreach (ShotStats s in shotStats) {
					hits += s.val;
				}
				result = (float)hits / shotStats.Count;
			}
		}
		return result;
	} 
	 */
	 
	public void DamageShip(int source) {
		if (source == Game.ENEMY) {
			enemyShotStats.Add(new ShotStats(ShotStats.HIT, Time.time));
		} else {
			shipShotStats.Add(new ShotStats(ShotStats.MISS, Time.time));
		}
	}
	
	public void DamageEnemy(int source) {
		if (source == Game.SHIP) {
			shipShotStats.Add(new ShotStats(ShotStats.HIT, Time.time));
		} else {
			enemyShotStats.Add(new ShotStats(ShotStats.MISS, Time.time));
		}
	}
	
	public void DamageNothing(int source) {
		if (source == Game.SHIP) {
			shipShotStats.Add(new ShotStats(ShotStats.MISS, Time.time));
		} else {
			enemyShotStats.Add(new ShotStats(ShotStats.MISS, Time.time));
		}
	}
	
	public void DisplayExplosion(Vector3 pos, Quaternion rot) {
		game.CreateFromPrefab().CreateExplosion(pos, rot);
	}

	public void DisplayHit(Vector3 pos, Quaternion rot) {
		game.CreateFromPrefab().CreateHit(pos, rot);
	}
	
	public void RemoveEnemy(Enemy e) {
		collecteablesDistributor.DistributeOnEnemyDeath(e);
		playGUI.RemoveEnemy(e);
		ship.RemoveEnemy(e);
	}
	
	public void HealShip(int amount) {
		ship.Heal(amount);
	}

	public void ShieldShip(int amount) {
		ship.Shield(amount);
	}
	
	public void AddMissile(int type, int amount) {
		// order is highest first
		ship.secondaryWeapons[type-1].ammunition += amount;
		playGUI.DisplaySecondaryWeapon(ship.secondaryWeapons[ship.currentSecondaryWeapon]);
	}
	
	public void CreateBreadcrumb() {
		breadcrumbs.Add(game.CreateFromPrefab().CreateBreadcrumb(ship.transform.position + ship.transform.TransformDirection(BREADCRUMB_POSITION), Quaternion.identity));
		if (breadcrumbs.Count > Game.MAX_BREADCRUMBS) {
			Destroy(breadcrumbs[0]);
			breadcrumbs.RemoveAt(0);
		}
	}

	public void ScrollFound() {
		Debug.Log ("scroll found");
	}
}

