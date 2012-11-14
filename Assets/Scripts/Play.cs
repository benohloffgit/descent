using System;
using UnityEngine;
using System.Collections;

public class Play : MonoBehaviour {	
	public GameObject guiPrefab;
	public GameObject marchingCubesPrefab;
	public GameObject shipPrefab;

	private Game game;
//	private GameInput gI;
	private State state;
	private MyGUI gui;

	private int container;
	private int dialogContainer;
	
	private Ship ship;
	private MarchingCubes marchingCubes;
		
	void Awake() {
	}
		
	void Update() {
	}
	
	public void Restart() {
		marchingCubes = (GameObject.Instantiate(marchingCubesPrefab) as GameObject).GetComponent<MarchingCubes>();
		ship = (GameObject.Instantiate(shipPrefab) as GameObject).GetComponent<Ship>();
		ship.Initialize(this, game);
		marchingCubes.Initialize(ship.transform);
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
}

