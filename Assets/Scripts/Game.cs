using System;
using UnityEngine;

public class Game : MonoBehaviour {
	public GameObject statePrefab;
	public GameObject guiPrefab;
	public GameObject mesh3DPrefab;

	public GameObject emptyPrefab;
	public GameObject roomMeshPrefab;
	public GameObject roomEntryPrefab;
	public GameObject roomConnectorPrefab;
	public GameObject secretCavePrefab;
	public GameObject miniMapPrefab;
	public GameObject miniMapRoomConnectorPrefab;
	public GameObject doorPrefab;
	public GameObject secretChamberDoorPrefab;
	public GameObject crystalPrefab;
	public GameObject flowerPrefab;
	
	public GameObject testCubePrefab;
	public GameObject shipPrefab;
	public GameObject exitHelperPrefab;
	public GameObject wallGunPrefab;
	public GameObject wallLaserPrefab;
	public GameObject mineBuilderPrefab;
	public GameObject lightBulbPrefab;
	public GameObject mantaPrefab;
	public GameObject pikePrefab;
	public GameObject bullPrefab;
	public GameObject bugPrefab;
	public GameObject snakePrefab;
	public GameObject hornetPrefab;
	public GameObject bulbPrefab;
	public GameObject wombatPrefab;
	public GameObject batPrefab;
	public GameObject gazellePrefab;
	public GameObject spiderPrefab;
	public GameObject rhinoPrefab;
	public GameObject[] primaryWeaponPrefabs;
	public GameObject[] powerUpSecondaryPrefabs;
	public GameObject powerUpHullPrefab;
	public GameObject powerUpSpecialPrefab;
	public GameObject keyPrefab;
	public GameObject manaPrefab;
	public GameObject spawnPrefab;

	// those are objects create while game is running (after cave generation)
	public GameObject[] shotPrefabs;
	public GameObject breadcrumbPrefab;
	public GameObject explosionPrefab;
	public GameObject hitPrefab;
	public GameObject sokobanBoardPrefab;
	// drops
	public GameObject healthDropPrefab;
	public GameObject shieldDropPrefab;
	public GameObject[] missileDropPrefabs;

	public Texture2D[] caveTextures;
	public Texture2D[] keyTextures;
	public Material sokobanMaterial;
//	public Material 
	
	public State state;
	public GameInput gameInput;
	public MyGUI gui;
	public bool isInitialized = false;
	public enum Mode { Menu=0, Play=2, None=3, Preferences=4 }
	
	public static Vector3 CELL_CENTER = new Vector3(0.5f,0.5f,0.5f);
	
	public static int SHIP = 0;
	public static int ENEMY = 1;
	
	public static int MAX_BREADCRUMBS = 10;
	public static int MAX_MISSILE_AMMO = 10;
	
//	public static int HEALTH_MODIFIER = 5;
	public static int BEGINNER_ZONES = 4;
	
	public static int LAYER_CAVE = 8;
	public static int LAYER_SHIP = 9;
	public static int LAYER_GUI = 10;
	public static int LAYER_BULLETS = 11;
	public static int LAYER_ENEMIES = 12;
	public static int LAYER_EFFECT = 13;
	public static int LAYER_MINI_MAP = 14;
	public static int LAYER_GUN_ENEMY = 15;
	public static int LAYER_GUN_SHIP = 16;
	public static int LAYER_COLLECTEABLES = 17;
	public static int LAYER_MINES = 18;
	public static int LAYER_SOKOBAN = 19;
	public static int DOOR_TRIGGER = 20;
	
	public static int DIMENSION_ZONE = 3; // BxB rooms
	public static int DIMENSION_ZONE_SQUARED = DIMENSION_ZONE * DIMENSION_ZONE;
	public static int DIMENSION_ZONE_CUBED = DIMENSION_ZONE_SQUARED * DIMENSION_ZONE;
	public static int DIMENSION_ROOM = 16; // CxC cells
	public static int DIMENSION_ROOM_SQUARED = DIMENSION_ROOM * DIMENSION_ROOM;
	public static int DIMENSION_ROOM_CUBED = DIMENSION_ROOM_SQUARED * DIMENSION_ROOM;
	// max diagonal line of our room cube, roughly 27 units times mesh scale
	public static float MAX_VISIBILITY_DISTANCE = RoomMesh.MESH_SCALE * Mathf.Sqrt( Mathf.Pow(Mathf.Sqrt(Mathf.Pow(DIMENSION_ROOM,2)*2),2) + Mathf.Pow(DIMENSION_ROOM,2));

