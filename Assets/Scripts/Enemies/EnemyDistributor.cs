using System;
using UnityEngine;
using System.Collections;

public class EnemyDistributor {
	private Play play;
	private Game game;
	
	private RaycastHit hit;
	
	public int enemiesLiving;
	public int enemiesActive;
	public int enemiesHealth;
	public int enemiesHealthActive;
	public int enemiesHealthActiveAvg;
	public float enemiesFirepowerPerSecond;
	public float enemiesFirepowerPerSecondAvg;
	public float enemiesFirepowerPerSecondActive;
	public float enemiesFirepowerPerSecondAvgActive;
	public float enemiesHitRatio;
	
	private static float MAX_RAYCAST_DISTANCE = 100.0f;
	
	private static float[] ENEMY_SIZES = new float[] {1.0f, 0.5f, 1.5f, 0.7f, 1.3f, 0.3f, 0.7f, 1.7f, 2.0f, 1.0f};
	/*unused*/private static float[] ENEMY_AGGRESSIVENESSES = new float[] {0.05f, 0.2f, 0.5f, 0.1f, 0.7f, 0.3f, 0.2f, 0.6f, 0.4f, 0.1f, 0.3f};
	private static float[] ENEMY_MOVEMENT_FORCES = new float[] {5.0f, 10f, 7.5f, 2.5f, 12.5f, 6.0f, 8.0f, 4.0f, 15.0f, 3.0f, 7.0f, 9f};
	private static float[] ENEMY_TURN_FORCES = new float[] {5.0f, 2.5f, 7.5f, 3.0f, 6.0f};
	private static int[] ENEMY_LOOK_RANGES = new int[] {4,6,10,8,3,11,7,14,9,5,13,12,15};
//	private static int[] ENEMY_CHASE_RANGES = new int[] {4,6,12,8,2,10};
	private static int[] ENEMY_ROAM_MINS = new int[] {3, 2, 5, 8, 6, 1, 7, 4, 9};
	private static int[] ENEMY_ROAM_MAXS = new int[] {6, 4, 7, 8, 9, 5, 8, 9, 10};
	private static int[] START_5ZONE_PER_CLAZZ = new int[] {1,2,5,10,17,26,37,50}; // these values -(1 times 5) give the CLAZZ_A equivalent for each CLAZZ when starting on model 1
	public static int[] CLAZZ_A_EQUIVALENT_MODEL = new int[] {0,5,20,45,80,125,180,245};
	
	public EnemyDistributor(Play play_) {
		play = play_;
		game = play.game;
		
		ResetStats();
	}
	
