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
					if (Input.GetButtonDown("left") || Input.GetButtonDown("a")) {
						play.sokoban.MovePlayer(IntDouble.LEFT);
					} else if (Input.GetButtonDown("right") || Input.GetButtonDown("d")) {
						play.sokoban.MovePlayer(IntDouble.RIGHT);
					} else if (Input.GetButtonDown("up") || Input.GetButtonDown("w")) {
						play.sokoban.MovePlayer(IntDouble.UP);
					} else if (Input.GetButtonDown("down") || Input.GetButtonDown("s")) {
						play.sokoban.MovePlayer(IntDouble.DOWN);
					}
					if (Input.GetButtonDown("escape")) {
						play.playGUI.ToQuitSokoban();
					}
				} else {
					if (Input.GetButtonDown("escape")) {
						if (play.game.gui.isInDialogMode) {
							play.SetPaused(false);
							play.playGUI.CloseDialog();
						}
					}
				}
			} else {
				if ((Input.GetButtonDown("Fire1"))) {
					if (ship.isCloakOn) {
						play.playGUI.DisplayNotification(play.game.state.GetDialog(56));
					} else {
						ship.ShootPrimary();
					}
				}
				if (Input.GetButtonDown("Fire2")) {
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
				if (Input.GetButtonUp("Fire2")) {
					if (ship.currentSecondaryWeapon == Weapon.TYPE_CHARGED_MISSILE) {
						ship.ShootSecondary();
					}
				}
				if (Input.GetButtonDown("escape")) {
					/*if (isPaused && !play.game.gui.isInDialogMode) {
						SetPaused(false);
						playGUI.CloseDialog();
					}*/
					play.SetPaused(true);
					play.playGUI.ToQuit();
				}
				if (Input.GetButtonDown("f5")) {
					play.SetPaused(true);
					play.playGUI.ToHelp();
				}
				if (Input.GetButtonDown("b")) {
					play.CreateBreadcrumb();
				}
				if (ship.hasSpecial[Ship.SPECIAL_LIGHTS] && Input.GetButtonDown("l")) {
					ship.SwitchHeadlight();
				}
				if (ship.hasSpecial[Ship.SPECIAL_BOOST] &&  Input.GetButtonDown("v")) {
					ship.BoostShip();
				}
				if (ship.hasSpecial[Ship.SPECIAL_CLOAK] &&  Input.GetButtonDown("c")) {
					ship.CloakShip();
				}
				if (ship.hasSpecial[Ship.SPECIAL_INVINCIBLE] &&  Input.GetButtonDown("i")) {
					ship.InvincibleShip();
				}
				if (Input.GetButtonDown("m")) {
					play.SwitchMiniMap();
				}
				if (Input.GetButtonDown("f")) {
					play.SwitchMiniMapFollow();
				}
				if (Input.GetButtonDown("f10")) {
					ship.CycleCamera();
				}
				if (Input.GetButtonDown("page up")) {
					ship.CycleSecondary(1);
				} else if (Input.GetButtonDown("page down")) {
					ship.CycleSecondary(-1);
				}
				if (!Input.GetButtonDown("left alt") && Input.GetAxis("Mouse ScrollWheel") != 0) {
					ship.CyclePrimary(Mathf.FloorToInt(Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"))));
				}
				if (Input.GetButtonDown("t") && play.isShipInPlayableArea) {
					ship.LaunchExitHelper();
				}
				if (Input.GetButtonDown("1")) {
					ship.SetPrimary(Weapon.TYPE_GUN);
				} else if (Input.GetButtonDown("2")) {
					ship.SetPrimary(Weapon.TYPE_LASER);
				} else if (Input.GetButtonDown("3")) {
					ship.SetPrimary(Weapon.TYPE_TWIN_GUN);
				} else if (Input.GetButtonDown("4")) {
					ship.SetPrimary(Weapon.TYPE_PHASER);
				} else if (Input.GetButtonDown("5")) {
					ship.SetPrimary(Weapon.TYPE_TWIN_LASER);
				} else if (Input.GetButtonDown("6")) {
					ship.SetPrimary(Weapon.TYPE_GAUSS);
				} else if (Input.GetButtonDown("7")) {
					ship.SetPrimary(Weapon.TYPE_TWIN_PHASER);
				} else if (Input.GetButtonDown("8")) {
					ship.SetPrimary(Weapon.TYPE_TWIN_GAUSS);
				}
				if (Input.GetButtonDown("f1")) {
					ship.SetSecondary(Weapon.TYPE_MISSILE);
				} else if (Input.GetButtonDown("f2")) {
					ship.SetSecondary(Weapon.TYPE_GUIDED_MISSILE);
				} else if (Input.GetButtonDown("f3")) {
					ship.SetSecondary(Weapon.TYPE_CHARGED_MISSILE);
				} else if (Input.GetButtonDown("f4")) {
					ship.SetSecondary(Weapon.TYPE_DETONATOR_MISSILE);
				}
			}
		}
	}
}



