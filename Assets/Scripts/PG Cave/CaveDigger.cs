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