	public static int LAYER_MASK_ALL = ( (1 << LAYER_SHIP) | (1 << LAYER_ENEMIES) | (1 << LAYER_CAVE) );
	public static int LAYER_MASK_SHIP_CAVE = ( (1 << LAYER_SHIP) | (1 << LAYER_CAVE) );
	public static int LAYER_MASK_SHIP = 1 << LAYER_SHIP;
	public static int LAYER_MASK_CAVE = 1 << LAYER_CAVE;
	public static int LAYER_MASK_MOVEABLES = ( (1 << Game.LAYER_SHIP) | (1 << Game.LAYER_ENEMIES) );
	public static int LAYER_MASK_ENEMIES_CAVE = ( (1 << Game.LAYER_CAVE) | (1 << Game.LAYER_ENEMIES) );
	public static int LAYER_MASK_CAMERA_WITHOUT_SHIP = ( (1 << Game.LAYER_CAVE) | (1 << Game.LAYER_BULLETS) | (1 << Game.LAYER_GUN_ENEMY) | (1 << Game.LAYER_EFFECT) | (1 << Game.LAYER_COLLECTEABLES) | (1 << Game.LAYER_ENEMIES) );
	public static int LAYER_MASK_CAMERA_WITH_SHIP = ( (1 << Game.LAYER_SHIP) | (1 << Game.LAYER_GUN_SHIP) | (1 << Game.LAYER_CAVE) | (1 << Game.LAYER_BULLETS) | (1 << Game.LAYER_EFFECT) | (1 << Game.LAYER_COLLECTEABLES) | (1 << Game.LAYER_GUN_ENEMY) | (1 << Game.LAYER_ENEMIES) );

	public static int WEAPON_POSITION_WING_LEFT = 0;
	public static int WEAPON_POSITION_WING_RIGHT = 1;
	public static int WEAPON_POSITION_CENTER = 2;

	public static int POWERUP_PRIMARY_WEAPON = 0;
	public static int POWERUP_SECONDARY_WEAPON = 1;
	public static int POWERUP_HULL = 2;
	public static int POWERUP_SPECIAL = 3;	
	
	public static Vector4 GUI_UV_NULL = new Vector4(0.0f,0.0f,0.0f,0.0f);
	public static Vector4 GUI_UV_COLOR_BLACK = new Vector4(0.0f,0.0f,0.0625f,0.0625f);
	public static Vector4 GUI_UV_DIM = new Vector4(0.0625f,0.0f,0.125f,0.0625f);
	public static Vector4 GUI_UV_NUMBER_0 = new Vector4(0.0f,0.875f,0.125f,1.0f);
	public static Vector4 GUI_UV_NUMBER_1 = new Vector4(0.125f,0.875f,0.25f,1.0f);
	public static Vector4 GUI_UV_NUMBER_2 = new Vector4(0.25f,0.875f,0.375f,1.0f);
	public static Vector4 GUI_UV_NUMBER_3 = new Vector4(0.375f,0.875f,0.5f,1.0f);
	public static Vector4 GUI_UV_NUMBER_4 = new Vector4(0.5f,0.875f,0.625f,1.0f);
	public static Vector4 GUI_UV_NUMBER_5 = new Vector4(0.625f,0.875f,0.75f,1.0f);
	public static Vector4 GUI_UV_NUMBER_6 = new Vector4(0.75f,0.875f,0.875f,1.0f);
	public static Vector4 GUI_UV_NUMBER_7 = new Vector4(0.875f,0.875f,1.0f,1.0f);
	public static Vector4 GUI_UV_NUMBER_8 = new Vector4(0.0f,0.75f,0.125f,0.875f);
	public static Vector4 GUI_UV_NUMBER_9 = new Vector4(0.125f,0.75f,0.25f,0.875f);
	public static Vector4 GUI_UV_KEY_SILVER = new Vector4(0.25f,0.75f,0.375f,0.875f);
	public static Vector4 GUI_UV_KEY_GOLD = new Vector4(0.375f,0.75f,0.5f,0.875f);
	public static Vector4 GUI_UV_KEY_EMPTY = new Vector4(0.5f,0.75f,0.625f,0.875f);
	public static Vector4 GUI_UV_DOOR_CLOSED = new Vector4(0.625f,0.75f,0.75f,0.875f);
	public static Vector4 GUI_UV_DOOR_OPEN = new Vector4(0.75f,0.75f,0.875f,0.875f);
	
