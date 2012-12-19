using System;
using UnityEngine;
using System.Collections;

public class EnemyDistributor {
	private Play play;
	private Game game;
	
	private Room room;
	private int roomSize;
	private ArrayList emptyCells;
	private RaycastHit hit;
		
	private static float MAX_RAYCAST_DISTANCE = 100.0f;

	public EnemyDistributor(Game g, Play p, Room r, int rS) {
		game = g;
		play = p;
		room = r;
		roomSize = rS;
		
		// build array list of all empty cells
		emptyCells = new ArrayList();
		int emptyCellIx = 0;
		for (int x=0; x<roomSize; x++) {
			for (int y=0; y<roomSize; y++) {
				for (int z=0; z<roomSize; z++) {
					if (room.cubeDensity[x,y,z] == CaveDigger.DENSITY_EMPTY) {
						emptyCells.Add(new IntTriple(x, y, z));
					}
				}
			}
		}
	}
	
	public void Distribute() {
				
		IntTriple result = (IntTriple)emptyCells[UnityEngine.Random.Range(0,emptyCells.Count)];
//		Debug.Log (result.x +" " + result.y +" " + result.z);
		Vector3 pos = new Vector3(result.x * Room.MESH_SCALE, result.y * Room.MESH_SCALE, result.z * Room.MESH_SCALE);	
		Vector3 rayPath = RandomVector();
		
		if (Physics.Raycast(pos, rayPath, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {			
			WallGun wallGun = CreateWallGun();
			PlaceOnWall(wallGun.gameObject, hit);
		}
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
	
	public void PlaceOnWall(GameObject gO, RaycastHit h) {
//		Vector3 centeredPos = h.collider.transform.TransformPoint(h.barycentricCoordinate);
//		gO.transform.position = cubePosition + centeredPos;		
		Vector3 v1 = room.mesh.vertices[room.mesh.triangles[h.triangleIndex * 3 + 0]];
		Vector3 v2 = room.mesh.vertices[room.mesh.triangles[h.triangleIndex * 3 + 1]];
		Vector3 v3 = room.mesh.vertices[room.mesh.triangles[h.triangleIndex * 3 + 2]];
		gO.transform.position = ((v1 + v2 + v3)/3) * Room.MESH_SCALE;
		gO.transform.forward = h.normal;
	}
	
	public static Vector3 RandomVector() {
		return new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * ((UnityEngine.Random.Range(0,2) == 0) ? 1 : -1);
	}
	
	public struct IntTriple {
		public int x,y,z;
		
		public IntTriple(int a, int b, int c) {
			x=a;
			y=b;
			z=c;
		}
		
		public IntTriple(Vector3 v) {
			x=Mathf.RoundToInt(v.x);
			y=Mathf.RoundToInt(v.y);
			z=Mathf.RoundToInt(v.z);
		}
	}
}

