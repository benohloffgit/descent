using System;
using UnityEngine;
using System.Collections;

public class EnemyDistributor {
	private Play play;
	private Game game;
	
	private RaycastHit hit;
	
	public int enemiesLiving;
	public int enemiesActive;
	public int enemiesAll;
	public int enemiesHealth;
	public int enemiesHealthActive;
	public int enemiesHealthActiveAvg;
	public float enemiesFirepowerPerSecond;
	public float enemiesFirepowerPerSecondAvg;
	public float enemiesFirepowerPerSecondActive;
	public float enemiesFirepowerPerSecondAvgActive;
	public float enemiesHitRatio;
	
//	private static float MAX_RAYCAST_DISTANCE = 100.0f;
	
	private static float[] ENEMY_HEALTH_MULTIPLICATOR = new float[] {3.0f, 3.125f, 3.25f, 3.375f, 3.5f, 3.675f, 3.75f, 3.875f, 4.0f };
	private static float[] ENEMY_HEALTH_MODIFIER = new float[] {-0.1f, -0.075f, -0.05f, -0.025f, 0f, 0.025f, 0.05f, 0.075f, 0.1f };
	private static float[] ENEMY_SIZES = new float[] {1.0f, 0.5f, 1.5f, 0.7f, 1.3f, 0.3f, 0.7f, 1.7f, 2.0f, 1.0f};
	private static float[] ENEMY_AGGRESSIVENESSES = new float[] {0.05f, 0.2f, 0.5f, 0.1f, 0.7f, 0.3f, 0.2f, 0.6f, 0.4f, 0.1f, 0.3f};
	private static float[] ENEMY_MOVEMENT_FORCES = new float[] {5.0f, 10f, 7.5f, 2.5f, 12.5f, 6.0f, 8.0f, 4.0f, 15.0f, 3.0f, 7.0f, 9f};
	private static float[] ENEMY_TURN_FORCES = new float[] {5.0f, 2.5f, 7.5f, 3.0f, 6.0f};
	private static int[] ENEMY_LOOK_RANGES = new int[] {4,6,10,8,3,11,7,14,9,5,13,12,15};
	private static int[] ENEMY_ROAM_MINS = new int[] {3, 2, 5, 8, 6, 1, 7, 4, 9};
	private static int[] ENEMY_ROAM_MAXS = new int[] {6, 4, 7, 8, 9, 5, 8, 9, 10};
	public static int[] CLAZZ_A_EQUIVALENT_MODEL = new int[] {2,4,8,13,20,30,42,58,   1,14,6,10,15,8};
	private static float[] SPAWN_MIN_FREQUENCY = new float[] {2.0f, 2.0f};
	private static float[] SPAWN_MAX_FREQUENCY = new float[] {4.0f, 6.0f};
	private static int[] SPAWN_MIN_LIVING = new int[] {1, 1}; // first value is for BEGINNER Zones
	private static int[] SPAWN_MAX_LIVING = new int[] {2, 3};
	private static int[] SPAWN_MIN_GENERATED = new int[] {1, 3};
	private static int[] SPAWN_MAX_GENERATED = new int[] {2, 4};

//private static int[] START_5ZONE_PER_CLAZZ = new int[] {1,2,5,10,17,26,37,50}; // these values -(1 times 5) give the CLAZZ_A equivalent for each CLAZZ when starting on model 1
//public static int[] CLAZZ_A_EQUIVALENT_MODEL = new int[] {0,5,20,45,80,125,180,245};
	
	public EnemyDistributor(Play play_) {
		play = play_;
		game = play.game;
		
		ResetStats();
	}
	
