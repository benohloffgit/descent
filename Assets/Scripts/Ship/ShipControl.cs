using System;
using UnityEngine;
using System.Collections;

public class ShipControl {
	private GameInput gameInput;
	
	private Ship ship;
	private Game game;
//	private RaycastHit hit;
	private Play play;
//	private Shot shotTemplate;
	
//	private static Vector3 GUN_POSITION = new Vector3(0f, 1.5f, 0f);
//	private static Vector3 LASER_POSITION = new Vector3(0f, -1.5f, 0f);
	private static Vector3 BREADCRUMB_POSITION = new Vector3(0f, 0f, 2.0f);
//	private static float MAX_RAYCAST_DISTANCE = 100.0f;
//	private static float RATE_OF_FIRE_SHOT = 0.5f;
//	private static float RATE_OF_FIRE_LASER = 0.3f;
	
	public ShipControl() {
	}
	
	public void Initialize(Ship s, Game g, Play p, GameInput gI) {
		ship = s;
		game = g;
		play = p;
		gameInput = gI;
		
//		InitializeWeapons();
	}

	public void DispatchGameInput() {
		if (gameInput.isMobile) {
		} else {
			if (Input.GetKeyDown("mouse 0") || Input.GetKeyDown(KeyCode.LeftControl)) {
				ship.ShootPrimary();
			}
			if (Input.GetKeyDown("mouse 1") || Input.GetKeyDown(KeyCode.RightControl)) {
				ship.ShootSecondary();
			}
			if (Input.GetKeyDown(KeyCode.B)) {
				game.CreateFromPrefab().CreateBreadcrumb(ship.transform.position + ship.transform.TransformDirection(BREADCRUMB_POSITION), Quaternion.identity);
			}
			if (Input.GetKeyDown(KeyCode.L)) {
				ship.SwitchHeadlight();
			}
			if (Input.GetKeyDown(KeyCode.M)) {
				play.SwitchMiniMap();
			}
			if (Input.GetKeyDown(KeyCode.F)) {
				play.SwitchMiniMapFollow();
			}
			if (Input.GetKeyDown(KeyCode.F1)) {
				ship.CycleCamera();
			}
			if (Input.GetKeyDown(KeyCode.PageUp)) {
				ship.CyclePrimary();
			}
			if (Input.GetKeyDown(KeyCode.PageDown)) {
				ship.CycleSecondary();
			}
		}
	}
}



