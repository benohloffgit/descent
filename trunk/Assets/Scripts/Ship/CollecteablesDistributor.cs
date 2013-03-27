using System;
using UnityEngine;
using System.Collections;

public class CollecteablesDistributor {
	private Play play;
	private Game game;
	private Ship ship;
	
	private static int AMOUNT_HEAL = 20; // percentage
	
	public CollecteablesDistributor(Play play_) {
		play = play_;
		game = play.game;
		ship = play.ship;
	}
	
	public void DistributeOnEnemyDeath(Enemy e) {
		if (ship.healthPercentage < 100) {
			int random = UnityEngine.Random.Range(1, 101);
			if (random > 66 && random > ship.healthPercentage) {
				game.CreateFromPrefab().CreateHealth(e.transform.position, Quaternion.identity, AMOUNT_HEAL);
			}
		}
	}
}
