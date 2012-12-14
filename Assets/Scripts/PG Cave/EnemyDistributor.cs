using System;
using UnityEngine;
using System.Collections;

public class EnemyDistributor {
	private Play play;
	private Game game;
	
	private MarchingCubes marchingCubes;
	private int roomSize;
	private ArrayList emptyCells;
	private RaycastHit hit;
		
	private static float MAX_RAYCAST_DISTANCE = 100.0f;

	public EnemyDistributor(Game g, Play p, MarchingCubes mC, int rS) {
		game = g;
		play = p;
		marchingCubes = mC;
		roomSize = rS;
		
		// build array list of all empty cells
		emptyCells = new ArrayList();
		int emptyCellIx = 0;
		for (int x=0; x<roomSize; x++) {
			for (int y=0; y<roomSize; y++) {
				for (int z=0; z<roomSize; z++) {
					if (marchingCubes.cubeDensity[x,y,z] == CaveDigger.DENSITY_EMPTY) {
						emptyCells.Add(new IntTriple(x, y, z));
					}
				}
			}
		}
	}
	
	public void Distribute() {
		
		IntTriple result = (IntTriple)emptyCells[UnityEngine.Random.Range(0,emptyCells.Count)];
//		Debug.Log (result.x +" " + result.y +" " + result.z);
		Vector3 pos = new Vector3(result.x * MarchingCubes.MESH_SCALE, result.y * MarchingCubes.MESH_SCALE, result.z * MarchingCubes.MESH_SCALE);
		GameObject wallGun = GameObject.Instantiate(play.wallGunPrefab, pos, Quaternion.identity) as GameObject;
		WallGun wG = wallGun.GetComponent<WallGun>();
		wG.Initialize(game, play);
		
		Vector3 rayPath = RandomVector();
		//Debug.Log (rayPath);
		if (Physics.Raycast(pos, rayPath, out hit, MAX_RAYCAST_DISTANCE, 1 << Game.LAYER_CAVE)) {
			//Vector3 v1 = marchingCubes.mesh.vertices[marchingCubes.mesh.triangles[hit.triangleIndex * 3 + 0]];
			//Vector3 v2 = marchingCubes.mesh.vertices[marchingCubes.mesh.triangles[hit.triangleIndex * 3 + 1]];
			//Vector3 v3 = marchingCubes.mesh.vertices[marchingCubes.mesh.triangles[hit.triangleIndex * 3 + 2]];
//			wallGun.transform.position = ((v1 + v2 + v3)/3) * MarchingCubes.MESH_SCALE;
			
			Vector3 centeredPos = hit.collider.transform.TransformPoint(hit.barycentricCoordinate);
			wallGun.transform.position = pos + centeredPos;
//			Debug.Log (centeredPos + " " + pos);
			
			wallGun.transform.forward = hit.normal;
		}
	}
	
	private Vector3 RandomVector() {
		return new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * ((UnityEngine.Random.Range(0,2) == 0) ? 1 : -1);
	}
	
	public struct IntTriple {
		public int x,y,z;
		
		public IntTriple(int a, int b, int c) {
			x=a;
			y=b;
			z=c;
		}
	}
}

