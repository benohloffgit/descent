using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cave {
	public List<Zone> zones;
	
	private IntTriple currentZone;
	private IntTriple nextZone;
	private IntTriple currentRoom;
	
	private int dimZone;

	public static int DENSITY_FILLED = 0;
	public static int DENSITY_EMPTY = 1;

	public static IntTriple[] ZONE_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	
	public Cave() {
		currentZone = IntTriple.ZERO;
		zones = new List<Zone>();
		AddZone(currentZone);
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
			if (true) { // TODO
				i = directions.Count;
			}
		}
		return result;
	}
	
	private void DigZone() {
		
/*		Miner2 miner0 = new Miner2(currentZone, 0, 
		while (miners[0].isActive || miners[1].isActive) {
			miners[0].Mine();
			miners[1].Mine();
		}
		Debug.Log ("Blocks digged: " + digCount);*/
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

	private void DigCave(Miner[] miners) {
		for (int i=0; i<miners.Length; i++) {
			miners[i] = new Miner(entry, i, this);
		}
		digCount = 0;
		while (miners[0].isActive || miners[1].isActive) {
			miners[0].Mine();
			miners[1].Mine();
		}
		Debug.Log ("Blocks digged: " + digCount);
	}
*/	
	private IntTriple DigEntryExit(int wall, int dim, int borderLimit) {
		IntTriple coords = new IntTriple(
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)),
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)),
			UnityEngine.Random.Range(borderLimit, dim-(borderLimit+1)));
		IntTriple result = IntTriple.ZERO;
		switch (wall) {
			case 0:
				result = new IntTriple(0,coords.y,coords.z);
				break;
			case 1:
				result = new IntTriple(dim-1,coords.y,coords.z);
				break;
			case 2:
				result = new IntTriple(coords.x,0,coords.z);
				break;
			case 3:
				result = new IntTriple(coords.x,dim-1,coords.z);
				break;
			case 4:
				result = new IntTriple(coords.x,coords.y,0);
				break;
			case 5:
				result = new IntTriple(coords.x,coords.y,dim-1);
				break;
		}
		return result;
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
