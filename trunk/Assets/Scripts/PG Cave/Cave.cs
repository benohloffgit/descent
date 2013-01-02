using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cave {
	public List<Zone> zones;
	
	private IntTriple currentZone;
	private IntTriple nextZone;
	private IntTriple currentRoom;
	
	private int dimZone;
	private ZoneMiner[] miners;

	public static int DENSITY_FILLED = 0;
	public static int DENSITY_EMPTY = 1;

	public static IntTriple[] ZONE_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	
	public Cave() {
		currentZone = IntTriple.ZERO;
		zones = new List<Zone>();
		AddZone(currentZone);
		Debug.Log ("currentZone " + currentZone + ", nextZone " + nextZone);
//		nextZone = new IntTriple(0,1,0);
		DigZone();
	}
		
/*	public void CreateZone(int dim) {
		dimZone = dim;
		zones[currentZone.x, currentZone.y, currentZone.z] = new CellCube(dim);
		// dig rooms
		Dig(zones[currentZone.x, currentZone.y, currentZone.z], dimZone, 123456789);
	}*/
	
	public void AddZone(IntTriple pos) {
		zones.Add(new Zone(Game.DIMENSION_ZONE, pos));
		nextZone = AdvanceZone();
	}
	
	private IntTriple AdvanceZone() {
		IntTriple result = IntTriple.ZERO;
		List<IntTriple> directions = new List<IntTriple>(ZONE_DIRECTIONS);
		for (int i=0; i<directions.Count; i++) {
			result = Cave.ExtractRandomIntTripleFromPool(ref directions);
			if (true) { // TODO in range and only rightwards
				i = directions.Count;
			}
		}
		return result;
	}
	
	private void DigZone() {
		int digCount = 0;
		IntTriple entryRoom, exitRoom;
		SetEntryExit(nextZone-currentZone, out entryRoom, out exitRoom, Game.DIMENSION_ZONE, 0);
		Debug.Log ("entryRoom " + entryRoom + ", exitRoom " + exitRoom);
		miners = new ZoneMiner[2];
		miners[0] = new ZoneMiner(this, entryRoom, 0, zones[zones.Count-1]);
		miners[1] = new ZoneMiner(this, exitRoom, 1, zones[zones.Count-1]);
		miners[0].otherMiner = miners[1];
		miners[1].otherMiner = miners[0];
//		int j=0;
		while (miners[0].isActive || miners[1].isActive) {
			digCount += miners[0].Mine();
			digCount += miners[1].Mine();
//			j++;
		}
		Debug.Log ("Blocks digged: " + digCount);
	}
	
	public void SetAllMinersInactive() {
		for (int i=0; i<miners.Length; i++) {
			miners[i].isActive = false;
		}
	}

/*	
	public IntTriple GetPosOfActiveMinerOtherThan(int minerId) {
		IntTriple result = IntTriple.ZERO;
		for (int i=0; i<miners.Length; i++) {
			if (miners[i].isActive && miners[i].id != minerId) {
				result = miners[i].pos;
				i = miners.Length;
			}
		}
		return result;
	}*/
	
	private void SetEntryExit(IntTriple delta, out IntTriple entryRoom, out IntTriple exitRoom, int dim, int borderLimit) {
		IntDouble randomA = new IntDouble(
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)),
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)));
		IntDouble randomB = new IntDouble(
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)),
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)));
//		IntTriple delta = exitRoom - entryRoom;
		if (delta == IntTriple.UP) {
				entryRoom = new IntTriple(randomA.x, 0, randomA.y);
				exitRoom = new IntTriple(randomB.x, dim-1, randomB.y);
		} else if (delta == IntTriple.DOWN) {
				entryRoom = new IntTriple(randomA.x, dim-1, randomA.y);
				exitRoom = new IntTriple(randomB.x, 0, randomB.y);
		} else if (delta == IntTriple.LEFT) {
				entryRoom = new IntTriple(dim-1, randomA.x,randomA.y);
				exitRoom = new IntTriple(0,randomB.x,randomB.y);
		} else if (delta == IntTriple.RIGHT) {
				entryRoom = new IntTriple(0, randomA.x,randomA.y);
				exitRoom = new IntTriple(dim-1,randomB.x,randomB.y);
		} else if (delta == IntTriple.FORWARD) {
				entryRoom = new IntTriple(randomA.x,randomA.y,0);
				exitRoom = new IntTriple(randomB.x,randomB.y,dim-1);
		} else { // (delta == IntTriple.BACKWARD) {
				entryRoom = new IntTriple(randomA.x,randomA.y,dim-1);
				exitRoom = new IntTriple(randomB.x,randomB.y,0);
		}		
	}
	
/*	
	private void Dig(Cell c, int dim, int seed) {
		UnityEngine.Random.seed = seed;
		
		Miner[] miners = new Miner[2];
		
		// determine wall side ( 0:x=0, 1:x=dimX-1, 2:y=0, 3:y=dimY-1, 4:z=0, 5:z=dimZ-1)
		int[] sides = new int[] {0,1,2,3,4,5};
		ArrayList walls = new ArrayList(sides);
		IntTriple entry = DigEntryExit(Cave.ExtractRandomNumberFromPool(ref walls));
		c.SetDensity(entry, DENSITY_EMPTY);

		IntTriple exit = DigEntryExit(Cave.ExtractRandomNumberFromPool(ref walls));
		c.SetDensity(exit, DENSITY_EMPTY);
		
		DigCave();
	}
*/	
	private IntTriple DigEntryExit(int wall, int dim, int borderLimit) {
		IntDouble randomCoords = new IntDouble(
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)),
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)));
		IntTriple result = IntTriple.ZERO;
		switch (wall) {
			case 0:
				result = new IntTriple(0,randomCoords.x,randomCoords.y);
				break;
			case 1:
				result = new IntTriple(dim-1,randomCoords.x,randomCoords.y);
				break;
			case 2:
				result = new IntTriple(randomCoords.x,0,randomCoords.y);
				break;
			case 3:
				result = new IntTriple(randomCoords.x,dim-1,randomCoords.y);
				break;
			case 4:
				result = new IntTriple(randomCoords.x,randomCoords.y,0);
				break;
			case 5:
				result = new IntTriple(randomCoords.x,randomCoords.y,dim-1);
				break;
		}
		return result;
	}

	public static int GetRandomNumberFromPool(int[] pool) {
		return pool[UnityEngine.Random.Range(0, pool.Length-1)];
	}
	
	public static int ExtractRandomNumberFromPool(ref List<int> pool) {
		int random = UnityEngine.Random.Range(0, pool.Count);
		int result = pool[random];
		pool.RemoveAt(random);
		return result;
	}

	public static IntTriple ExtractRandomIntTripleFromPool(ref List<IntTriple> pool) {
		int random = UnityEngine.Random.Range(0, pool.Count);
		IntTriple result = pool[random];
		pool.RemoveAt(random);
		return result;
	}
}