	public void Distribute() {
		enemiesAll = 0;
		
//		Debug.Log ("Distributing enemies based on zoneID: " + zoneID + ", enemyCoreModelNum: "+ enemyCoreModelNum +", enemyCoreClazz: " + enemyCoreClazz +", enemyCoreModel: " +enemyCoreModel);
		
		int enemyClazzVariety = CalculateEnemyClazzVariety(play.zoneID);
		Debug.Log ("enemyClazzVariety : " + enemyClazzVariety );
		
		if (enemyClazzVariety > 0) {
			float[] enemyClazzProbability = CalculateEnemyClazzProbability(enemyClazzVariety);
			
			float spawnMinFrequency, spawnMaxFrequency;
			int spawnMinLiving, spawnMaxLiving, spawnMinGenerated, spawnMaxGenerated;
			int enemyClazz, enemyModel, enemyEquivalentClazzAModel;
			
			for (enemyClazz=0; enemyClazz < enemyClazzProbability.Length; enemyClazz++) {
				float rand = UnityEngine.Random.value;
				if (rand <= enemyClazzProbability[enemyClazz]) {
					enemyEquivalentClazzAModel = CalculateEnemyEquivalentClazzAModel(play.zoneID, enemyClazz);
					enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[enemyClazz];
					Debug.Log ("Enemy of enemyClazz/enemyModel/equivalent A: " + enemyClazz+" / " +enemyModel + " / " + enemyEquivalentClazzAModel);
					
					if (play.zoneID > Game.BEGINNER_ZONES) {
						spawnMinFrequency = SPAWN_MIN_FREQUENCY[1];
						spawnMaxFrequency = SPAWN_MAX_FREQUENCY[1];
						spawnMinLiving = SPAWN_MIN_LIVING[1] + Mathf.FloorToInt(play.zoneID/8.0f);
						spawnMaxLiving = SPAWN_MAX_LIVING[1] + Mathf.FloorToInt(play.zoneID/8.0f);
						spawnMinGenerated = SPAWN_MIN_GENERATED[1] + Mathf.FloorToInt(play.zoneID/8.0f);
						spawnMaxGenerated = SPAWN_MAX_GENERATED[1] + Mathf.FloorToInt(play.zoneID/8.0f);
					} else {
						spawnMinFrequency = SPAWN_MIN_FREQUENCY[0];
						spawnMaxFrequency = SPAWN_MAX_FREQUENCY[0];
						spawnMinLiving = SPAWN_MIN_LIVING[0];
						spawnMaxLiving = SPAWN_MAX_LIVING[0];
						spawnMinGenerated = SPAWN_MIN_GENERATED[0];
						spawnMaxGenerated = SPAWN_MAX_GENERATED[0];
					}
					CreateSpawn(enemyClazz, enemyModel, enemyEquivalentClazzAModel,
						play.cave.zone.GetRandomRoom().GetRandomVoidGridPosition(),
						UnityEngine.Random.Range(spawnMinFrequency, spawnMaxFrequency),
						UnityEngine.Random.Range(spawnMinLiving, spawnMaxLiving),
						UnityEngine.Random.Range(spawnMinGenerated, spawnMaxGenerated)); // Spawn.INFINITY
			
					enemiesAll += spawnMaxGenerated;
				}
			}
			// BOSS spawn
			enemyClazz = enemyClazzProbability.Length-1;
			enemyEquivalentClazzAModel = play.zoneID;
			enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[enemyClazz];
			CreateSpawn(enemyClazz, enemyModel, enemyEquivalentClazzAModel,
						play.cave.zone.roomList[1].GetRandomExitPosition(),
						1.0f, 1, 1, true);
			enemiesAll++;
			Debug.Log ("Boss of enemyClazz/enemyModel/equivalent A: " + enemyClazz+" / " +enemyModel + " / " + enemyEquivalentClazzAModel);
		}
		
		DistributeOthers();
//			}
//		}
		

	}
	
