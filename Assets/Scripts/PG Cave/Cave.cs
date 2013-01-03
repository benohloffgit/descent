using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cave {
	public List<Zone> zones;
	
	private Play play;
	private IntTriple currentZone;
	private IntTriple nextZone;
	private IntTriple currentRoom;
	
	private int dimZone;
	private ZoneMiner[] zoneMiners;
	private RoomMiner[] roomMiners;
	private List<RoomMesh> roomMeshs;

	public static int DENSITY_FILLED = 0;
	public static int DENSITY_EMPTY = 1;

	public static IntTriple[] ZONE_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	public static IntTriple[] ROOM_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.BACKWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	
	public Cave(Play p) {
		play = p;
		currentZone = IntTriple.ZERO;
		zones = new List<Zone>();
		AddZone(currentZone, SetEntryExit(IntTriple.BACKWARD, 0, Game.DIMENSION_ZONE, 0), IntTriple.FORWARD);
		Debug.Log ("currentZone " + currentZone + ", nextZone " + nextZone);
		DigZone(zones[zones.Count-1]);
		DigRooms(zones[zones.Count-1]);
	}
		
/*	public void CreateZone(int dim) {
		dimZone = dim;
		zones[currentZone.x, currentZone.y, currentZone.z] = new CellCube(dim);
		// dig rooms
		Dig(zones[currentZone.x, currentZone.y, currentZone.z], dimZone, 123456789);
	}*/
	
	public void AddZone(IntTriple pos, IntTriple entryRoom, IntTriple deltaToLastZone) {
		zones.Add(new Zone(Game.DIMENSION_ZONE, pos, entryRoom, deltaToLastZone));
		nextZone = AdvanceZone();
	}
	
	private IntTriple AdvanceZone() {
		IntTriple result = IntTriple.ZERO;
		List<IntTriple> directions = new List<IntTriple>(ZONE_DIRECTIONS);
		for (int i=0; i<directions.Count; i++) {
			result = Cave.ExtractRandomIntTripleFromPool(ref directions);
			if (true) { // TODO in range and never to the left
				i = directions.Count;
			}
		}
		return IntTriple.FORWARD;
		//return result;
	}
	
	private void DigZone(Zone zone) {
		zone.exitRoom = SetEntryExit(nextZone-currentZone, 0, Game.DIMENSION_ZONE, 0);
		Debug.Log ("entryRoom " + zone.entryRoom + ", exitRoom " + zone.exitRoom);
		zoneMiners = new ZoneMiner[2];
		zoneMiners[0] = new ZoneMiner(this, zone.entryRoom, 0, zone);
		zoneMiners[1] = new ZoneMiner(this, zone.exitRoom, 1, zone);
		zoneMiners[0].otherMiner = zoneMiners[1];
		zoneMiners[1].otherMiner = zoneMiners[0];
//		int j=0;
		int digCount = 0;
		while (zoneMiners[0].isActive || zoneMiners[1].isActive) {
			digCount += zoneMiners[0].Mine();
			digCount += zoneMiners[1].Mine();
//			j++;
		}
		Debug.Log ("Zone has rooms: " + (digCount+2));
	}

	public void DigRooms(Zone zone) {
		for (int i=0; i<zone.roomList.Count; i++) { // first in array is entry room , second is exit room
			List<RoomMiner> roomMiners = new List<RoomMiner>();
			Room2 room = zone.roomList[i];
			List<Room2> neighbours = zone.GetEmptyNeighboursOfRoom(room.pos);
			IntTriple startingCell;
			Debug.Log ("Digging Room " + i + " on pos " +  room.pos);
			foreach (Room2 neighbour in neighbours) {
				IntTriple alignment = room.pos-neighbour.pos; // how we are positioned towards neighbor
				Debug.Log ("room " + i + " has " + neighbours.Count + " neighbours, alignment is : " + alignment);
				if (neighbour.exits.ContainsKey(alignment)) {
					startingCell = GetOppositeCell(neighbour.exits[alignment], alignment);
					Debug.Log ("01: " + startingCell);
				} else {
					startingCell = SetEntryExit(-1*alignment, 0, Game.DIMENSION_ROOM, 2);
					Debug.Log ("02: " + startingCell);
				}
				roomMiners.Add(new RoomMiner(this, startingCell, -1*alignment, room));
			}
			if (i==0) { // entry room
				startingCell = SetEntryExit(-1*zone.deltaToLastZone, 0, Game.DIMENSION_ROOM, 2); // + zone.entryRoom * Game.DIMENSION_ROOM;
				roomMiners.Add(new RoomMiner(this, startingCell, -1*zone.deltaToLastZone, room));
					Debug.Log ("Entry Room: " + startingCell);
			} else if (i==1) { // exit room
				startingCell = SetEntryExit(-1*nextZone, 0, Game.DIMENSION_ROOM, 2);
				roomMiners.Add(new RoomMiner(this, startingCell, -1*nextZone, room));
					Debug.Log ("Exit Room: " + startingCell);
			}
			
			int digCount = 0;
			int noOfActiveMiners = roomMiners.Count;
			while (noOfActiveMiners > 0) {
				foreach (RoomMiner miner in roomMiners) {
					if (miner.isActive) {
						int result = miner.Mine();
						digCount += result;
						if (result == 0) {
							noOfActiveMiners--;
						}
					}
				}
			}
			Debug.Log ("Room " + i + " has cells: " + (digCount+roomMiners.Count));
			
/*			IntTriple entryCell;
			if (i==0) { // entry room
				entryCell = SetEntryExit(zone.deltaToLastZone, 0, Game.DIMENSION_ROOM, 2); // + zone.entryRoom * Game.DIMENSION_ROOM;
				Debug.Log ("entryCell : " + entryCell);
			} else if (i==1) { // exit room
				entryCell = SetEntryExit(nextZone, 0, Game.DIMENSION_ROOM, 2);
			} else {
				entryCell = IntTriple.ZERO;
			}
			// determine all exit cells and create one miner for each
			List<Room2> neighbours = zone.GetEmptyNeighboursOfRoom(room.pos);
			Debug.Log ("Room " + room.pos + " has neighbours: " +  neighbours.Count);
			roomMiners = new RoomMiner[neighbours.Count + 1];
			roomMiners[0] = new RoomMiner(this, entryCell, 0, room);
			
			for (int j=0; j<neighbours.Count; j++) {
				IntTriple exitCell = SetEntryExit(neighbours[j].pos-room.pos, 0, Game.DIMENSION_ROOM, 2); // + neighbours[j].pos * Game.DIMENSION_ROOM;
				roomMiners[j+1] = new RoomMiner(this, exitCell, 0, room);
			}
			
			int digCount = 0;
			while (roomMiners[0].isActive || roomMiners[1].isActive) {
				digCount += roomMiners[0].Mine();
				digCount += roomMiners[1].Mine();
			}
			Debug.Log ("Room " + i + " has cells: " + (digCount+2));*/
			CreateRoomMesh(room);
		}
	}
	
	private IntTriple GetOppositeCell(Cell cell, IntTriple alignment) {
		IntTriple result = cell.pos;
		if (alignment == IntTriple.UP) {
			result.y = 0;
		} else if (alignment == IntTriple.DOWN) {
			result.y = Game.DIMENSION_ROOM-1;
		} else if (alignment == IntTriple.RIGHT) {
			result.x = 0;
		} else if (alignment == IntTriple.LEFT) {
			result.x = Game.DIMENSION_ROOM-1;
		} else if (alignment == IntTriple.FORWARD) {
			result.z = 0;
		} else if (alignment == IntTriple.BACKWARD) {
			result.z = Game.DIMENSION_ROOM-1;
		}
		return result;
	}
	
	private void CreateRoomMesh(Room2 room) {
		RoomMesh roomMesh = (GameObject.Instantiate(play.roomMeshPrefab) as GameObject).GetComponent<RoomMesh>();
		roomMesh.Initialize(room);
	}
	
	public void SetAllMinersInactive() {
		for (int i=0; i<zoneMiners.Length; i++) {
			zoneMiners[i].isActive = false;
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

	private IntTriple SetEntryExit(IntTriple delta, int lowDim, int highDim, int borderLimit) {
		IntTriple entryExit;
		IntDouble random = new IntDouble(
			UnityEngine.Random.Range(lowDim+borderLimit, highDim-(borderLimit+1)),
			UnityEngine.Random.Range(lowDim+borderLimit, highDim-(borderLimit+1)));
		if (delta == IntTriple.UP) {
			entryExit = new IntTriple(random.x, highDim-1, random.y);
		} else if (delta == IntTriple.DOWN) {
			entryExit = new IntTriple(random.x, lowDim, random.y);
		} else if (delta == IntTriple.LEFT) {
			entryExit = new IntTriple(lowDim,random.x,random.y);
		} else if (delta == IntTriple.RIGHT) {
			entryExit = new IntTriple(highDim-1,random.x,random.y);
		} else if (delta == IntTriple.FORWARD) {
			entryExit = new IntTriple(random.x,random.y,highDim-1);
		} else {
			entryExit = new IntTriple(random.x,random.y,lowDim);
		}
		return entryExit;
	}
	
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