	private Menu menu;
	private Play play;
	private PrefabFactory prefabFactory;
	public bool showTrialDialog;
	private float volume;
		
	void Awake() {
		DontDestroyOnLoad(this);
		
		Application.targetFrameRate = 60;
		Application.runInBackground = true;
		volume = AudioListener.volume;
		showTrialDialog = false;
//		Debug.Log ("MAX_VISIBILITY_DISTANCE " + MAX_VISIBILITY_DISTANCE);
//		Screen.lockCursor = true;
		//Screen.showCursor = false;
	}
	
	public void Initialize(Mode m) {
		GameObject o;
		if (!isInitialized) {
				
			o = Instantiate(statePrefab) as GameObject;
			state = o.GetComponent<State>();
			state.Initialize(this);
			
			gameInput = GetComponent<GameInput>();

			gui = (GameObject.Instantiate(guiPrefab) as GameObject).GetComponent<MyGUI>();
			gui.Initialize(this, gameInput);
			gui.CenterOnScreen(gui.transform);
			gui.ResizeToScreenSize(gui.transform);
			
			o = GameObject.Find("/Menu");
			if (o != null) {
				menu = o.GetComponent<Menu>();
				menu.Initialize(this);
			}
			o = GameObject.Find("/Play");
			if (o != null) {
				play = o.GetComponent<Play>();
				play.Initialize(this);
			}
			
			prefabFactory = new PrefabFactory(this);
			prefabFactory.Initialize(play);
				
			isInitialized = true;
		}
		
		gameInput.enabled = true;
		

		if (state.gameMode != Game.Mode.None) {
			SetGameMode(state.gameMode);
		} else {
			SetGameMode(m);
		}
	}
	
	public void SetGameMode(Mode mode) {
//		gameInput.DeRegisterGUI();
		state.gameMode = mode;
		if (mode == Mode.Menu) {
			Screen.lockCursor = false;
			Screen.showCursor = true;
			Screen.sleepTimeout = SleepTimeout.SystemSetting;
			play.Deactivate();
			menu.Activate();
		} else if (mode == Mode.Play) {
			Screen.lockCursor = true;
			Screen.showCursor = false;
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			menu.Deactivate();			
			play.Activate();
		}
	}
		
	public void NonGUIClickInDialog() {
	}
	
	public void DispatchGameInput() {
		if (state.gameMode == Mode.Play) {
			play.DispatchGameInput();
		}
	}
	
	public void PlayPayed() {
		BuyProduct(0);
	}
			
	public void RateIt() {
		state.jniBridge.ShowMarketDialog();
	}
			
	public void SwitchToProductAcquired(int id) {
		if (id == 0) {
			state.HideAd();
			if (state.gameMode == Game.Mode.Menu) {
				menu.RemovePaygate();
			}
		} else {
			state.ShowAd();
			if (state.gameMode == Game.Mode.Menu) {
				menu.ShowPaygate();
			}
		}
	}
	
	public void BuyProduct(int id) {
		state.jniBridge.BuyProduct(id);
//		state.jniBridge.TrackByFlurry(1);
	//	Debug.Log("buying product");
	}
	
	public PrefabFactory CreateFromPrefab() {
		return prefabFactory;
	}
	
	public static void DefNull(object o) {
	}
	
}