	public void Distribute(int zoneID) {
		int zone5 = Zone.GetZone5StepID(zoneID);
//		int enemyCoreModelNum = zoneID;
//		int enemyCoreClazz = Mathf.FloorToInt(enemyCoreModelNum / Enemy.CLAZZ_STEP);
//		int enemyCoreModel = enemyCoreModelNum % Enemy.CLAZZ_STEP;
		
//		Debug.Log ("Distributing enemies based on zoneID: " + zoneID + ", enemyCoreModelNum: "+ enemyCoreModelNum +", enemyCoreClazz: " + enemyCoreClazz +", enemyCoreModel: " +enemyCoreModel);
		
		int enemyClazzVariety = CalculateEnemyClazzVariety(zone5);
		Debug.Log ("enemyClazzVariety : " + enemyClazzVariety );
		
		float[] enemyClazzProbability = CalculateEnemyClazzProbability(enemyClazzVariety);
				
		foreach (Room r in play.cave.zone.roomList) {
			if (r.id > -1) {  // ----  > 0 all rooms except entry
				float rand = UnityEngine.Random.value;
				for (int enemyClazz=0; enemyClazz < enemyClazzProbability.Length; enemyClazz++) {
					if (rand <= enemyClazzProbability[enemyClazz]) {
					//	int enemyClazzDelta = CalculateEnemyClazzDelta(enemyClazzVariety, enemyCoreClazz, i);
						//Debug.Log ("enemyClazzDelta: " + enemyClazzDelta);
					//	int enemyModel = CalculateEnemyModel(enemyCoreClazz, enemyCoreModel, enemyClazzDelta);
						int enemyEquivalentClazzAModel = CalculateEnemyEquivalentClazzAModel(zoneID, enemyClazz);
						//Debug.Log ("enemyModel: " + enemyModel);
					//	int enemyClazz = Mathf.Clamp(enemyCoreClazz + enemyClazzDelta, Enemy.CLAZZ_MIN, Enemy.CLAZZ_MAX);
						int enemyModel = enemyEquivalentClazzAModel - CLAZZ_A_EQUIVALENT_MODEL[enemyClazz];
						Debug.Log ("enemyClazz/enemyModel (equivalent A): " + enemyClazz+"/" + " " +enemyModel);
						
						CreateSpawn(enemyClazz, enemyModel, enemyEquivalentClazzAModel, r.GetRandomNonSpawnNonExitGridPosition(),
							UnityEngine.Random.Range(5.0f, 15.0f), UnityEngine.Random.Range(1, 5), UnityEngine.Random.Range(3, 7)); // Spawn.INFINITY
					}
				}
			}
		}
		
/*		IntTriple result = (IntTriple)emptyCells[UnityEngine.Random.Range(0,emptyCells.Count)];
//		Debug.Log (result.x +" " + result.y +" " + result.z);
		Vector3 pos = new Vector3(result.x * RoomMesh.MESH_SCALE, result.y * RoomMesh.MESH_SCALE, result.z * RoomMesh.MESH_SCALE);	
		Vector3 rayPath = RandomVector();
		
		if (Physics.Raycast(pos, rayPath, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {			
			WallGun wallGun = CreateWallGun();
			PlaceOnWall(wallGun.gameObject, hit);
		}*/
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

	public LightBulb CreateLightBulb() {
		GameObject lB = GameObject.Instantiate(game.lightBulbPrefab) as GameObject;
		LightBulb lightBulb = lB.GetComponent<LightBulb>();
		lightBulb.Initialize(game, play);
		return lightBulb;
	}
	
	public MineBuilder CreateMineBuilder() {
		GameObject mB = GameObject.Instantiate(game.mineBuilderPrefab) as GameObject;
		MineBuilder mineBuilder = mB.GetComponent<MineBuilder>();
		return mineBuilder;
	}
	
	public MineTouch CreateMineTouch() {
		GameObject mT = GameObject.Instantiate(game.mineTouchPrefab) as GameObject;
		MineTouch mineTouch = mT.GetComponent<MineTouch>();
		mineTouch.Initialize(game, play);
		return mineTouch;
	}

	public WallGun CreateWallGun() {
		GameObject wG = GameObject.Instantiate(game.wallGunPrefab) as GameObject;
		WallGun wallGun = wG.GetComponent<WallGun>();
		wallGun.Initialize(game, play);
		return wallGun;
	}

	public WallLaser CreateWallLaser() {
		GameObject wL = GameObject.Instantiate(game.wallLaserPrefab) as GameObject;
		WallLaser wallLaser = wL.GetComponent<WallLaser>();
		wallLaser.Initialize(game, play);
		return wallLaser;
	}

	public Pyramid CreatePyramid() {
		GameObject p = GameObject.Instantiate(game.pyramidPrefab) as GameObject;
		Pyramid pyramid = p.GetComponent<Pyramid>();
		return pyramid;
	}

	public Spike CreateSpike() {
		GameObject p = GameObject.Instantiate(game.spikePrefab) as GameObject;
		Spike spike = p.GetComponent<Spike>();
		return spike;
	}

	public Bull CreateBull() {
		GameObject p = GameObject.Instantiate(game.bullPrefab) as GameObject;
		Bull bull = p.GetComponentInChildren<Bull>();
		return bull;
	}

	public Mana CreateMana() {
		GameObject p = GameObject.Instantiate(game.manaPrefab) as GameObject;
		Mana mana = p.GetComponentInChildren<Mana>();
		return mana;
	}

	public Spawn CreateSpawn(int enemyClazz, int enemyModel, int enemyEquivalentClazzAModel, GridPosition gridPos,
				float frequency = 15.0f, int maxLiving = 3, int maxGenerated = Spawn.INFINITY) {
		GameObject p = GameObject.Instantiate(game.spawnPrefab) as GameObject;
		Spawn spawn = p.GetComponentInChildren<Spawn>();
		spawn.Initialize(this, play, gridPos, enemyClazz, enemyModel, enemyEquivalentClazzAModel, frequency, maxLiving, maxGenerated);
		spawn.transform.position = gridPos.GetVector3() * RoomMesh.MESH_SCALE;
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
		enemiesActive++;
		enemiesFirepowerPerSecondActive += e.firepowerPerSecond;
		enemiesFirepowerPerSecondAvgActive = enemiesFirepowerPerSecondActive / enemiesActive;
		enemiesHealthActive += e.health;
		enemiesHealthActiveAvg = enemiesHealthActive / enemiesActive;
	}

	public void DeactivateEnemy(Enemy e) {
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
		Enemy enemy;
		if (clazz == Enemy.CLAZZ_A0) {
			enemy = (Enemy)CreateBull();
		} else if (clazz == Enemy.CLAZZ_B1) {
			enemy = (Enemy)CreateSpike();
		} else {
			enemy = (Enemy)CreateBull();
		}
		enemy.Initialize(play, spawn, clazz, model, enemyEquivalentClazzAModel,
				CalculateEnemyHealth(clazz, enemyEquivalentClazzAModel),
				CalculateEnemyShield(clazz, enemyEquivalentClazzAModel),
				CalculateEnemySize(clazz, model),
				CalculateEnemyAggressiveness(clazz, model),
				CalculateEnemyMovementForce(clazz, model),
				CalculateEnemyTurnForce(clazz, model),
				CalculateEnemyLookRange(clazz, model),
				//CalculateEnemyChaseRange(clazz, model),
			    CalculateEnemyRoamMin(clazz, model),
				CalculateEnemyRoamMax(clazz, model));
		
		enemiesFirepowerPerSecond += enemy.firepowerPerSecond;
		enemiesFirepowerPerSecondAvg = enemiesFirepowerPerSecond / enemiesLiving;
		enemiesHealth += enemy.health;
		enemiesLiving++;
		return enemy;
	}
	
	public void PlaceOnWall(GameObject gO, RaycastHit h) {
//		Vector3 centeredPos = h.collider.transform.TransformPoint(h.barycentricCoordinate);
//		gO.transform.position = cubePosition + centeredPos;	
		Mesh mesh = play.GetRoomOfShip().roomMesh.mesh;
		Vector3 v1 = mesh.vertices[mesh.triangles[h.triangleIndex * 3 + 0]];
		Vector3 v2 = mesh.vertices[mesh.triangles[h.triangleIndex * 3 + 1]];
		Vector3 v3 = mesh.vertices[mesh.triangles[h.triangleIndex * 3 + 2]];
		gO.transform.position = ((v1 + v2 + v3)/3) * RoomMesh.MESH_SCALE;
		gO.transform.forward = h.normal;
	}
	
	public static Vector3 RandomVector() {
		return new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * ((UnityEngine.Random.Range(0,2) == 0) ? 1 : -1);
	}
		
	// Super Formula stuff
	
	private int CalculateEnemyClazzVariety(int zone5ID) {
		return (int) Mathf.Min(Mathf.Ceil( Mathf.Sqrt(zone5ID+1) ), Enemy.DISTRIBUTION_CLAZZ_MAX); // 1-8
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
		int model = CLAZZ_A_EQUIVALENT_MODEL[clazz] + zoneID- CLAZZ_A_EQUIVALENT_MODEL[clazz];
		
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
		//modelDelta += enemyClazzDelta * -3;
		
		return Mathf.Clamp(model + modelDelta, CLAZZ_A_EQUIVALENT_MODEL[clazz], CLAZZ_A_EQUIVALENT_MODEL[clazz] + Enemy.MODEL_MAX);
	}

/*	private int CalculateEnemyModel(int coreClazz, int coreModel, int enemyClazzDelta) {
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
		modelDelta += enemyClazzDelta * -3;
		
		return Mathf.Clamp(coreModel + modelDelta, Enemy.MODEL_MIN, Enemy.MODEL_MAX);
		
	}*/
	
	private int CalculateEnemyHealth(int clazz, int model) {
		return model * 5 + clazz * 25;
	}

	private int CalculateEnemyShield(int clazz, int model) {
		return Mathf.FloorToInt((model * 5 + clazz * 25) / 2f);
	}
	
	private float CalculateEnemySize(int clazz, int model) {
		return ENEMY_SIZES[(clazz + (model-1)) % 10];
	}

	private float CalculateEnemyAggressiveness(int clazz, int model) {
		return ENEMY_AGGRESSIVENESSES[(clazz + (model-1)) % 11];
	}
	
	private float CalculateEnemyMovementForce(int clazz, int model) {
		return ENEMY_MOVEMENT_FORCES[(clazz + (model-1)) % 12];
	}

	private float CalculateEnemyTurnForce(int clazz, int model) {
		return ENEMY_TURN_FORCES[(clazz + (model-1)) % 5];
	}

	private int CalculateEnemyLookRange(int clazz, int model) {
		return ENEMY_LOOK_RANGES[(clazz + (model-1)) % 13];
	}
	
/*	private int CalculateEnemyChaseRange(int clazz, int model) {
		return ENEMY_CHASE_RANGES[(clazz + (model-1)) % 6];
	}*/

	private int CalculateEnemyRoamMin(int clazz, int model) {
		return ENEMY_ROAM_MINS[(clazz + (model-1)) % 9];
	}

	private int CalculateEnemyRoamMax(int clazz, int model) {
		return ENEMY_ROAM_MAXS[(clazz + (model-1)) % 9];
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