	private void DistributeOthers() {
		int enemyEquivalentClazzAModel, enemyModel, number;
		if (CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_BUG8] <= play.zoneID) {
//		Debug.Log ("Distributing Bug");
			enemyEquivalentClazzAModel = play.zoneID;
			enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_BUG8];
			// Bug 2-8 per zone
			number = Mathf.FloorToInt(play.zoneID / 10f) + 2;
			CreateSpawn(Enemy.CLAZZ_BUG8, enemyModel, enemyEquivalentClazzAModel,
						play.cave.zone.roomList[1].GetRandomNonExitGridPosition(),
						1.0f, number, number, false, Spawn.DistributionMode.AllOverCave);
			enemiesAll += number;
		}
		if (CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_SNAKE9] <= play.zoneID) {
//		Debug.Log ("Distriuting Snake");
			enemyEquivalentClazzAModel = play.zoneID;
			enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_SNAKE9];
			// Bug 1-4 per zone
			number = Mathf.FloorToInt(play.zoneID / 16f) + 1;
			CreateSpawn(Enemy.CLAZZ_SNAKE9, enemyModel, enemyEquivalentClazzAModel,
						play.cave.zone.roomList[1].GetRandomNonExitGridPosition(),
						1.0f, number, number, false, Spawn.DistributionMode.AllOverCave);
			enemiesAll += number;
		}
		if (CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_MINEBUILDER10] <= play.zoneID) {
//		Debug.Log ("Distriuting Minebuilder");
			enemyEquivalentClazzAModel = play.zoneID;
			enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_MINEBUILDER10];
			// 1-4 per zone
			number = Mathf.FloorToInt(play.zoneID / 16f) + 1;
			CreateSpawn(Enemy.CLAZZ_MINEBUILDER10, enemyModel, enemyEquivalentClazzAModel,
						play.cave.zone.roomList[1].GetRandomNonExitGridPosition(),
						1.0f, number, number, false, Spawn.DistributionMode.AllOverCave);
			enemiesAll += number;
		}
		if (CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_WALLLASER11] <= play.zoneID) {
//		Debug.Log ("Distriuting Walllaser");
			enemyEquivalentClazzAModel = play.zoneID;
			enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_WALLLASER11];
			// 2-10 per zone
			number = Mathf.FloorToInt(play.zoneID / 8f) + 2;
			int keyCellCounter = 0;
			for (int i=0; i<number; i+=3) {
				GridPosition keyPos;
				if (keyCellCounter < 2) {
					keyPos = play.cave.zone.keyCells[keyCellCounter];
				} else {
					keyPos = play.cave.zone.GetRandomRoom().GetRandomNonExitGridPosition();
				}
				CreateSpawn(Enemy.CLAZZ_WALLLASER11, enemyModel, enemyEquivalentClazzAModel,
							keyPos, 1.0f, number, number, false, Spawn.DistributionMode.PlaceOnWall);
				keyCellCounter++;
			}
			enemiesAll += number;
		}
		if (CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_HORNET12] <= play.zoneID) {
			enemyEquivalentClazzAModel = play.zoneID;
			enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_HORNET12];
			number = Mathf.FloorToInt(play.zoneID / 4f) + 1;
			CreateSpawn(Enemy.CLAZZ_HORNET12, enemyModel, enemyEquivalentClazzAModel,
						play.cave.zone.roomList[1].GetRandomNonExitGridPosition(),
						1.0f, number, number, false, Spawn.DistributionMode.AllOverCave);
			enemiesAll += number;
		}
		if (CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_BULB13] <= play.zoneID) {
			enemyEquivalentClazzAModel = play.zoneID;
			enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[Enemy.CLAZZ_BULB13];
			// Bug 1-4 per zone
			number = Mathf.FloorToInt(play.zoneID / 8f) + 1;
			CreateSpawn(Enemy.CLAZZ_BULB13, enemyModel, enemyEquivalentClazzAModel,
						play.cave.zone.roomList[1].GetRandomNonExitGridPosition(),
						1.0f, number, number, false, Spawn.DistributionMode.AllOverCave);
			enemiesAll += number;
		}
	}
	
	public void RemoveAll() {
		foreach (GameObject gO in GameObject.FindGameObjectsWithTag(Enemy.TAG)) {
			GameObject.Destroy(gO);
		}
		foreach (GameObject gO in GameObject.FindGameObjectsWithTag(Spawn.TAG)) {
			GameObject.Destroy(gO);
		}
		ResetStats();
	}
	
	public Enemy CreateEnemy(int clazz) {
		Enemy e;
		if (clazz == Enemy.CLAZZ_A0) {
			e = (Enemy)(GameObject.Instantiate(game.spiderPrefab) as GameObject).GetComponent<Spider>();
		} else if (clazz == Enemy.CLAZZ_B1) {
			e = (Enemy)(GameObject.Instantiate(game.batPrefab) as GameObject).GetComponent<Bat>();
		} else if (clazz == Enemy.CLAZZ_C2) {
			e = (Enemy)(GameObject.Instantiate(game.gazellePrefab) as GameObject).GetComponent<Gazelle>();
		} else if (clazz == Enemy.CLAZZ_D3) {
			e = (Enemy)(GameObject.Instantiate(game.wombatPrefab) as GameObject).GetComponent<Wombat>();
		} else if (clazz == Enemy.CLAZZ_E4) {
			e = (Enemy)(GameObject.Instantiate(game.mantaPrefab) as GameObject).GetComponent<Manta>();
		} else if (clazz == Enemy.CLAZZ_F5) {
			e = (Enemy)(GameObject.Instantiate(game.pikePrefab) as GameObject).GetComponent<Pike>();
		} else if (clazz == Enemy.CLAZZ_G6) {
			e = (Enemy)(GameObject.Instantiate(game.bullPrefab) as GameObject).GetComponent<Bull>();
		} else if (clazz == Enemy.CLAZZ_H7) {
			e = (Enemy)(GameObject.Instantiate(game.rhinoPrefab) as GameObject).GetComponent<Rhino>();
		} else if (clazz == Enemy.CLAZZ_BUG8) {
			e = (Enemy)(GameObject.Instantiate(game.bugPrefab) as GameObject).GetComponent<Bug>();
		} else if (clazz == Enemy.CLAZZ_SNAKE9) {
			e = (Enemy)(GameObject.Instantiate(game.snakePrefab) as GameObject).GetComponent<Snake>();
		} else if (clazz == Enemy.CLAZZ_MINEBUILDER10) {
			e = (Enemy)(GameObject.Instantiate(game.mineBuilderPrefab) as GameObject).GetComponent<MineBuilder>();
		} else if (clazz == Enemy.CLAZZ_WALLLASER11) {
			e = (Enemy)(GameObject.Instantiate(game.wallLaserPrefab) as GameObject).GetComponent<WallLaser>();
		} else if (clazz == Enemy.CLAZZ_HORNET12) {
			e = (Enemy)(GameObject.Instantiate(game.hornetPrefab) as GameObject).GetComponent<Hornet>();
		} else if (clazz == Enemy.CLAZZ_BULB13) {
			e = (Enemy)(GameObject.Instantiate(game.bulbPrefab) as GameObject).GetComponent<LightBulb>();
		} else {
			e = (Enemy)(GameObject.Instantiate(game.bullPrefab) as GameObject).GetComponent<Bull>();
		}
		return e;
	}
	
	public Spawn CreateSpawn(int enemyClazz, int enemyModel, int enemyEquivalentClazzAModel, GridPosition gridPos,
				float frequency = 15.0f, int maxLiving = 3, int maxGenerated = Spawn.INFINITY, bool isBoss = false,
				Spawn.DistributionMode distributionMode = Spawn.DistributionMode.RandomInRoom) {
		GameObject p = GameObject.Instantiate(game.spawnPrefab) as GameObject;
		Spawn spawn = p.GetComponentInChildren<Spawn>();
		spawn.Initialize(this, play, gridPos, enemyClazz, enemyModel, enemyEquivalentClazzAModel,
			frequency, maxLiving, maxGenerated, isBoss, distributionMode);
		spawn.transform.position = gridPos.GetVector3() * RoomMesh.MESH_SCALE;
		play.cave.zone.GetRoom(gridPos).SetCellToSpawn(gridPos.cellPosition);
		return spawn;
	}
	
	public void LoseHealth(Enemy e, int loss) {
		enemiesHealth -= loss;
		if (e.isActive) {
			enemiesHealthActive -= loss;	
			enemiesHealthActiveAvg = enemiesHealthActive / enemiesActive;
			// TODO enemiesActive can somehow be zero here even if e.active is true DivideByZeroException
		}
	}
	
	public void ActivateEnemy(Enemy e) {
		e.isActive = true;
		enemiesActive++;
		enemiesFirepowerPerSecondActive += e.firepowerPerSecond;
		enemiesFirepowerPerSecondAvgActive = enemiesFirepowerPerSecondActive / enemiesActive;
		enemiesHealthActive += e.health;
		enemiesHealthActiveAvg = enemiesHealthActive / enemiesActive;
	}

	public void DeactivateEnemy(Enemy e) {
		e.isActive = false;
		enemiesActive--;
		enemiesFirepowerPerSecondActive -= e.firepowerPerSecond;
		enemiesHealthActive -= e.health;
		if (enemiesActive > 0) {
			enemiesFirepowerPerSecondAvgActive = enemiesFirepowerPerSecondActive / enemiesActive;
			enemiesHealthActiveAvg = enemiesHealthActive / enemiesActive;
		} else {
			enemiesFirepowerPerSecondAvgActive = 0;
			enemiesHealthActiveAvg = 0;
		}
	}
	
	public void RemoveEnemy(Enemy e) {
		enemiesLiving--;
		enemiesAll--;
		play.RemoveEnemy(e);
		if (e.isActive) {
			DeactivateEnemy(e);
		}
		enemiesFirepowerPerSecond -= e.firepowerPerSecond;
		enemiesFirepowerPerSecondAvg = enemiesFirepowerPerSecond / enemiesLiving;
	}

	public Enemy CreateEnemy(Spawn spawn, int clazz, int model) {
		return CreateEnemy(spawn, clazz, model, model + CLAZZ_A_EQUIVALENT_MODEL[clazz]);
	}

	public Enemy CreateEnemy(Spawn spawn, int clazz, int model, int enemyEquivalentClazzAModel) {
		Enemy enemy = CreateEnemy(clazz);
		enemy.Initialize(play, spawn, clazz, model, enemyEquivalentClazzAModel,
				CalculateEnemyHealth(clazz, enemyEquivalentClazzAModel),
				CalculateEnemySize(clazz, model),
				CalculateEnemyAggressiveness(clazz, model),
				CalculateEnemyMovementForce(clazz, model),
				CalculateEnemyTurnForce(clazz, model),
				CalculateEnemyLookRange(clazz, model),
			    CalculateEnemyRoamMin(clazz, model),
				CalculateEnemyRoamMax(clazz, model));
		
		enemiesFirepowerPerSecond += enemy.firepowerPerSecond;
		enemiesFirepowerPerSecondAvg = enemiesFirepowerPerSecond / enemiesLiving;
		enemiesHealth += enemy.health;
		enemiesLiving++;
		return enemy;
	}
	
	// Super Formula stuff
	
	private int CalculateEnemyClazzVariety(int zoneID) {
		int variety = 0;
		for (int i=0; i<8; i++) {
			if (CLAZZ_A_EQUIVALENT_MODEL[i] <= zoneID) {
				variety++;
			}
		}
		return variety;
		//return Mathf.Min(Mathf.CeilToInt( Mathf.Sqrt(zone5ID) ), Enemy.DISTRIBUTION_CLAZZ_MAX); // 1-8
	}
	
	private float[] CalculateEnemyClazzProbability(int enemyClazzVariety) {
		float[] result = new float[1];
		switch (enemyClazzVariety) {
			case 1: result = new float[] { 1.0f }; break;
			case 2: result = new float[] { 0.4f, 1.0f }; break;
			case 3: result = new float[] { 0.2f, 0.5f, 1.0f }; break;
			case 4: result = new float[] { 0.15f, 0.3f, 0.55f, 1.0f }; break;
			case 5: result = new float[] { 0.1f, 0.22f, 0.34f, 0.57f, 1.0f }; break;
			case 6: result = new float[] { 0.075f, 0.17f, 0.29f, 0.43f, 0.6f, 1.0f }; break;
			case 7: result = new float[] { 0.065f, 0.13f, 0.2f, 0.3f, 0.45f, 0.6f, 1.0f }; break;
			case 8: result = new float[] { 0.06f, 0.12f, 0.2f, 0.29f, 0.39f, 0.50f, 0.65f, 1.0f }; break;
//			case 9: result = new float[] { 0.05f, 0.1f, 0.17f, 0.24f, 0.33f, 0.43f, 0.54f, 0.7f, 1.0f }; break;
//			case 10: result = new float[] { 0.03f, 0.1f, 0.16f, 0.23f, 0.31f, 0.4f, 0.5f, 0.55f, 0.75f, 1.0f }; break;
		}
		return result;
	}

