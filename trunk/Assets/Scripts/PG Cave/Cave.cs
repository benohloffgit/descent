using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cave {
	public Zone zone;
	
	private Game game;
	private Play play;
	
	private int dimZone;
	private List<RoomMiner> roomMiners;
	private Door[] doors;
	private GameObject zoneEntry;
	private GameObject zoneExit;

	public static int DENSITY_FILLED = 0;
	public static int DENSITY_EMPTY = 1;

	public static IntTriple[] ZONE_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	public static IntTriple[] ROOM_DIRECTIONS = new IntTriple[] { IntTriple.FORWARD, IntTriple.BACKWARD, IntTriple.UP, IntTriple.DOWN, IntTriple.LEFT, IntTriple.RIGHT };
	
	public Cave(Play p) {
		play = p;
		game = play.game;
		doors = new Door[4];
		AddDoor(Door.TYPE_LAST_EXIT);
		AddDoor(Door.TYPE_ENTRY);
		AddDoor(Door.TYPE_EXIT);
		AddDoor(Door.TYPE_NEXT_ENTRY);
	}

	public void AddZone(int id) {
		zone = new Zone(Game.DIMENSION_ZONE, this, id);
		DigRooms();
	}

	public void RemoveZone() {
		foreach (GameObject gO in GameObject.FindGameObjectsWithTag(RoomMesh.TAG)) {
			GameObject.Destroy(gO);
		}
		foreach (GameObject gO in GameObject.FindGameObjectsWithTag(RoomMesh.TAG_ROOM_CONNECTOR)) {
			GameObject.Destroy(gO);
		}
		GameObject.Destroy(zoneEntry);
//		GameObject.Destroy(doors[Door.TYPE_LAST_EXIT].gameObject);
//		GameObject.Destroy(doors[Door.TYPE_ENTRY].gameObject);
		zone = null;
	}
	
	public void DigRooms() {
		int[] connectorsSet = new int[] {0,0,0,0,0,0,0,0,0,0};
		for (int i=0; i<zone.roomList.Count; i++) { // first in array is entry room
			roomMiners = new List<RoomMiner>();
			Room room = zone.roomList[i];
			List<Room> neighbours = zone.GetEmptyNeighboursOfRoom(room.pos);
			IntTriple startingCell;
			
//			Debug.Log ("Digging Room " + i + " on pos " +  room.pos);
			foreach (Room neighbour in neighbours) {
				IntTriple alignment = room.pos-neighbour.pos; // how we are positioned towards neighbor
//				Debug.Log ("room " + i + " has " + neighbours.Count + " neighbours, alignment is : " + alignment);
				if (neighbour.exits.ContainsKey(alignment)) {
					startingCell = GetOppositeCell(neighbour.exits[alignment], alignment);
				} else {
					startingCell = SetEntryExit(-1*alignment, 0, Game.DIMENSION_ROOM, 2);
				}
				if (connectorsSet[neighbour.id] == 0) {
					AddRoomConnector(new GridPosition(startingCell, room.pos), alignment);
				}
				if (i > 1) { // all rooms other than entry or exit room
					if (neighbours.Count == 1) { // additional dead end room with just one exit, we have to add a second miner
						roomMiners.Add(new RoomMiner(this, startingCell, -1*alignment, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection));
						startingCell = SetEntryExit(alignment, 0, Game.DIMENSION_ROOM, 4);
						roomMiners.Add(new RoomMiner(this, startingCell, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection));
					} else {
						roomMiners.Add(new RoomMiner(this, startingCell, -1*alignment, room, roomMiners.Count, RoomMiner.Type.QuitOn40Percent));
					}
				} else {
					roomMiners.Add(new RoomMiner(this, startingCell, -1*alignment, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection));
				}
			}
			if (i==0) { // entry room
				startingCell = SetEntryExit(IntTriple.BACKWARD, 0, Game.DIMENSION_ROOM, 2);
				roomMiners.Add(new RoomMiner(this, startingCell, IntTriple.BACKWARD, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection));
//				Debug.Log ("Entry Room: " + startingCell);
				if (zoneExit == null) {
					zoneEntry = AddZoneEntryExit(new GridPosition(startingCell, room.pos), 0f);
				} else {
					zoneEntry = zoneExit;
					play.ship.transform.parent = zoneEntry.transform;
					zoneEntry.transform.position = GetPositionFromGrid(new GridPosition(startingCell-new IntTriple(0,0,8), room.pos));
					play.ship.transform.parent = null;
				}
				doors[Door.TYPE_LAST_EXIT].transform.position = GetPositionFromGrid(new GridPosition(startingCell-new IntTriple(0,0,6), room.pos));
				doors[Door.TYPE_ENTRY].transform.position = GetPositionFromGrid(new GridPosition(startingCell, room.pos));
			} else if (i==1) { // exit room
				startingCell = SetEntryExit(IntTriple.FORWARD, 0, Game.DIMENSION_ROOM, 2);
				roomMiners.Add(new RoomMiner(this, startingCell, IntTriple.FORWARD, room, roomMiners.Count, RoomMiner.Type.QuitOnConnection));
				zoneExit = AddZoneEntryExit(new GridPosition(startingCell, room.pos), 180.0f);
				doors[Door.TYPE_EXIT].transform.position = GetPositionFromGrid(new GridPosition(startingCell+new IntTriple(0,0,2), room.pos));
				doors[Door.TYPE_NEXT_ENTRY].transform.position = GetPositionFromGrid(new GridPosition(startingCell+new IntTriple(0,0,8), room.pos));
				play.placeShipBeforeExitDoor = new GridPosition(startingCell, room.pos);
//				Debug.Log ("Exit Room: " + startingCell);				
			}
			
			connectorsSet[room.id] = 1;
//			Debug.Log ("setting connectorsSet forid " + room.id);
			
			int digCount = 0;
			int maxDig = Game.DIMENSION_ROOM_CUBED;
			bool isAtLeastOneMinerActive = true;
			int j=0;
			while (j<10000 && isAtLeastOneMinerActive) {
				j++;
				isAtLeastOneMinerActive = false;
				foreach (RoomMiner miner in roomMiners) {
					if (miner.isActive) {
						isAtLeastOneMinerActive = true;
						digCount += miner.Mine();
						if (miner.type == RoomMiner.Type.QuitOnConnection && miner.isConnectedToOtherMiner) {
							miner.isActive = false;
						} else if (miner.type == RoomMiner.Type.QuitOn40Percent && miner.isConnectedToOtherMiner && digCount >= maxDig * 0.2f) {
							miner.isActive = false;
						}
					}
				}
			}
			
//			if (j==10000) Debug.Log ("room miner count " + roomMiners.Count);
			Debug.Log ("Room " + i + " " + room.pos + " has cells: " + (digCount+roomMiners.Count) + " j=" + j);
//			if (i==2) room.TestRoomForSingleCells();
			CreateRoomMesh(room);
		}
		roomMiners.Clear();
		foreach (Door d in doors) {
			d.Reset();
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
		RoomMesh roomMesh = (GameObject.Instantiate(game.roomMeshPrefab) as GameObject).GetComponent<RoomMesh>();
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
		Room entryRoom = zone.roomList[0];
//		Debug.Log ("entryRoom pos " +entryRoom.pos +" " + entryRoom.entryCell);
		IntTriple pos = entryRoom.exits[IntTriple.BACKWARD].pos + new IntTriple(0,0,-3);
		return (entryRoom.pos.GetVector3() * Game.DIMENSION_ROOM + pos.GetVector3()) * RoomMesh.MESH_SCALE;
//		return (entryRoom.pos.GetVector3() * Game.DIMENSION_ROOM + entryRoom.exits[IntTriple.BACKWARD].pos.GetVector3()+Game.CELL_CENTER) * RoomMesh.MESH_SCALE;
	}
	
	public int GetCellDensity(GridPosition gridPosition) {
		return zone.GetCellDensity(gridPosition);
	}

	public int GetRoomDensity(GridPosition gridPosition) {
		return zone.GetRoomDensity(gridPosition);
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
					cellDensity = GetCellDensity(newPosition);
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

	public GridPosition GetNearestEmptyGridPositionFrom(GridPosition gridPosition) {
		GridPosition result = gridPosition;
		IntTriple cell = gridPosition.cellPosition;
		int dimension = Game.DIMENSION_ROOM;
		Room r = zone.GetRoom(gridPosition);
		for (int shells=0; shells<4; shells++) {
			int startDim = shells;
			for (int slices=0; slices<shells*2+1; slices++) {
				if (slices == 0 || slices == shells*2) { // full layer
					for (int nX=cell.x-startDim; nX<=cell.x+startDim; nX++) {
						if (nX >= 0 && nX<dimension) {
							for (int nY=cell.y-startDim; nY<=cell.y+startDim; nY++) {
								if (nY >= 0 && nY<dimension) {
									try {
										if (r.GetCellDensity(nX, nY, cell.z - startDim + slices) == Cave.DENSITY_EMPTY) {
											result.cellPosition = new IntTriple(nX, nY, cell.z - startDim + slices);
											return result;
										}
									} catch (IndexOutOfRangeException e) {
										Game.DefNull(e);
									}
								}
							}
						}
					}
				} else { // outer spiral
					for (int side=0; side<4; side++) {
						int x = cell.x-startDim;
						int y = cell.y-startDim;
						if (side == 1) {
							x += shells*2;
						} else if (side == 2) {
							x += shells*2;
							y += shells*2;
						} else if (side == 3) {
							y += shells*2;
						}
						for (int edge=0; edge<shells*2; edge++) {
							if (side == 0) {
								x++;
							} else if (side == 1) {
								y++;
							} else if (side == 2) {
								x--;
							} else if (side == 3) {
								y--;
							}
							try {
								if (r.GetCellDensity(x, y, cell.z - startDim + slices) == Cave.DENSITY_EMPTY) {
									result.cellPosition = new IntTriple(x, y, cell.z - startDim + slices);
									return result;
								}
							} catch (IndexOutOfRangeException e) {
								Game.DefNull(e);
							}
						}
					}
				}
			}
		}
		return result;
	}
	
	private void AddRoomConnector(GridPosition gP, IntTriple alignment) {
		GameObject rC = GameObject.Instantiate(game.roomConnectorPrefab) as GameObject;
		rC.transform.localScale *= RoomMesh.MESH_SCALE;
		rC.transform.position = GetPositionFromGrid(gP);
		if (alignment.x != 0) {
			rC.transform.Rotate(new Vector3(180.0f, 90.0f * alignment.x, 0));
		}
		if (alignment.y != 0) {
			rC.transform.Rotate(new Vector3(90.0f * alignment.y, 0, 0));
		}
		if (alignment.z != 0) {
			float turn = (alignment.z == 1) ? 180.0f : 0f;
			rC.transform.Rotate(new Vector3(turn, 0, 0));
		}
	}

	private GameObject AddZoneEntryExit(GridPosition gP, float rotation) {
		GameObject o = GameObject.Instantiate(game.roomEntryPrefab) as GameObject;
		o.transform.localScale *= RoomMesh.MESH_SCALE;
		o.transform.position = GetPositionFromGrid(gP);
		o.transform.Rotate(new Vector3(rotation, 0f, 0f));
		return o;
	}

	private void AddDoor(int doorType) {
		doors[doorType] = (GameObject.Instantiate(game.doorPrefab) as GameObject).GetComponent<Door>();
		doors[doorType].transform.localScale *= RoomMesh.MESH_SCALE;
		doors[doorType].Initialize(play, doorType);
	}
	
/*	public GridPosition GetGridFromPosition(Vector3 position) {
		Vector3 unscaled = position / RoomMesh.MESH_SCALE;
//		int roomOffset = Mathf.FloorToInt(unscaled.x / Game.DIMENSION_ROOM);
		Vector3 roomVector = unscaled / Game.DIMENSION_ROOM;
		IntTriple roomPos = new IntTriple(Mathf.FloorToInt(roomVector.x), Mathf.FloorToInt(roomVector.y), Mathf.FloorToInt(roomVector.z));
		Vector3 cellVector = unscaled - (roomPos * Game.DIMENSION_ROOM).GetVector3();
		if (cellVector.z < 0) {
			Debug.Log (position);
		}
		// centered in cube
		IntTriple cellPos = new IntTriple(Mathf.RoundToInt(cellVector.x), Mathf.RoundToInt(cellVector.y), Mathf.RoundToInt(cellVector.z));
		return new GridPosition(cellPos, roomPos);
	}*/
	
	public GridPosition GetGridFromPosition(Vector3 position) {
		Vector3 unscaled = position / RoomMesh.MESH_SCALE;
		IntTriple cellPos = new IntTriple(Mathf.RoundToInt(unscaled.x),Mathf.RoundToInt(unscaled.y),Mathf.RoundToInt(unscaled.z));
		IntTriple roomPos = cellPos / Game.DIMENSION_ROOM;
		cellPos %= Game.DIMENSION_ROOM;
//		if (roomPos.x == 0 || cellPos.x < 0 || cellPos.x > 15 || cellPos.y < 0 || cellPos.y > 15 || cellPos.z < 0 || cellPos.z > 15) {
//			Debug.Log (position +" " +  cellPos + " " + roomPos);
//		}
		
/*		IntTriple roomPos = new IntTriple(Mathf.FloorToInt(roomVector.x), Mathf.FloorToInt(roomVector.y), Mathf.FloorToInt(roomVector.z));
		Vector3 cellVector = unscaled - (roomPos * Game.DIMENSION_ROOM).GetVector3();
		if (cellVector.z < 0) {
			Debug.Log (position);
		}
		// centered in cube
		IntTriple cellPos = new IntTriple(Mathf.RoundToInt(cellVector.x), Mathf.RoundToInt(cellVector.y), Mathf.RoundToInt(cellVector.z));*/
		return new GridPosition(cellPos, roomPos);
	}
		
	public Vector3 GetPositionFromGrid(GridPosition gP) {
		return (gP.roomPosition.GetVector3() * Game.DIMENSION_ROOM + gP.cellPosition.GetVector3()) * RoomMesh.MESH_SCALE;
//		return (gP.roomPosition.GetVector3() * Game.DIMENSION_ROOM + gP.cellPosition.GetVector3()+Game.CELL_CENTER) * RoomMesh.MESH_SCALE;
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

}
