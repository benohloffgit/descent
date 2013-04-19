using System;
using UnityEngine;
using System.Collections;

public class CollecteablesDistributor {
	private Play play;
	private Game game;
	private Ship ship;
	
	private static int AMOUNT_HEAL = 15; // percentage
	private static int AMOUNT_SHIELD = 25; // percentage
	
	public CollecteablesDistributor(Play play_) {
		play = play_;
		game = play.game;
		ship = play.ship;
	}
	
	public void DistributeOnEnemyDeath(Enemy e) {
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
		// MISSILES starting from zone 11 on
		if (!hasDropped && e.modelClazzAEquivalent > Weapon.MISSILE_START) {
			float rand = UnityEngine.Random.value;
			if (rand < 0.1f) { // 10% chance
				hasDropped = true;
				if (e.modelClazzAEquivalent <= Weapon.MISSILE_DETONATOR_START) {
					if (e.modelClazzAEquivalent <= Weapon.MISSILE_CHARGED_START) {
						// chose from 2 missile types
						rand = UnityEngine.Random.value;
						if (rand < 0.3f) {
							DropMissile(e, Weapon.TYPE_MISSILE, 3);
						} else {
							DropMissile(e, Weapon.TYPE_GUIDED_MISSILE, 1);
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
	
	public void DropScroll(Vector3 pos) {
		game.CreateFromPrefab().CreateScrollDrop(pos, Quaternion.identity);
	}

	private void DropMissile(Enemy e, int type, int amount) {
		game.CreateFromPrefab().CreateMissileDrop(e.transform.position, Quaternion.identity, type, amount);
	}
}
