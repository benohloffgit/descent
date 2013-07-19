using System;
using UnityEngine;
using System.Collections;

public class ShipControl {
	private GameInput gameInput;
	
	private Ship ship;
//	private Game game;
//	private RaycastHit hit;
	private Play play;
//	private Shot shotTemplate;
	
	public ShipControl() {
	}
	
	public void Initialize(Ship s, Game g, Play p, GameInput gI) {
		ship = s;
//		game = g;
		play = p;
		gameInput = gI;
		
//		InitializeWeapons();
	}

	public void DispatchGameInput() {
		if (gameInput.isMobile) {
		} else {
			if (play.isPaused) {
				if (play.mode == Play.Mode.Sokoban) {
					if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
						play.sokoban.MovePlayer(IntDouble.LEFT);
					} else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
						play.sokoban.MovePlayer(IntDouble.RIGHT);
					} else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
						play.sokoban.MovePlayer(IntDouble.UP);
					} else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
						play.sokoban.MovePlayer(IntDouble.DOWN);
					}
					if (Input.GetKeyDown(KeyCode.Escape)) {
						play.playGUI.ToQuitSokoban();
					}
				} else {
					if (Input.GetKeyDown(KeyCode.Escape)) {
						if (play.game.gui.isInDialogMode) {
							play.SetPaused(false);
							play.playGUI.CloseDialog();
						}
					}
				}
			} else {
				if ((Input.GetKeyDown("mouse 0") || Input.GetKeyDown(KeyCode.LeftControl))) {
					if (ship.isCloakOn) {
						play.playGUI.DisplayNotification(play.game.state.GetDialog(56));
					} else {
						ship.ShootPrimary();
					}
				}
				if (Input.GetKeyDown("mouse 1") || Input.GetKeyDown(KeyCode.RightControl)) {
					if (ship.isCloakOn) {
						play.playGUI.DisplayNotification(play.game.state.GetDialog(56));
					} else {
						if (ship.currentSecondaryWeapon == Weapon.TYPE_CHARGED_MISSILE) {
							ship.StartChargedMissileTimer();
						} else {
							ship.ShootSecondary();
						}
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
					play.SetPaused(true);
					play.playGUI.ToQuit();
				}
				if (Input.GetKeyDown(KeyCode.F5)) {
					play.SetPaused(true);
					play.playGUI.ToHelp();
				}
				if (Input.GetKeyDown(KeyCode.B)) {
					play.CreateBreadcrumb();
				}
				if (ship.hasSpecial[Ship.SPECIAL_LIGHTS] && Input.GetKeyDown(KeyCode.L)) {
					ship.SwitchHeadlight();
				}
				if (ship.hasSpecial[Ship.SPECIAL_BOOST] &&  Input.GetKeyDown(KeyCode.V)) {
					ship.BoostShip();
				}
				if (ship.hasSpecial[Ship.SPECIAL_CLOAK] &&  Input.GetKeyDown(KeyCode.C)) {
					ship.CloakShip();
				}
				if (ship.hasSpecial[Ship.SPECIAL_INVINCIBLE] &&  Input.GetKeyDown(KeyCode.I)) {
					ship.InvincibleShip();
				}
				if (Input.GetKeyDown(KeyCode.M)) {
					play.SwitchMiniMap();
				}
				if (Input.GetKeyDown(KeyCode.F)) {
					play.SwitchMiniMapFollow();
				}
				if (Input.GetKeyDown(KeyCode.F10)) {
					ship.CycleCamera();
				}
				if (Input.GetKeyDown(KeyCode.PageUp)) {
					ship.CycleSecondary(1);
				} else if (Input.GetKeyDown(KeyCode.PageDown)) {
					ship.CycleSecondary(-1);
				}
				if (!Input.GetKey(KeyCode.LeftAlt) && Input.GetAxis("Mouse ScrollWheel") != 0) {
					ship.CyclePrimary(Mathf.FloorToInt(Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"))));
				}
				if (Input.GetKeyDown(KeyCode.T) && play.isShipInPlayableArea) {
					ship.LaunchExitHelper();
				}
				if (Input.GetKeyDown(KeyCode.Alpha1)) {
					ship.SetPrimary(Weapon.TYPE_GUN);
				} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
					ship.SetPrimary(Weapon.TYPE_LASER);
				} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
					ship.SetPrimary(Weapon.TYPE_TWIN_GUN);
				} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
					ship.SetPrimary(Weapon.TYPE_PHASER);
				} else if (Input.GetKeyDown(KeyCode.Alpha5)) {
					ship.SetPrimary(Weapon.TYPE_TWIN_LASER);
				} else if (Input.GetKeyDown(KeyCode.Alpha6)) {
					ship.SetPrimary(Weapon.TYPE_GAUSS);
				} else if (Input.GetKeyDown(KeyCode.Alpha7)) {
					ship.SetPrimary(Weapon.TYPE_TWIN_PHASER);
				} else if (Input.GetKeyDown(KeyCode.Alpha8)) {
					ship.SetPrimary(Weapon.TYPE_TWIN_GAUSS);
				}
				if (Input.GetKeyDown(KeyCode.F1)) {
					ship.SetSecondary(Weapon.TYPE_MISSILE);
				} else if (Input.GetKeyDown(KeyCode.F2)) {
					ship.SetSecondary(Weapon.TYPE_GUIDED_MISSILE);
				} else if (Input.GetKeyDown(KeyCode.F3)) {
					ship.SetSecondary(Weapon.TYPE_CHARGED_MISSILE);
				} else if (Input.GetKeyDown(KeyCode.F4)) {
					ship.SetSecondary(Weapon.TYPE_DETONATOR_MISSILE);
				}
			}
		}
	}
}



