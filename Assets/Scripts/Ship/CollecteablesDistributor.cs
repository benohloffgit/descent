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

	private int powerUpPositionOffsetLast;
	private static Vector3[] POWER_UP_POS_OFFSETS = new Vector3[4] {new Vector3(-0.5f,0,0), new Vector3(0.5f,0,0), new Vector3(0,0.5f,0), new Vector3(0,-0.5f,0)};
		
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
				Debug.Log ("shield drop for " + e.gameObject.GetInstanceID());
			}
		}
		// HEALTH
		if (!hasDropped && ship.healthPercentage < 100) {
			random100 = UnityEngine.Random.Range(1, 101);
			if (random100 > 80 && random100 > ship.healthPercentage) {
				DropHealth(e.transform.position);
				hasDropped = true;
				Debug.Log ("health drop for " + e.gameObject.GetInstanceID());
			}
		}
		Debug.Log ("hasDropped is " + hasDropped);
		// MISSILES starting from zone 2 on
		if (!hasDropped && ship.secondaryWeapons[Weapon.TYPE_MISSILE] != null) {
			float rand = UnityEngine.Random.value;
			if (rand < 0.1f) { // 10% chance
				hasDropped = true;
				Debug.Log ("missile drop for " + e.gameObject.GetInstanceID());
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

	public void DropPowerUp(Vector3 pos, int type, int id) {
		game.CreateFromPrefab().CreatePowerUpDrop(pos, Quaternion.identity, type, id);
	}
	
	public void DropKeys() {
		GridPosition[] keyPositions = new GridPosition[2];
		int positionCount=0;
		foreach (Room r in play.cave.zone.roomList) {
			if (r.exits.Count == 1) { // dead end room
//				Debug.Log(positionCount + " " + r);
//				Debug.Log(r.deadEndCell);
				keyPositions[positionCount] = new GridPosition(r.deadEndCell, r.pos);
				r.SetCellToKey(r.deadEndCell);
				positionCount++;
			}
			if (positionCount == 2) {
				break;
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
		powerUpPositionOffsetLast = 0;
		
		for (int i=0; i<Weapon.SHIP_PRIMARY_WEAPON_AVAILABILITY_MIN.Length; i++) {
			CheckDropPowerUp(Game.POWERUP_PRIMARY_WEAPON, i, Weapon.SHIP_PRIMARY_WEAPON_AVAILABILITY_MIN[i], Weapon.SHIP_PRIMARY_WEAPON_AVAILABILITY_MAX[i]);
		}
		for (int i=0; i<Weapon.SHIP_SECONDARY_WEAPON_AVAILABILITY_MIN.Length; i++) {
			CheckDropPowerUp(Game.POWERUP_SECONDARY_WEAPON, i, Weapon.SHIP_SECONDARY_WEAPON_AVAILABILITY_MIN[i], Weapon.SHIP_SECONDARY_WEAPON_AVAILABILITY_MAX[i]);
		}
		for (int i=0; i<Ship.HULL_POWER_UP.Length; i++) {
			CheckDropPowerUp(Game.POWERUP_HULL, i, Ship.HULL_POWER_UP[i], Ship.HULL_POWER_UP[i]);
		}		
		for (int i=0; i<Ship.SPECIAL_POWER_UP.Length; i++) {
			CheckDropPowerUp(Game.POWERUP_SPECIAL, i, Ship.SPECIAL_POWER_UP[i], Ship.SPECIAL_POWER_UP[i]);
		}
		// TODO IF NO POWER UP DROP; DROP HEALTH OR SHIELD OR AMMO
		
	}
	
	private void CheckDropPowerUp(int type, int id, int min, int max) {
		if (!play.game.state.HasPowerUp(type, id)) {
			if (play.zoneID >= min && play.zoneID <= max) {
				//int parts = (max-play.zoneID);
				//float probability = 1f / (max-play.zoneID+1);
				if (UnityEngine.Random.value <= 1f / (max-play.zoneID+1)) {
					DropPowerUp(GetPositionInSecretChamber(POWER_UP_POS_OFFSETS[powerUpPositionOffsetLast]), type, id);
					powerUpPositionOffsetLast++;
				}
			}
		}
	}
	
	private Vector3 GetPositionInSecretChamber(Vector3 offset) {
		return (play.cave.secretCave.transform.position - play.cave.secretCave.transform.forward*3f*RoomMesh.MESH_SCALE)
			+ play.cave.secretCave.transform.InverseTransformDirection(offset)*RoomMesh.MESH_SCALE;
	}
	
	public void RemoveAll() {
		foreach (GameObject gO in GameObject.FindGameObjectsWithTag(CollecteablePowerUp.TAG)) {
			GameObject.Destroy(gO);
		}
	}
}
