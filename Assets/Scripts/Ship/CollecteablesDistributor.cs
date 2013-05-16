using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollecteablesDistributor {
	private Play play;
	private Game game;
	private Ship ship;
//	private List<GameObject> drops;
	
	private static int AMOUNT_HEAL = 35;
	private static int AMOUNT_SHIELD = 20;
	
	public CollecteablesDistributor(Play play_) {
		play = play_;
		game = play.game;
		ship = play.ship;
//		drops = new List<GameObject>();
	}
	
	public void DistributeOnEnemyDeath(Enemy e) {
		if (e.clazzNum == Enemy.CLAZZ_WALLLASER11) {
			return;
		}
		int random100;
		bool hasDropped = false;
		// SHIELD
		if (!hasDropped && ship.shieldPercentage < 100) {
			random100 = UnityEngine.Random.Range(1, 101);
			if (random100 > 66 && random100 > ship.shieldPercentage) {
				DropShield(e.transform.position);
				hasDropped = true;
			}
		}
		// HEALTH
		if (!hasDropped && ship.healthPercentage < 100) {
			random100 = UnityEngine.Random.Range(1, 101);
			if (random100 > 80 && random100 > ship.healthPercentage) {
				DropHealth(e.transform.position);
				hasDropped = true;
			}
		}
		// MISSILES starting from zone 2 on
		if (!hasDropped && ship.secondaryWeapons[Weapon.TYPE_MISSILE] != null) {
			float rand = UnityEngine.Random.value;
			if (rand < 0.1f) { // 10% chance
				hasDropped = true;
				if (ship.secondaryWeapons[Weapon.TYPE_DETONATOR_MISSILE] == null) {
					if (ship.secondaryWeapons[Weapon.TYPE_CHARGED_MISSILE] == null) {
						if (ship.secondaryWeapons[Weapon.TYPE_GUIDED_MISSILE] == null) {
								DropMissile(e, Weapon.TYPE_MISSILE, 2);
						} else {
							// chose from 2 missile types
							rand = UnityEngine.Random.value;
							if (rand < 0.3f) {
								DropMissile(e, Weapon.TYPE_MISSILE, 3);
							} else {
								DropMissile(e, Weapon.TYPE_GUIDED_MISSILE, 1);
							}
						}
					} else {
						// chose from 3 missile types
						rand = UnityEngine.Random.value;
						if (rand < 0.5f) {
							DropMissile(e, Weapon.TYPE_MISSILE, 4);
						} else if (rand < 0.8f) {
							DropMissile(e, Weapon.TYPE_GUIDED_MISSILE, 2);
						} else {
							DropMissile(e, Weapon.TYPE_CHARGED_MISSILE, 1);
						}						
					}
				} else {
					// chose from 4 missile types
					rand = UnityEngine.Random.value;
					if (rand < 0.4f) {
						DropMissile(e, Weapon.TYPE_MISSILE, 5);
					} else if (rand < 0.7f) {
						DropMissile(e, Weapon.TYPE_GUIDED_MISSILE, 3);
					} else if (rand < 0.9f) {
						DropMissile(e, Weapon.TYPE_CHARGED_MISSILE, 2);
					} else {
						DropMissile(e, Weapon.TYPE_DETONATOR_MISSILE, 1);
					}						
				}
			}
		}
	}
	
	public void DropHealth(Vector3 pos) {
		game.CreateFromPrefab().CreateHealthDrop(pos, Quaternion.identity, AMOUNT_HEAL);
	}

	public void DropShield(Vector3 pos) {
		game.CreateFromPrefab().CreateShieldDrop(pos, Quaternion.identity, AMOUNT_SHIELD);
	}		
	
	private void DropMissile(Enemy e, int type, int amount) {
		game.CreateFromPrefab().CreateMissileDrop(e.transform.position, Quaternion.identity, type, amount);
	}
	
	public void DropKey(Vector3 pos, int keyType) {
		game.CreateFromPrefab().CreateKeyDrop(pos, Quaternion.identity, keyType);
	}

	public void DropPowerUp(Vector3 pos, int weaponType, int index) {
		game.CreateFromPrefab().CreatePowerUpDrop(pos, Quaternion.identity, weaponType, index);
	}
	
	public void DropKeys() {
		GridPosition[] keyPositions = new GridPosition[2];
		int positionCount=0;
		foreach (Room r in play.cave.zone.roomList) {
			if (r.exits.Count == 1) { // dead end room
				keyPositions[positionCount] = new GridPosition(r.deadEndCell, r.pos);
				r.SetCellToKey(r.deadEndCell);
				positionCount++;
			}
			if (positionCount == 2) {
				continue;
			}
		}
		while (positionCount < 2) {
			Room r = play.cave.zone.GetRandomRoom();
			keyPositions[positionCount] = r.GetRandomVoidGridPosition();
			r.SetCellToKey(keyPositions[positionCount].cellPosition);
			positionCount++;
		}
		int rand1 = UnityEngine.Random.Range (0,2);
		int rand2 = (rand1 == 0) ? 1 : 0;
		DropKey(keyPositions[rand1].GetWorldVector3(), CollecteableKey.TYPE_SILVER);
		DropKey(keyPositions[rand2].GetWorldVector3(), CollecteableKey.TYPE_GOLD);
	}
	
	public void DropPowerUps() {
/*		if (Weapon.SHIP_PRIMARY_WEAPON_TYPES[play.zoneID] != 0) {
			Room r = play.cave.zone.GetRandomRoom();
			GridPosition gP = r.GetRandomVoidGridPosition();
			DropPowerUp(gP.GetWorldVector3(), Weapon.PRIMARY, play.zoneID);
			r.SetCellToPowerUp(gP.cellPosition);
		}
		if (Weapon.SHIP_SECONDARY_WEAPON_TYPES[play.zoneID] != 0) {
			Room r = play.cave.zone.GetRandomRoom();
			GridPosition gP = r.GetRandomVoidGridPosition();
			DropPowerUp(gP.GetWorldVector3(), Weapon.SECONDARY, play.zoneID);
			r.SetCellToPowerUp(gP.cellPosition);
		}*/
	}
	
	public void RemoveAll() {
		foreach (GameObject gO in GameObject.FindGameObjectsWithTag(CollecteablePowerUp.TAG)) {
			GameObject.Destroy(gO);
		}
	}
}
