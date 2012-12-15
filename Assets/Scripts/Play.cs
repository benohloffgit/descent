using System;
using UnityEngine;
using System.Collections;

public class Play : MonoBehaviour {	
	public GameObject guiPrefab;
	public GameObject marchingCubesPrefab;
	public GameObject shipPrefab;
	public GameObject wallGunPrefab;
	public GameObject mineTouchPrefab;

	public static int ROOM_SIZE = 16;

	private Game game;
//	private GameInput gI;
	private State state;
	private MyGUI gui;
	private EnemyDistributor enemyDistributor;
	private RaycastHit hit;

	private int container;
	private int dialogContainer;
	
	private Ship ship;
	private MarchingCubes marchingCubes;

	private static float MAX_RAYCAST_DISTANCE = 100.0f;
	
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
				Vector3 cubePositionOfShip = ship.GetCubePosition();
				MineTouch mineTouch = enemyDistributor.CreateMineTouch();
				mineTouch.transform.position = cubePositionOfShip * MarchingCubes.MESH_SCALE;
				Debug.Log ("Adding Mine Touch (Editor mode)");
			}
		}
	}
	
	public void Restart() {
		marchingCubes = (GameObject.Instantiate(marchingCubesPrefab) as GameObject).GetComponent<MarchingCubes>();
		ship = (GameObject.Instantiate(shipPrefab) as GameObject).GetComponent<Ship>();
		ship.Initialize(this, game);
		marchingCubes.Initialize(ship.transform, ROOM_SIZE);
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
		Destroy(marchingCubes.gameObject);
	}
	
	public void DispatchGameInput() {
		ship.DispatchGameInput();
	}
	
	private void PlaceEnemies() {
		enemyDistributor = new EnemyDistributor(game, this, marchingCubes, ROOM_SIZE);
		enemyDistributor.Distribute();
	}
	
	public Vector3 GetShipPosition() {
		// TODO cache
		return ship.transform.position;
	}
}

