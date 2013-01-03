using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
