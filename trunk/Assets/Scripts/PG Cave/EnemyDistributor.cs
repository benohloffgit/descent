using System;
using UnityEngine;
using System.Collections;

public class EnemyDistributor {
	private Play play;
	private Game game;
	
	private ArrayList emptyCells;
	private RaycastHit hit;
		
	private static float MAX_RAYCAST_DISTANCE = 100.0f;

	public EnemyDistributor(Game g, Play p) {
		game = g;
		play = p;
		
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
		IntTriple result = (IntTriple)emptyCells[UnityEngine.Random.Range(0,emptyCells.Count)];
//		Debug.Log (result.x +" " + result.y +" " + result.z);
		Vector3 pos = new Vector3(result.x * RoomMesh.MESH_SCALE, result.y * RoomMesh.MESH_SCALE, result.z * RoomMesh.MESH_SCALE);	
		Vector3 rayPath = RandomVector();
		
		if (Physics.Raycast(pos, rayPath, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {			
			WallGun wallGun = CreateWallGun();
			PlaceOnWall(wallGun.gameObject, hit);
		}
	}

	public LightBulb CreateLightBulb() {
		GameObject lB = GameObject.Instantiate(play.lightBulbPrefab) as GameObject;
		LightBulb lightBulb = lB.GetComponent<LightBulb>();
		lightBulb.Initialize(game, play);
		return lightBulb;
	}
	
	public MineBuilder CreateMineBuilder() {
		GameObject mB = GameObject.Instantiate(play.mineBuilderPrefab) as GameObject;
		MineBuilder mineBuilder = mB.GetComponent<MineBuilder>();
		mineBuilder.Initialize(game, play);
		return mineBuilder;
	}
	
	public MineTouch CreateMineTouch() {
		GameObject mT = GameObject.Instantiate(play.mineTouchPrefab) as GameObject;
		MineTouch mineTouch = mT.GetComponent<MineTouch>();
		mineTouch.Initialize(game, play);
		return mineTouch;
	}

	public WallGun CreateWallGun() {
		GameObject wG = GameObject.Instantiate(play.wallGunPrefab) as GameObject;
		WallGun wallGun = wG.GetComponent<WallGun>();
		wallGun.Initialize(game, play);
		return wallGun;
	}

	public WallLaser CreateWallLaser() {
		GameObject wL = GameObject.Instantiate(play.wallLaserPrefab) as GameObject;
		WallLaser wallLaser = wL.GetComponent<WallLaser>();
		wallLaser.Initialize(game, play);
		return wallLaser;
	}

	public Pyramid CreatePyramid() {
		GameObject p = GameObject.Instantiate(play.pyramidPrefab) as GameObject;
		Pyramid pyramid = p.GetComponent<Pyramid>();
		pyramid.Initialize(game, play);
		return pyramid;
	}

	public Spike CreateSpike() {
		GameObject p = GameObject.Instantiate(play.spikePrefab) as GameObject;
		Spike spike = p.GetComponent<Spike>();
		spike.Initialize(game, play);
		return spike;
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