/*	private int CalculateEnemyClazzDelta(int enemyClazzVariety, int coreClazz, int clazzProbabilityIndex) {
		int classDelta = enemyClazzVariety - clazzProbabilityIndex;
		// divide by 2 to adjust for clazz above or below coreClazz
		int clazzDeltaRelative = Mathf.FloorToInt(classDelta / 2.0f);
		if (clazzProbabilityIndex % 2 == 0) { // even
			clazzDeltaRelative *= -1;
		}
		return coreClazz + clazzDeltaRelative;
	}*/

	private int CalculateEnemyEquivalentClazzAModel(int zoneID, int clazz) {
//		int model = CLAZZ_A_EQUIVALENT_MODEL[clazz] + zoneID - CLAZZ_A_EQUIVALENT_MODEL[clazz];
		int model = zoneID;
		
		float[] probabilities = new float[] { 0.1f, 0.3f, 0.6f, 1.0f };
		float rand = UnityEngine.Random.value;
		int modelDelta = 0;
		for (int i=0; i<probabilities.Length; i++) {
			if (rand <= probabilities[i]) {
				modelDelta = (probabilities.Length-1) - i;
				if (UnityEngine.Random.Range(0,2) == 0) { // 50:50
					modelDelta *= -1;
				}
				i = probabilities.Length;
			}
		}
		
		return Mathf.Clamp(model + modelDelta, CLAZZ_A_EQUIVALENT_MODEL[clazz], Enemy.MODEL_MAX);
	}
	
	private int CalculateEnemyHealth(int clazz, int model) {
		int lookUp = (clazz + model) % 9;
		float baseHealth = Weapon.PRIMARY_DAMAGE[clazz] * ENEMY_HEALTH_MULTIPLICATOR[lookUp];
//		Debug.Log (clazz + " " +model + " " + baseHealth + " " + lookUp +" " + ENEMY_HEALTH_MODIFIER[lookUp]);
		return (int)(baseHealth + baseHealth * ENEMY_HEALTH_MODIFIER[lookUp]);
		
	//	return model * Game.HEALTH_MODIFIER;
	}

	private int CalculateEnemyShield(int clazz, int model) {
		return 0;
/*		if (clazz == Enemy.CLAZZ_BUG8 || clazz == Enemy.CLAZZ_SNAKE9 || clazz == Enemy.CLAZZ_WALLLASER11 || clazz == Enemy.CLAZZ_HORNET12) {
			return 0;
		} else {
			return Mathf.FloorToInt((model * Game.HEALTH_MODIFIER) / 2f);
		}*/
	}
	
	private float CalculateEnemySize(int clazz, int model) {
		if (clazz == Enemy.CLAZZ_BUG8 || clazz == Enemy.CLAZZ_SNAKE9 || clazz == Enemy.CLAZZ_MINEBUILDER10
				|| clazz == Enemy.CLAZZ_WALLLASER11 || clazz == Enemy.CLAZZ_HORNET12 || clazz == Enemy.CLAZZ_D3 || clazz == Enemy.CLAZZ_BULB13) {
			return ENEMY_SIZES[0];
		} else {
			return ENEMY_SIZES[(clazz + model) % 10];
		}
	}

	private float CalculateEnemyAggressiveness(int clazz, int model) {
		if (clazz == Enemy.CLAZZ_BUG8 || clazz == Enemy.CLAZZ_SNAKE9 || clazz == Enemy.CLAZZ_MINEBUILDER10
			|| clazz == Enemy.CLAZZ_WALLLASER11 || clazz == Enemy.CLAZZ_HORNET12 || clazz == Enemy.CLAZZ_BULB13) {
			return Enemy.AGGRESSIVENESS_OFF;
		} else {
			return ENEMY_AGGRESSIVENESSES[(clazz + model) % 11];
		}
	}
	
	private float CalculateEnemyMovementForce(int clazz, int model) {
		return ENEMY_MOVEMENT_FORCES[(clazz + model) % 12];
	}

	private float CalculateEnemyTurnForce(int clazz, int model) {
		return ENEMY_TURN_FORCES[(clazz + model) % 5];
	}

	private int CalculateEnemyLookRange(int clazz, int model) {
		return ENEMY_LOOK_RANGES[(clazz + model) % 13];
	}
	
/*	private int CalculateEnemyChaseRange(int clazz, int model) {
		return ENEMY_CHASE_RANGES[(clazz + (model-1)) % 6];
	}*/

	private int CalculateEnemyRoamMin(int clazz, int model) {
		return ENEMY_ROAM_MINS[(clazz + model) % 9];
	}

	private int CalculateEnemyRoamMax(int clazz, int model) {
		return ENEMY_ROAM_MAXS[(clazz + model) % 9];
	}
	
	private void ResetStats() {
		enemiesLiving = 0;
		enemiesActive = 0;
		enemiesHealth = 0;
		enemiesHealthActive = 0;
		enemiesHealthActiveAvg = 0;
		enemiesFirepowerPerSecond = 0;
		enemiesFirepowerPerSecondActive = 0;
		enemiesFirepowerPerSecondAvg = 0;
		enemiesFirepowerPerSecondAvgActive = 0;
		enemiesHitRatio = 0;
	}	
}

