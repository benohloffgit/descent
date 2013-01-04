using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cave {
	public List<Zone> zones;
	
	private Play play;
	
	private int dimZone;
	private List<RoomMiner> roomMiners;
	private List<RoomMesh> roomMeshs;

	public static int DENSITY_FILLED = 0;
	public static int DENSITY_EMPTY = 1;

	public static IntTriple[] ZONE_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	public static IntTriple[] ROOM_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.BACKWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	
	public Cave(Play p) {
		play = p;
		zones = new List<Zone>();
		AddZone();
		DigRooms(GetCurrentZone());
	}

	public void AddZone() {
		zones.Add(new Zone(Game.DIMENSION_ZONE, this));
	}
	
	public Zone GetCurrentZone() {
		return zones[zones.Count-1];
	}
	
	public void DigRooms(Zone zone) {
		for (int i=0; i<zone.roomList.Count; i++) { // first in array is entry room , second is exit room
			roomMiners = new List<RoomMiner>();
			Room room = zone.roomList[i];
			List<Room> neighbours = zone.GetEmptyNeighboursOfRoom(room.pos);
			IntTriple startingCell;
			Debug.Log ("Digging Room " + i + " on pos " +  room.pos);
			foreach (Room neighbour in neighbours) {
				IntTriple alignment = room.pos-neighbour.pos; // how we are positioned towards neighbor
//				Debug.Log ("room " + i + " has " + neighbours.Count + " neighbours, alignment is : " + alignment);
				if (neighbour.exits.ContainsKey(alignment)) {
					startingCell = GetOppositeCell(neighbour.exits[alignment], alignment);
//					Debug.Log ("01: " + startingCell);
				} else {
					startingCell = SetEntryExit(-1*alignment, 0, Game.DIMENSION_ROOM, 2);
//					Debug.Log ("02: " + startingCell);
				}
				roomMiners.Add(new RoomMiner(this, startingCell, -1*alignment, room, roomMiners.Count));
			}
			if (i==0) { // entry room
				startingCell = SetEntryExit(IntTriple.BACKWARD, 0, Game.DIMENSION_ROOM, 2);
				roomMiners.Add(new RoomMiner(this, startingCell, IntTriple.BACKWARD, room, roomMiners.Count));
				room.entryCell = startingCell;
//				Debug.Log ("Entry Room: " + startingCell);
			} else if (i==1) { // exit room
				startingCell = SetEntryExit(IntTriple.FORWARD, 0, Game.DIMENSION_ROOM, 2);
				roomMiners.Add(new RoomMiner(this, startingCell, IntTriple.FORWARD, room, roomMiners.Count));
//				Debug.Log ("Exit Room: " + startingCell);
			}
			
			int digCount = 0;
			bool isAtLeastOneMinerActive = true;
			int j=0;
			while (j<10000 && isAtLeastOneMinerActive) {
				j++;
				isAtLeastOneMinerActive = false;
				foreach (RoomMiner miner in roomMiners) {
					if (miner.isActive) {
						isAtLeastOneMinerActive = true;
						digCount += miner.Mine();
					}
				}
			}
			if (j==10000) Debug.Log ("room miner count " + roomMiners.Count);
			Debug.Log ("Room " + i + " has cells: " + (digCount+roomMiners.Count) + " j=" + j);
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
	
	private void CreateRoomMesh(Room room) {
		RoomMesh roomMesh = (GameObject.Instantiate(play.roomMeshPrefab) as GameObject).GetComponent<RoomMesh>();
		roomMesh.Initialize(room);
		room.roomMesh = roomMesh;
	}
	
	public void SetAllMinersInactive() {
		foreach (RoomMiner miner in roomMiners) {
			miner.isActive = false;
		}
	}

	public IntTriple GetPosOfActiveMinerOtherThan(int minerId) {
		IntTriple result = IntTriple.ZERO;
		foreach (RoomMiner miner in roomMiners) {
			if (miner.id != minerId) {
				result = miner.pos;
				break;
			}
		}
		return result;
	}

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
	
	public static Vector3 GetPositionFromCube(Vector3 cubePosition) {
		return cubePosition * RoomMesh.MESH_SCALE;
	}

	public Vector3 GetCaveEntryPosition() {
		Room entryRoom = GetCurrentZone().roomList[0];
//		Debug.Log ("entryRoom pos " +entryRoom.pos +" " + entryRoom.entryCell);
		return (entryRoom.pos.GetVector3() * Game.DIMENSION_ROOM + entryRoom.entryCell.GetVector3()) * RoomMesh.MESH_SCALE;
	}
	
	// return pos in marching cube grid
	public static Vector3 GetCubePosition(Vector3 position) {
		Vector3 cubePos = position / RoomMesh.MESH_SCALE;
		// centered in cube
		return new Vector3(Mathf.RoundToInt(cubePos.x), Mathf.RoundToInt(cubePos.y), Mathf.RoundToInt(cubePos.z));
	}
	
	public int GetCellDensity(GridPosition gridPosition) {
		return GetCurrentZone().GetCellDensity(gridPosition);
	}

	public GridPosition GetRandomEmptyGridPositionFrom(GridPosition gridPosition, int maxDistance) {
		GridPosition result = gridPosition;
		int currentDirection = UnityEngine.Random.Range(0, RoomMesh.DIRECTIONS.Length);
//		Debug.Log ("starting with direction " + currentDirection);
		for (int i=0; i<RoomMesh.DIRECTIONS.Length; i++) {
			// test up to max Distance in that direction
			for (int j=0; j<maxDistance; j++) {
				GridPosition newPosition = new GridPosition(RoomMesh.DIRECTIONS[currentDirection] * (j+1) + gridPosition.cellPosition, gridPosition.roomPosition);
				int cellDensity;
				try {
					cellDensity =  GetCellDensity(newPosition);
				} catch (IndexOutOfRangeException e) {
					Game.DefNull(e);
					cellDensity = Cave.DENSITY_FILLED;
				}
				if (cellDensity == Cave.DENSITY_FILLED) {
					if (j > 0) {
						// exit
						i = RoomMesh.DIRECTIONS.Length;
					}
					// exit
					j = maxDistance;
				} else {
					result = newPosition;
//					Debug.Log ("result " + result);
				}
			}
			if (result != gridPosition) {
				// exit
				i = RoomMesh.DIRECTIONS.Length;
			} else {
				currentDirection++;
				if (currentDirection == RoomMesh.DIRECTIONS.Length) {
					currentDirection = 0;
				}
			}
		}
//		Debug.Log ("final direction " + currentDirection);
		return result;
	}

	public static GridPosition GetGridFromPosition(Vector3 position) {
		Vector3 unscaled = position / RoomMesh.MESH_SCALE;
		Vector3 roomVector = unscaled / Game.DIMENSION_ROOM;
		IntTriple roomPos = new IntTriple(Mathf.FloorToInt(roomVector.x), Mathf.FloorToInt(roomVector.y), Mathf.FloorToInt(roomVector.z));
		Vector3 cellPos = unscaled - (roomPos * Game.DIMENSION_ROOM).GetVector3();
		// centered in cube
		return new GridPosition(new IntTriple(Mathf.RoundToInt(cellPos.x), Mathf.RoundToInt(cellPos.y), Mathf.RoundToInt(cellPos.z)), roomPos);
	}
	
	public static Vector3 GetPositionFromGrid(GridPosition rP) {
		return (rP.roomPosition.GetVector3() * Game.DIMENSION_ROOM + rP.cellPosition.GetVector3()) * RoomMesh.MESH_SCALE;
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
