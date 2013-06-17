using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Play : MonoBehaviour {	
	
	public int zoneID;
	
	public Mode mode;
	public Game game;
	public Cave cave;
	public Movement movement;
	public Ship ship;
	private ExitHelper exitHelper;
	public bool isShipInvincible;
	public bool isShipInPlayableArea;
	public bool isMiniMapOn;
	public bool isMiniMapFollowOn;
	public bool isPaused;
	private MiniMap miniMap;
	public PlayGUI playGUI;
	public Sokoban sokoban;
	private string keyCommand;
	public bool isInKeyboardMode;
	private int caveSeed;
	private int botSeed;
	private Transform lightsHolder;
	private bool[] isKeyCollected;
	
	public GridPosition placeShipBeforeExitDoor;
	public GridPosition placeShipBeforeSecretChamberDoor;
	
//	private GameInput gI;
	private State state;
	private EnemyDistributor enemyDistributor;
	private CollecteablesDistributor collecteablesDistributor;
	private RaycastHit hit;
	private AStarThreadState aStarThreadState = new AStarThreadState();
//	private int currentGravitiyDirection;
//	private float lastGravitiyChange;
	
	private List<GameObject> breadcrumbs = new List<GameObject>();
	
	private Light[] caveLights;
	
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
//	private static float GRAVITY_INTERVAL = 10.0f;
//	private static float STATS_INTERVAL = 10.0f;
	private static float STATS_MIN = 0.01f;

	private static Vector3 BREADCRUMB_POSITION = new Vector3(0f, 0f, 2.0f);

	public enum Mode { Normal=0, Sokoban=1 }

	void Awake() {
		lightsHolder = GameObject.Find("Lights").transform;
		caveLights = lightsHolder.GetComponentsInChildren<Light>();
	}

	public void Initialize(Game g) {
		game = g;
		state = game.state;
		isShipInvincible = false;
		isMiniMapOn = false;
		isMiniMapFollowOn = false;
		mode = Mode.Normal;
		playGUI = new PlayGUI(this);

		sokoban = new Sokoban(this);
		
		botSeed = UnityEngine.Random.Range(0,9999999);
//		caveSeed = 2122215; 9164008 4995052;//
		caveSeed = UnityEngine.Random.Range(1000000,9999999);
		
//		zoneID = state.level;
		isInKeyboardMode = false;
		
		exitHelper = (GameObject.Instantiate(game.exitHelperPrefab) as GameObject).GetComponent<ExitHelper>();
		
		// game setup
		ship = (GameObject.Instantiate(game.shipPrefab) as GameObject).GetComponent<Ship>();
		ship.Initialize(this, exitHelper);
		ship.Deactivate();
		
		playGUI.Initialize();
		
		GameObject newMiniMap = GameObject.Instantiate(game.miniMapPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		miniMap = newMiniMap.GetComponent<MiniMap>() as MiniMap;
		miniMap.Initialize(ship, this, game.gameInput, newMiniMap.GetComponentInChildren<Camera>());
		
		cave = new Cave(this);
		collecteablesDistributor = new CollecteablesDistributor(this);
		enemyDistributor = new EnemyDistributor(this);
		movement = new Movement(this);
		
		exitHelper.Initialize(this);

		SetPaused(true);
	}
	
	public void Activate() {
		StartZone();
		ship.CalculateHullClazz();
		ship.AddSpecials();
		ship.AddWeapons();
		playGUI.Activate();
		InvokeRepeating("UpdateStats", 0, 1.0f);
	}
	
	public void Deactivate() {
		playGUI.Deactivate();
		ship.RemoveWeapons();
		CancelInvoke();
	}

	void OnGUI() {
		if (!isPaused) {
/*			GUI.Label(new Rect (20,Screen.height-90,500,80),
					"Active-Living-All: " + enemyDistributor.enemiesActive +"--"+ enemyDistributor.enemiesLiving +"--"+ enemyDistributor.enemiesAll +
					"\nHealth E: " + enemyDistributor.enemiesHealthActive +"--"+ enemyDistributor.enemiesHealthActiveAvg.ToString("F2") +
					"\nFPS S-E-S/E: " + ship.firepowerPerSecond.ToString("F2") +"--"+ enemyDistributor.enemiesFirepowerPerSecondActive.ToString("F2") +"--"+enemiesToShipFPSRatio.ToString("F2")+
					"\nHealth/FPS S-E: " + activeEnemiesHealthToShipFPSRatio.ToString("F2") +"--"+ shipHealthToActiveEnemiesFPSRatio.ToString("F2") +
					"\nHit/Miss S-E-S/E: " + shipHitRatio.ToString("F2") +"--"+enemyHitRatio.ToString("F2") + "--" + shipToEnemyHitRatio.ToString("F2")
				);*/
			
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
   	}
	
	void Update() {
		if (mode == Mode.Sokoban && sokoban.isAnimatingMove) {
			sokoban.DispatchUpdate();
		}
		// editor commands
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
			/*if (Input.GetKeyDown(KeyCode.F6)) {
				SwitchMode();
			}*/
			if (Input.GetKeyDown(KeyCode.Alpha5)) {	
				KeyFound(CollecteableKey.TYPE_SILVER);
				KeyFound(CollecteableKey.TYPE_GOLD);
				ship.transform.position = cave.GetPositionFromGrid(placeShipBeforeExitDoor);
			}
			if (Input.GetKeyDown(KeyCode.Alpha6)) {	
				ship.transform.position = cave.GetPositionFromGrid(placeShipBeforeSecretChamberDoor);
			}
			if (Input.GetKeyDown(KeyCode.Alpha7)) {	
				ship.transform.position = cave.GetPositionFromGrid(placeShipBeforeSecretChamberDoor);
				SokobanSolved();
			}
			if (Input.GetKeyDown(KeyCode.Alpha8)) {	
				ship.isInvincibleOn = ship.isInvincibleOn ? false : true;
			}
		}
	}
	
	void FixedUpdate() {
		if (!isPaused) {
			playGUI.DispatchFixedUpdate();
		}
	}
	
	private void StartZone() {
		zoneID = state.level;
		shipHitRatio = 0;
		enemyHitRatio = 0;
		shipToEnemyHitRatio = 1.0f;
		isShipInPlayableArea = false;
		Debug.Log (" ------------------------------- Cave Seed: " + caveSeed);
		UnityEngine.Random.seed = caveSeed;
		cave.AddZone(zoneID);
		UnityEngine.Random.seed = botSeed;
		isKeyCollected = new bool[] {false, false};
//		collecteablesDistributor.DropKeys();
//		collecteablesDistributor.DropPowerUps();
		if (zoneID > 0) {
			enemyDistributor.Distribute();
		}
		ship.Activate();
		sokoban.RenderLevel(zoneID);
		ConfigureLighting();
		playGUI.Reset();
		ship.transform.position = cave.GetCaveEntryPosition();
		SetPaused(false);
	}
	
	public void ZoneCompleted() {
		EndZone();
		playGUI.ToStory();
	}
	
	public void NextZone() {
		state.SetLevel(zoneID+1);
		StartZone();
	}
	
	public void RepeatZone() {
		SetPaused(true);
		EndZone();
		ship.SetHealthAndShield();
		StartZone();
		SetPaused(false);
	}
	
	private void EndZone() {
		DestroyAllBreadcrumbs();
		enemyDistributor.RemoveAll();
		collecteablesDistributor.RemoveAll();
		cave.RemoveZone();
		botSeed = UnityEngine.Random.Range(0,9999999);
		UnityEngine.Random.seed = caveSeed;
		caveSeed = UnityEngine.Random.Range(1000000,9999999);
		ship.Deactivate();
		ship.LaunchExitHelper(false);
		SetPaused(true);
	}
		
	void onDisable() {
		CancelInvoke();
//		Destroy(ship.gameObject);
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
/*		if (keyCommand.Substring(1, 1) == "e") {
				Enemy e = enemyDistributor.CreateEnemy(null, Enemy.CLAZZ_NUM(keyCommand.Substring(2, 1)), Convert.ToInt32(keyCommand.Substring(3, 2)));
				e.transform.position = GetShipPosition();
				Debug.Log ("Adding Enemy " + keyCommand.Substring(2, 1) + Convert.ToInt32(keyCommand.Substring(3, 2)) + " (Editor mode)");*/
		if (keyCommand.Substring(1, 1) == "d" && keyCommand.Substring(2, 5) == "power") {
				collecteablesDistributor.DropPowerUp(GetShipPosition() + ship.transform.forward * RoomMesh.MESH_SCALE, Weapon.PRIMARY, 0);
				Debug.Log ("Adding Power Up Primary 1 (Editor mode)");
		} else if (keyCommand.Substring(1, 1) == "d" && keyCommand.Substring(2, 6) == "health") {
				collecteablesDistributor.DropHealth(GetShipPosition() + ship.transform.forward * RoomMesh.MESH_SCALE);
				Debug.Log ("Adding Health (Editor mode)");
		} else if (keyCommand.Substring(1, 1) == "d" && keyCommand.Substring(2, 6) == "shield") {
				collecteablesDistributor.DropShield(GetShipPosition() + ship.transform.forward * RoomMesh.MESH_SCALE);
				Debug.Log ("Adding Shield (Editor mode)");
/*		} else if (keyCommand.Substring(1, 1) == "d" && keyCommand.Substring(2, 6) == "scroll") {
				collecteablesDistributor.DropScroll(GetShipPosition() + ship.transform.forward * RoomMesh.MESH_SCALE);
				Debug.Log ("Adding Scroll (Editor mode)");*/
		} else if (keyCommand.Substring(1, 1) == "s") {
				int clazz = Enemy.CLAZZ_NUM(keyCommand.Substring(2, 1));
				int model = Convert.ToInt32(keyCommand.Substring(3, 2));
				enemyDistributor.CreateSpawn(clazz, model, model + EnemyDistributor.CLAZZ_A_EQUIVALENT_MODEL[clazz], GetShipGridPosition());
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
		isShipInPlayableArea = true;
		shipGridPosition = cave.GetGridFromPosition(pos);
//		Debug.Log (pos + " " + shipGridPosition);
		try {
			if (GetRoomOfShip() == null) {
				isShipInPlayableArea = false;
			}
		} catch (IndexOutOfRangeException e) {
			isShipInPlayableArea = false;
		}
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
	
	public bool AddMissile(int type, int amount) {
		// order is highest first
		if (ship.secondaryWeapons[type].ammunition < Game.MAX_MISSILE_AMMO) {
			ship.secondaryWeapons[type].ammunition += amount;
			playGUI.DisplaySecondaryWeapon(ship.secondaryWeapons[ship.currentSecondaryWeapon]);
			return true;
		} else {
			// Todo diplay max ammo hint
			return false;
		}
	}
	
	public void CreateBreadcrumb() {
		Vector3 pos = ship.transform.position + ship.transform.TransformDirection(BREADCRUMB_POSITION);
		if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit,
				ship.transform.TransformDirection(BREADCRUMB_POSITION).magnitude, 1 << Game.LAYER_CAVE)) {
			pos = hit.point;
		}
		breadcrumbs.Add(game.CreateFromPrefab().CreateBreadcrumb(pos, Quaternion.identity));
		if (breadcrumbs.Count > Game.MAX_BREADCRUMBS) {
			Destroy(breadcrumbs[0]);
			breadcrumbs.RemoveAt(0);
		}
	}
	
	private void DestroyAllBreadcrumbs() {
		foreach (GameObject gO in breadcrumbs) {
			Destroy(gO);
		}
		breadcrumbs.Clear();
	}
	
	public void KeyFound(int keyType) {
		isKeyCollected[keyType] = true;
		playGUI.DisplayKey(keyType);
		if (AllKeysCollected()) {
			cave.OpenExitDoor();
			playGUI.DisplayDoorOpen();
		}
	}
	
	private bool AllKeysCollected() {
		if (isKeyCollected[CollecteableKey.TYPE_SILVER] && isKeyCollected[CollecteableKey.TYPE_GOLD]) {
			return true;
		} else {
			return false;
		}
	}

	public void CollectPowerUp(int type, int id) {
		SetPaused(true);
		if (type == Game.POWERUP_PRIMARY_WEAPON) {
			ship.AddPrimaryWeapon(id);
			playGUI.ToPowerUpFound(state.GetDialog(17), state.GetDialog(21+id), PlayGUI.PRIMARY_WEAPONS[id]); 
		} else if (type == Game.POWERUP_SECONDARY_WEAPON) {
			ship.AddSecondaryWeapon(id);
			playGUI.ToPowerUpFound(state.GetDialog(18), state.GetDialog(28+id), PlayGUI.SECONDARY_WEAPONS[id]);
		} else if (type == Game.POWERUP_HULL) {
			ship.SetHull(id);
			playGUI.ToPowerUpFound(state.GetDialog(19), state.GetDialog(32), Game.GUI_UV_SHIELD);
		} else if (type == Game.POWERUP_SPECIAL) {
			ship.AddSpecial(id);
			playGUI.ToPowerUpFound(state.GetDialog(20), state.GetDialog(33+id), PlayGUI.SPECIALS[id]);
		}
	}
	
	public void PlaceOnWall(Vector3 worldPos, Room r, Transform t) {
		Vector3 rayPath = Play.RandomVector();
		int triangleIndex = -1;
		if (Physics.Raycast(worldPos, rayPath, out hit, Game.MAX_VISIBILITY_DISTANCE, 1 << Game.LAYER_CAVE)) {
			if (hit.transform.gameObject.GetInstanceID() == r.roomMesh.transform.gameObject.GetInstanceID()) {
				triangleIndex = hit.triangleIndex;
			} else {
				Debug.Log ("Hit DIFFERENT room while placing object on wall " + hit.transform.name + " " + r.roomMesh.transform.name);
			}
		}
		Mesh mesh = r.roomMesh.mesh;
		if (triangleIndex == -1) {
			triangleIndex = UnityEngine.Random.Range(0, mesh.triangles.Length/3);
		}
		Vector3 v1 = mesh.vertices[mesh.triangles[triangleIndex * 3 + 0]];
		Vector3 v2 = mesh.vertices[mesh.triangles[triangleIndex * 3 + 1]];
		Vector3 v3 = mesh.vertices[mesh.triangles[triangleIndex * 3 + 2]];
		t.position = ((v1 + v2 + v3)/3) * RoomMesh.MESH_SCALE;
		t.forward = hit.normal;
	}
	
	public static Vector3 RandomVector() {
		return new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * ((UnityEngine.Random.Range(0,2) == 0) ? 1 : -1);
	}
	
	private void ConfigureLighting() {
		lightsHolder.rotation = Quaternion.identity;
		lightsHolder.Rotate(UnityEngine.Random.value*360f,UnityEngine.Random.value*360f,UnityEngine.Random.value*360f);
		int lightZone = zoneID % 10;
		if (lightZone >= 7 && lightZone <= 8) {
			lightZone = 2-(8-lightZone); // 1-2
			for (int i=0; i<caveLights.Length; i++) {
				caveLights[i].intensity = 1.0f - (0.45f*lightZone);
			}
		} else {
			for (int i=0; i<caveLights.Length; i++) {
				caveLights[i].intensity = 1.0f;
			}
		}
	}
	
	public void SetPaused(bool toPaused) {
		if (toPaused) {
			isPaused = true;
			Time.timeScale = 0;
			Time.fixedDeltaTime = 0;
		} else {
			isPaused = false;
			Time.timeScale = 1f;
			Time.fixedDeltaTime = 0.0166666f;
		}
	}

	public void BackToMenu() {
		EndZone();
		game.SetGameMode(Game.Mode.Menu);
	}
	
	public void SwitchMode() {
		if (mode == Mode.Normal) {
			SetPaused(true);
			mode = Mode.Sokoban;
			sokoban.SwitchOn();
			playGUI.ToSokoban();
		} else {
			SetPaused(false);
			mode = Mode.Normal;
			sokoban.SwitchOff();
			playGUI.CloseDialog();
		}
	}
	
	public void SokobanSolved() {
		SwitchMode();
		cave.OpenSecretChamberDoor();
		ship.PlaySound(Game.SOUND_TYPE_VARIOUS, 25);
	}
	
	public void RetrySokoban() {
		sokoban.Retry();
	}
	
}

