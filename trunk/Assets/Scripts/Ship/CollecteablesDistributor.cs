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
				game.CreateFromPrefab().CreateShield(e.transform.position, Quaternion.identity, AMOUNT_SHIELD);
				hasDropped = true;
			}
		}
		// HEALTH
		if (!hasDropped && ship.healthPercentage < 100) {
			random100 = UnityEngine.Random.Range(1, 101);
			if (random100 > 80 && random100 > ship.healthPercentage) {
				game.CreateFromPrefab().CreateHealth(e.transform.position, Quaternion.identity, AMOUNT_HEAL);
				hasDropped = true;
			}
		}
		// MISSILES starting from zone 11 on
		if (!hasDropped && e.modelClazzAEquivalent > Ship.SECONDARY_WEAPON_MISSILE_START) {
			if (UnityEngine.Random.value < 0.1f) { // 10% chance
			
				if (e.modelClazzAEquivalent <= Ship.SECONDARY_WEAPON_MISSILE_CHARGED_START) {
					if (e.modelClazzAEquivalent <= Ship.SECONDARY_WEAPON_MISSILE_GUIDED_START) {
						// 1 missile type
						
					} else {
						// chose from 2 missile types
						float rand = UnityEngine.Random.value;
						
					}
				} else {
					// chose from all 3 missile types
				}
			}
		}
	}
}
