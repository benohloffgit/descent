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
				if (ship.currentSecondaryWeapon == Weapon.TYPE_CHARGED_MISSILE) {
					ship.StartChargedMissileTimer();
				} else {
					ship.ShootSecondary();
				}
			}
			if (Input.GetKeyUp("mouse 1") || Input.GetKeyUp(KeyCode.RightControl)) {
				if (ship.currentSecondaryWeapon == Weapon.TYPE_CHARGED_MISSILE) {
					ship.ShootSecondary();
				}
			}
			if (Input.GetKeyDown(KeyCode.Escape)) {
				/*if (isPaused && !play.game.gui.isInDialogMode) {
					SetPaused(false);
					playGUI.CloseDialog();
				}*/
				if (!play.isPaused) {
					play.SetPaused(true);
					play.playGUI.ToQuit();
				}
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
			if (Input.GetKeyDown(KeyCode.F2)) {
				ship.LaunchExitHelper();
			}
			if (play.mode == Play.Mode.Sokoban) {
				if (Input.GetKeyDown(KeyCode.LeftArrow)) {
					play.sokoban.MovePlayer(IntDouble.LEFT);
				} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
					play.sokoban.MovePlayer(IntDouble.RIGHT);
				} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
					play.sokoban.MovePlayer(IntDouble.UP);
				} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
					play.sokoban.MovePlayer(IntDouble.DOWN);
				}
			}
		}
	}
}



