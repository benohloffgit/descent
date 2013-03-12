using System;
using UnityEngine;
using System.Collections;

public class EnemyDistributor {
	private Play play;
	private Game game;
	
	private ArrayList emptyCells;
	private RaycastHit hit;
		
	private static float MAX_RAYCAST_DISTANCE = 100.0f;

	public EnemyDistributor(Play play_) {
		play = play_;
		game = play.game;
				
		// build array list of all empty cells
/*		emptyCells = new ArrayList();
		int emptyCellIx = 0;
		for (int x=0; x<room.dimension; x++) {
			for (int y=0; y<room.dimension; y++) {
				for (int z=0; z<room.dimension; z++) {
					if (room.GetCellDensity(x,y,z) == CaveDigger.DENSITY_EMPTY) {
						emptyCells.Add(new IntTriple(x, y, z));
					}
				}
			}
		}*/
	}
	
	public void Distribute() {
		
		foreach (Room r in play.cave.GetCurrentZone().roomList) {
			if (r.id > 0) { // all rooms except entry
				GridPosition empty = GetRandomEmptyGridPosition(r);
				CreateSpawn(Enemy.CLAZZ_A, 5, empty, 15.0f, 3, 10);
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
	
	private GridPosition GetRandomEmptyGridPosition (Room r) {
		GridPosition result = GridPosition.ZERO;
		bool cont = true;
		while (cont) {
			Cell c = r.emptyCells[UnityEngine.Random.Range(0, r.emptyCells.Count)];
			if (!c.isSpawn && !c.isExit) {
				result = new GridPosition(c.pos, r.pos);
				cont = false;
			}
		}
		return result;
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

	public Spawn CreateSpawn(string enemyClazz, int enemyModel, GridPosition gridPos,
				float frequency = 15.0f, int maxLiving = 3, int maxGenerated = Spawn.INFINITY) {
		GameObject p = GameObject.Instantiate(game.spawnPrefab) as GameObject;
		Spawn spawn = p.GetComponentInChildren<Spawn>();
		spawn.Initialize(this, play, gridPos, enemyClazz, enemyModel, frequency, maxLiving, maxGenerated);
		spawn.transform.position = gridPos.GetVector3() * RoomMesh.MESH_SCALE;
		return spawn;
	}
	
	public Enemy CreateEnemy(Spawn spawn, string clazz, int number) {
		Enemy enemy;
		if (clazz == Enemy.CLAZZ_A) {
			enemy = (Enemy)CreateBull();
			switch (number) {
												             //   health shield size    aggr    movF   turnF lookR aimTol roamTol chaseR
				case 1:	enemy.Initialize(play, spawn, clazz, number, 10,	0,	1.0f,	2.5f,	5.0f,	5.0f,	4,	0.5f,	20.0f,	0, new int[] {Weapon.TYPE_GUN}, new int[] {1}); break;
				case 5:	enemy.Initialize(play, spawn, clazz, number, 20,	0,	0.5f,	10.0f,	5.0f,	5.0f,	4,	0.5f,	20.0f,	0, new int[] {Weapon.TYPE_GUN, Weapon.TYPE_GUN}, new int[] {1,2}); break;
				case 11:enemy.Initialize(play, spawn, clazz, number, 20,	0,	0.5f,	5.0f,	10.0f,	5.0f,	4,	0.5f,	20.0f,	0, new int[] {Weapon.TYPE_LASER}, new int[] {1}); break;
				default:break;
			}
		} else if (clazz == Enemy.CLAZZ_B) {
			enemy = (Enemy)CreateSpike();
			switch (number) {
												             //   health shield size    aggr    movF   turnF lookR aimTol roamTol chaseR
				case 1:	enemy.Initialize(play, spawn, clazz, number, 10,	0,	1.0f,	2.5f,	7.5f,	5.0f,	8,	0.5f,	20.0f,	4, new int[] {Weapon.TYPE_GUN}, new int[] {1}); break;
				case 5:	enemy.Initialize(play, spawn, clazz, number, 20,	0,	0.5f,	10.0f,	5.0f,	5.0f,	8,	0.5f,	20.0f,	4, new int[] {Weapon.TYPE_GUN}, new int[] {2}); break;
				case 11:enemy.Initialize(play, spawn, clazz, number, 20,	0,	0.5f,	5.0f,	10.0f,	5.0f,	8,	0.5f,	20.0f,	4, new int[] {Weapon.TYPE_LASER}, new int[] {1}); break;
				default:break;
			}
			
		} else {
			enemy = (Enemy)CreateBull();
		}		
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
		
}

