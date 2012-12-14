using System;
using UnityEngine;
using System.Collections;

public class CaveDigger {
	public int[,,] cubeDensity;
	public int[,,] cubeMinedBy; // 0=mined by nobody, minerst start from 1
	public int[] dimension;
	
	public int[] entry;
	private int entryWall;
	public int[] exit;
	private int exitWall;
	public int digCount;
	
	private Miner[] miners;

//	public enum Dim {x=0, y=1, z=2}
//	public enum Density {filled=0, empty=1}
	public static int DENSITY_FILLED = 0;
	public static int DENSITY_EMPTY = 1;
	public static int DIM_X = 0;
	public static int DIM_Y = 1;
	public static int DIM_Z = 2;
	
	public enum Dir {xMin=0, xMax=1, yMin=2, yMax=3, zMin=4, zMax=5}
	
	public CaveDigger(int x, int y, int z, int[,,] cD) {
		UnityEngine.Random.seed = 123456789;
		
		dimension = new int[] {x, y, z};
		cubeDensity = cD;
		cubeMinedBy = new int[x,y,z];
		miners = new Miner[2];
		
		// determine wall side ( 0:x=0, 1:x=dimX-1, 2:y=0, 3:y=dimY-1, 4:z=0, 5:z=dimZ-1)
		entryWall = CaveDigger.GetRandomNumberFromPool(new int[] {0,1,2,3,4,5});
		entry = DigEntryExit(entryWall);
		cubeDensity[entry[CaveDigger.DIM_X],entry[CaveDigger.DIM_Y],entry[CaveDigger.DIM_Z]] = DENSITY_EMPTY;

		int[] remainingWalls = new int[5];
		int index = 0;
		for (int i=0; i<6; i++) {
			if (i != entryWall) {
				remainingWalls[index] = i;
				index++;
			}
		}
		exitWall = CaveDigger.GetRandomNumberFromPool(remainingWalls);
		exit = DigEntryExit(exitWall);
		cubeDensity[exit[CaveDigger.DIM_X],exit[CaveDigger.DIM_Y],exit[CaveDigger.DIM_Z]] = DENSITY_EMPTY;
		
		DigCave();
	}
	
	private int[] DigEntryExit(int wall) {
		int coordX = UnityEngine.Random.Range(2, dimension[CaveDigger.DIM_X]-3);
		int coordY = UnityEngine.Random.Range(2, dimension[CaveDigger.DIM_Y]-3);
		int coordZ = UnityEngine.Random.Range(2, dimension[CaveDigger.DIM_Z]-3);
		int[] result = new int[3];
		switch (wall) {
			case 0:
				result = new int[] {0,coordY,coordZ};
				break;
			case 1:
				result = new int[] {dimension[CaveDigger.DIM_X]-1,coordY,coordZ};
				break;
			case 2:
				result = new int[] {coordX,0,coordZ};
				break;
			case 3:
				result = new int[] {coordX,dimension[CaveDigger.DIM_Y]-1,coordZ};
				break;
			case 4:
				result = new int[] {coordX,coordY,0};
				break;
			case 5:
				result = new int[] {coordX,coordY,dimension[CaveDigger.DIM_Z]-1};
				break;
		}
		return result;
	}
	
	private void DigCave() {				
		miners[0] = new Miner(entry, 1, this);
		miners[1] = new Miner(exit, 2, this);
		digCount = 0;
		while (miners[0].isActive || miners[1].isActive) {
			miners[0].Mine();
			miners[1].Mine();
		}
		Debug.Log ("Blocks digged: " + digCount);
	}
	
	public static int GetRandomNumberFromPool(int[] pool) {
		return pool[UnityEngine.Random.Range(0, pool.Length-1)];
	}
}

public class Miner {
	public int[] pos;
	public int x;
	public int y;
	public int z;
	public int id; // starting from 1
	public bool isActive = true;
	
	public enum Dim {x=0, y=1, z=2}

	private CaveDigger cD;
	
	public Miner(int[] p, int i, CaveDigger caveDigger) {
		pos = p;
		id = i;
		cD = caveDigger;
	}
	
	public void Mine() {
		int[] newPos = GetMineableNeighbour();
		if (newPos == pos) {
//			Debug.Log ("Miner deactivated on pos " + pos);
			isActive = false;
		} else {
			pos = newPos;
			cD.digCount ++;
			if (pos[CaveDigger.DIM_X] == 0 || pos[CaveDigger.DIM_Y] == 0 || pos[CaveDigger.DIM_Z] == 0 || pos[CaveDigger.DIM_X] == cD.dimension[CaveDigger.DIM_X]-1 || pos[CaveDigger.DIM_Y] == cD.dimension[CaveDigger.DIM_Y]-1 || pos[CaveDigger.DIM_Z] == cD.dimension[CaveDigger.DIM_Z]-1) {
				Debug.Log("should not be!" );
			}
		}
		cD.cubeDensity[pos[CaveDigger.DIM_X],pos[CaveDigger.DIM_Y],pos[CaveDigger.DIM_Z]] = CaveDigger.DENSITY_EMPTY;
	}
	
	private int[] GetMineableNeighbour() {
		int[][] cubes = new int[][] { new int[] {0,0,0}, new int[] {0,0,0}, new int[] {0,0,0}, new int[] {0,0,0}, new int[] {0,0,0}, new int[] {0,0,0}};
		int possibilities = 0;
		
		for (int i=0; i<3; i++) {
			int ix = 0;
			int[] delta = new int[] {0,0,0};
			for (int step=-1; step<2; step+=2) {
				delta[i] = step;
				int[] stepPos = new int[] 
					{
						pos[CaveDigger.DIM_X]+delta[0],
						pos[CaveDigger.DIM_Y]+delta[1],
						pos[CaveDigger.DIM_Z]+delta[2]
					};
				if (		stepPos[CaveDigger.DIM_X] > 0 && stepPos[CaveDigger.DIM_X] < cD.dimension[CaveDigger.DIM_X]-1 
						&& 	stepPos[CaveDigger.DIM_Y] > 0 && stepPos[CaveDigger.DIM_Y] < cD.dimension[CaveDigger.DIM_Y]-1 
						&& 	stepPos[CaveDigger.DIM_Z] > 0 && stepPos[CaveDigger.DIM_Z] < cD.dimension[CaveDigger.DIM_Z]-1 
						&& 	cD.cubeDensity[stepPos[CaveDigger.DIM_X], stepPos[CaveDigger.DIM_Y], stepPos[CaveDigger.DIM_Z]] == CaveDigger.DENSITY_FILLED) {
//					cubes[2*i+ix] = new int[] {stepPos[0], stepPos[1], stepPos[2]};
					cubes[2*i+ix] = stepPos;
					ix++;
					possibilities++;
				}
			}
		}
		
		if (possibilities > 0) {
			int[] mineableNeighbours = new int[possibilities];
			int count = 0;
			for (int i=0; i<6; i++) {
				if (cubes[i][0] != 0 || cubes[i][1] != 0 || cubes[i][2] != 0) {
					mineableNeighbours[count] = i;
					count++;
				}
			}			
			// select randomly one of these
			return cubes[CaveDigger.GetRandomNumberFromPool(mineableNeighbours)];
		} else {
			return pos;
		}
	}

}
