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
	public bool[] isKeyCollected;
	public string storyChapter;
	public bool hasDied;
	
	public GridPosition placeShipBeforeExitDoor;
	public GridPosition placeShipBeforeSecretChamberDoor;
	
	private State state;
	private EnemyDistributor enemyDistributor;
	private CollecteablesDistributor collecteablesDistributor;
	private RaycastHit hit;
	private List<int> trianglesUsedForWallPlacement;
	
	public List<Breadcrumb> breadcrumbs = new List<Breadcrumb>();
	
	private Light[] caveLights;
	
	private List<ShotStats> shipShotStats = new List<ShotStats>();
	private List<ShotStats> enemyShotStats = new List<ShotStats>();
	private float shipHitRatio;
	private float enemyHitRatio;
//	private float shipToEnemyHitRatio;
//	private float activeEnemiesHealthToShipFPSRatio;
//	private float shipHealthToActiveEnemiesFPSRatio;
//	private float enemiesToShipFPSRatio;
	
	private GridPosition testPathStart;
	private GridPosition testPathEnd;
	
	// cached values
//	private Vector3 shipPosition;
	private GridPosition shipGridPosition;

	private static float STATS_MIN = 0.01f;
	
	private GameObject[] shotTrailRenderers;
	private int nextShotTrailRenderer;
	private GameObject[] missileExhaustRenderers;
	private int nextMissileExhaustRenderer;
	private ParticleSystem geysirParticleSystem;
	private Transform geysirTransformWithParticleSystem;
	
	private static Vector3 BREADCRUMB_POSITION = new Vector3(0f, 0f, 2.0f);
	private static Vector3 GEYSIR_POSITION = new Vector3(0f, 0f, 0.25f);
	private static int MAX_SHOT_TRAIL_RENDERERS = 20;
	private static int MAX_MISSILE_EXHAUST_RENDERERS = 5;

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
		
		shotTrailRenderers = new GameObject[MAX_SHOT_TRAIL_RENDERERS];
		for (int i=0; i<MAX_SHOT_TRAIL_RENDERERS; i++) {
			shotTrailRenderers[i] =  GameObject.Instantiate(game.bulletTrailRenderer, Vector3.zero, Quaternion.identity) as GameObject;
			shotTrailRenderers[i].renderer.enabled = false;
		}
		nextShotTrailRenderer = 0;

		missileExhaustRenderers = new GameObject[MAX_MISSILE_EXHAUST_RENDERERS];
		for (int i=0; i<MAX_MISSILE_EXHAUST_RENDERERS; i++) {
			missileExhaustRenderers[i] =  GameObject.Instantiate(game.missileExhaustRenderer, Vector3.zero, Quaternion.identity) as GameObject;
		}
		nextMissileExhaustRenderer = 0;
		
		geysirParticleSystem = (GameObject.Instantiate(game.geysirParticleSystemPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<ParticleSystem>();
		geysirTransformWithParticleSystem = null;
		
		SetPaused(true);
	}
	
	void OnGUI() {
//		if (!isPaused) {
/*			GUI.Label(new Rect (20,Screen.height-90,500,80),
					"Active-Living-All: " + enemyDistributor.enemiesActive +"--"+ enemyDistributor.enemiesLiving +"--"+ enemyDistributor.enemiesAll +
					"\nHealth E: " + enemyDistributor.enemiesHealthActive +"--"+ enemyDistributor.enemiesHealthActiveAvg.ToString("F2") +
					"\nFPS S-E-S/E: " + ship.firepowerPerSecond.ToString("F2") +"--"+ enemyDistributor.enemiesFirepowerPerSecondActive.ToString("F2") +"--"+enemiesToShipFPSRatio.ToString("F2")+
					"\nHealth/FPS S-E: " + activeEnemiesHealthToShipFPSRatio.ToString("F2") +"--"+ shipHealthToActiveEnemiesFPSRatio.ToString("F2") +
					"\nHit/Miss S-E-S/E: " + shipHitRatio.ToString("F2") +"--"+enemyHitRatio.ToString("F2") + "--" + shipToEnemyHitRatio.ToString("F2")
				);*/
			
/*			Event e = Event.current;
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
		}*/
   	}
	
	void Update() {
		if (mode == Mode.Sokoban && sokoban.isAnimatingMove) {
			sokoban.DispatchUpdate();
		}
		// editor commands
		if (Application.platform == RuntimePlatform.WindowsEditor) { //  || Application.platform == RuntimePlatform.WindowsPlayer
			/*if (Input.GetKeyDown(KeyCode.F6)) {
				SwitchMode();
			}*/
			if (Input.GetKeyDown(KeyCode.F6)) {	
				ship.transform.position = cave.GetPositionFromGrid(placeShipBeforeSecretChamberDoor);
			}
			if (Input.GetKeyDown(KeyCode.F7)) {	
				ship.transform.position = cave.GetPositionFromGrid(placeShipBeforeSecretChamberDoor);
				SokobanSolved();
			}
			if (Input.GetKeyDown(KeyCode.F8)) {	
				KeyFound(CollecteableKey.TYPE_SILVER);
				KeyFound(CollecteableKey.TYPE_GOLD);
				ship.transform.position = cave.GetPositionFromGrid(placeShipBeforeExitDoor);
			}
			if (Input.GetKeyDown(KeyCode.F9)) {	
				ship.isInvincibleOn = ship.isInvincibleOn ? false : true;
			}
			if (Input.GetKeyDown(KeyCode.F11)) {	
				ship.Damage(100, Vector3.zero, Shot.BULLET);
			}
		}
	}
	
	void FixedUpdate() {
		if (!isPaused) {
			playGUI.DispatchFixedUpdate();
		}
	}

	public void Activate() {
		hasDied = false;
		state.SetPreferenceSokobanSolved(false);
		zoneID = state.level;
		playGUI.Activate();
		ship.CalculateHullClazz();
		ship.AddSpecials();
		ship.AddWeapons();
		StartZone();
		InvokeRepeating("UpdateStats", 0, 1.0f);
	}
	
	public void Deactivate() {
		playGUI.Deactivate();
		ship.RemoveWeapons();
		CancelInvoke();
	}

	private void StartZone() {
		shipHitRatio = 0;
		enemyHitRatio = 0;
		isShipInPlayableArea = false;
		Debug.Log (" ------------------------------- Cave Seed: " + caveSeed);
		if (hasDied) {
			cave.ResetDoors();
			playGUI.Reset();
			if (isKeyCollected[CollecteableKey.TYPE_SILVER]) {
				KeyFound(CollecteableKey.TYPE_SILVER);
			}
			if (isKeyCollected[CollecteableKey.TYPE_GOLD]) {
				KeyFound(CollecteableKey.TYPE_GOLD);
			}
			ship.Reset();
			miniMap.Reset();
			playGUI.ToHasDied();
		} else {
			trianglesUsedForWallPlacement = new List<int>();
			UnityEngine.Random.seed = caveSeed;
			cave.AddZone(zoneID);
			UnityEngine.Random.seed = botSeed;
			isKeyCollected = new bool[] {false, false};
			collecteablesDistributor.DropKeys();
			if (zoneID > 0) {
				enemyDistributor.Distribute();
			}
			sokoban.RenderLevel(zoneID);
			if (!state.GetPreferenceSokobanSolved()) {
				collecteablesDistributor.DropPowerUps();
			}
			playGUI.Reset();
			storyChapter = (Resources.Load("Story/EN/" + zoneID, typeof(TextAsset)) as TextAsset).text;
			playGUI.ToStory();
			ConfigureLighting();
			ship.Reset();
			miniMap.Reset();
		}
		hasDied = false;
	}
	
	public void ZoneCompleted() {
		hasDied = false;
		state.SetPreferenceSokobanSolved(false);
		EndZone();
		NextZone();
		StartCoroutine(DelayedStartZone());

		// 1 Level Demo
//		playGUI.To1LevelDemoEnd();
}
	
	private void NextZone() {
		zoneID++;
		state.SetLevel(zoneID);
	}
	
	public void RepeatZone() {
		hasDied = true;
		EndZone();
		ship.SetHealthAndShield();
		StartCoroutine(DelayedStartZone());
	}
	
	private void EndZone() {
		if (!hasDied) {
			DestroyAllBreadcrumbs();
			miniMap.DestroyAllBreadcrumbs();
			collecteablesDistributor.RemoveAllKeys();
			enemyDistributor.RemoveAll();
			collecteablesDistributor.RemoveAllPowerUps();
			cave.RemoveZone();
			botSeed = UnityEngine.Random.Range(0,9999999);
			UnityEngine.Random.seed = caveSeed;
			caveSeed = UnityEngine.Random.Range(1000000,9999999);
		}
		ship.Deactivate();
		ship.LaunchExitHelper(false);
		SetPaused(true);
	}
	
	IEnumerator DelayedStartZone() {
//		Debug.Log ("DelayedStartZone");
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + 0.5f) {
//			Debug.Log ("returning null");
			yield return null;
		}
//		Debug.Log ("DelayedStartZone end");
		StartZone();
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
			Game.DefNull(e);
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
//		shipToEnemyHitRatio = shipHitRatio / enemyHitRatio;
//		activeEnemiesHealthToShipFPSRatio = enemyDistributor.enemiesHealthActive/ship.firepowerPerSecond;
//		shipHealthToActiveEnemiesFPSRatio = ship.health/enemyDistributor.enemiesFirepowerPerSecondActive;
//		enemiesToShipFPSRatio = enemyDistributor.enemiesFirepowerPerSecondActive/ship.firepowerPerSecond;
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
		// move a bit in direction of ship to avoid being culled by walls etc.
		game.CreateFromPrefab().CreateExplosion(pos + (GetShipPosition()-pos).normalized * (RoomMesh.MESH_SCALE/3f), rot);
	}

	public void DisplayHit(Vector3 pos, Quaternion rot, string tag, int source) {
		Vector3 newPos = pos + (GetShipPosition()-pos).normalized * (RoomMesh.MESH_SCALE/5f);
		if (tag == Enemy.TAG) {
			game.PlaySound(newPos, UnityEngine.Random.Range(41,44));
		} else if (source == Game.SHIP) {
			game.PlaySound(newPos, UnityEngine.Random.Range(38,41));
		}
		game.CreateFromPrefab().CreateHit(newPos, rot, tag);
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
			playGUI.DisplaySecondaryWeapon();
			ship.PlaySound(Game.SOUND_TYPE_VARIOUS, 37);
			return true;
		} else {
			playGUI.DisplayNotification(state.GetDialog(55));
			return false;
		}
	}
	
	public void CreateBreadcrumb() {
		Vector3 pos = ship.transform.position + ship.transform.TransformDirection(BREADCRUMB_POSITION);
		if (Physics.Raycast(ship.transform.position, ship.transform.forward, out hit,
				ship.transform.TransformDirection(BREADCRUMB_POSITION).magnitude, 1 << Game.LAYER_CAVE)) {
			pos = hit.point;
		}
		breadcrumbs.Add(game.CreateFromPrefab().CreateBreadcrumb(pos, Quaternion.identity, GetRoomOfShip().id));
		miniMap.SetBreadcrumb(pos);
		if (breadcrumbs.Count > Game.MAX_BREADCRUMBS) {
			Destroy(breadcrumbs[0].gameObject);
			breadcrumbs.RemoveAt(0);
			miniMap.RemoveBreadcrumb();
		}
	}
	
	private void DestroyAllBreadcrumbs() {
		foreach (Breadcrumb b in breadcrumbs) {
			Destroy(b.gameObject);
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
			playGUI.ToPowerUpFound(state.GetDialog(18), state.GetDialog(48+id), PlayGUI.SECONDARY_WEAPONS[id]);
		} else if (type == Game.POWERUP_HULL) {
			ship.SetHull(id);
			playGUI.ToPowerUpFound(state.GetDialog(19), state.GetDialog(32), Game.GUI_UV_SHIELD);
		} else if (type == Game.POWERUP_SPECIAL) {
			ship.AddSpecial(id);
			playGUI.ToPowerUpFound(state.GetDialog(20), state.GetDialog(33+id), PlayGUI.SPECIALS[id]);
		}
	}
	
	public void PlaceOnWall(Vector3 worldPos, Room r, Transform t) {
		int triangleIndex = -1;
		Vector3 rayPath;
		while (triangleIndex == -1) {
			rayPath = Play.RandomVector();
			if (Physics.Raycast(worldPos, rayPath, out hit, Game.MAX_VISIBILITY_DISTANCE, 1 << Game.LAYER_CAVE)) {
				if (hit.transform.gameObject.GetInstanceID() == r.roomMesh.transform.gameObject.GetInstanceID()) {
					if (!trianglesUsedForWallPlacement.Contains(hit.triangleIndex)) {
						triangleIndex = hit.triangleIndex;
						trianglesUsedForWallPlacement.Add(triangleIndex);
					}
				}
			}
		}
		Mesh mesh = r.roomMesh.mesh;
/*		if (triangleIndex == -1) {
			triangleIndex = UnityEngine.Random.Range(0, mesh.triangles.Length/3);
		}*/
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
		state.SetPreferenceSokobanSolved(true);
	}
	
	public void RetrySokoban() {
		sokoban.Retry();
	}
	
	public GameObject GetNextShotTrailRenderer() {
		int n = nextShotTrailRenderer;
		nextShotTrailRenderer++;
		if (nextShotTrailRenderer == MAX_SHOT_TRAIL_RENDERERS) {
			nextShotTrailRenderer = 0;
		}
		return shotTrailRenderers[n];
	}

	public GameObject GetNextMissileExhaustRenderer() {
		int n = nextMissileExhaustRenderer;
		nextMissileExhaustRenderer++;
		if (nextMissileExhaustRenderer == MAX_MISSILE_EXHAUST_RENDERERS) {
			nextMissileExhaustRenderer = 0;
		}
		return missileExhaustRenderers[n];
	}
	
	public void SetGeysirParticleSystem(Transform geysir) {
		if (geysirTransformWithParticleSystem == null) {
			geysirParticleSystem.transform.parent = geysir;
			geysirParticleSystem.transform.localPosition = GEYSIR_POSITION;
			geysirParticleSystem.transform.localRotation = Quaternion.identity;
			geysirParticleSystem.transform.localScale = Vector3.one;
			geysirTransformWithParticleSystem = geysir;
			geysirParticleSystem.Play();
		}
	}

	public void LetGeysirParticleSystem() {
		if (geysirParticleSystem != null) {
			geysirParticleSystem.transform.parent = null;
			geysirTransformWithParticleSystem = null;
			geysirParticleSystem.Stop();
		}
	}

}

