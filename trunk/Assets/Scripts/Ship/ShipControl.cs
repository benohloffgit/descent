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
				play.CreateBreadcrumb();
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



