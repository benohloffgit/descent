using System;
using UnityEngine;
using System.Collections.Generic;
	
public class Zone {
	public int id;
	public int dimension;
	public Room[,,] rooms;
	public List<Room> roomList;
	public IntTriple position;
	public IntTriple entryRoom;
	public IntTriple exitRoom;
	public IntTriple deltaToLastZone;
	public List<GridPosition> keyCells;
	
	public bool hasBugBeenDisplayed = false;
	
	private Cave cave;
	
	private static IntDouble[] ENTRYEXIT_POSITIONS = new IntDouble[] { new IntDouble(0,1), new IntDouble(1,1), new IntDouble(2,1), new IntDouble(1,0), new IntDouble(1,2) };

	public Zone(int dimension_, Cave cave_, int id_) {
		dimension = dimension_;
		cave = cave_;
		id = id_;
		rooms = new Room[dimension,dimension,dimension];
		roomList = new List<Room>();
		keyCells = new List<GridPosition>();
		CreateRooms();
	}
	
	private void CreateRooms() {
		int entryExitPossibilities = cave.play.zoneID > Game.BEGINNER_ZONES ? 5 : 2;
		entryRoom = new IntTriple(ENTRYEXIT_POSITIONS[UnityEngine.Random.Range(0,entryExitPossibilities)],0);
		AddRoom(0, entryRoom);
		exitRoom = new IntTriple(ENTRYEXIT_POSITIONS[UnityEngine.Random.Range(0,entryExitPossibilities)],2);
		AddRoom(1, exitRoom);
		// dig from entry to exit
		IntTriple pos = entryRoom;
		int count = 0;
		while (pos != exitRoom) {
			IntTriple delta = exitRoom - pos;
			int random = UnityEngine.Random.Range(0,3);
			for (int i=0; i<3; i++) {
				if (delta.GetFactor(random) != 0) {
					pos.SetFactor(random, pos.GetFactor(random) + Math.Sign(delta.GetFactor(random)));
					if (pos != exitRoom) {
						AddRoom(2+count, pos);
						count++;
					}
					i=3;
				} else {
					random++;
					if (random == 3) random = 0;
				}
			}
		}
		if (cave.play.zoneID > Game.BEGINNER_ZONES) {
			AddAdditionalRooms();
		}			
		Debug.Log ("Rooms generated " + roomList.Count);
	}
	
	private void AddAdditionalRooms() {
		List<Room> newRoomList = new List<Room>();
		foreach (Room r in roomList) {
			if (UnityEngine.Random.Range(0,2) == 0) { // 50/50 chance
				// add available room positions of neighbouring rooms to array and select one randomly
				List<IntTriple> neighbours = GetFilledNeighboursOfRoom(r.pos);
				if (neighbours.Count > 0) {
					IntTriple newPos = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
					rooms[newPos.x, newPos.y, newPos.z] = new Room(newRoomList.Count+roomList.Count, Game.DIMENSION_ROOM, newPos, this);
					newRoomList.Add(rooms[newPos.x, newPos.y, newPos.z]);
				}
/*				foreach (IntTriple direction in Cave.ZONE_DIRECTIONS) {
					IntTriple newPos = r.pos + direction;
					if (newPos.x >=0  && newPos.x <= 2 && newPos.y >=0  && newPos.y <= 2 && newPos.z >=0  && newPos.z <= 2
						&& rooms[newPos.x, newPos.y, newPos.z] == null) {
//							AddRoom(newRoomList.Count-1, newPos);
							rooms[newPos.x, newPos.y, newPos.z] = new Room(newRoomList.Count+roomList.Count, Game.DIMENSION_ROOM, newPos, cave);
							newRoomList.Add(rooms[newPos.x, newPos.y, newPos.z]);
					}
				}*/
			}
		}
		roomList.AddRange(newRoomList);
		Debug.Log ("Added AddAdditionalRooms " + newRoomList.Count);
	}

	public Room GetRoom(GridPosition gP) {
		return rooms[gP.roomPosition.x, gP.roomPosition.y, gP.roomPosition.z];
	}
	
	public Room GetRandomRoom() {
		return roomList[UnityEngine.Random.Range(0, roomList.Count)];
	}
	
	public int GetRoomDensity(GridPosition gP) {
		if (rooms[gP.roomPosition.x, gP.roomPosition.y, gP.roomPosition.z] == null) {
			return Cave.DENSITY_FILLED;
		} else {
			return Cave.DENSITY_EMPTY;
		}
	}
	
	public int GetCellDensity(GridPosition gP) {
		try {
			return GetRoom(gP).GetCellDensity(gP.cellPosition);
		} catch (NullReferenceException e) {
			Game.DefNull(e);
			if (!hasBugBeenDisplayed) {
				Debug.Log ("NULL " + gP);
				hasBugBeenDisplayed = true;
				throw new NullReferenceException();
			}
			return Cave.DENSITY_EMPTY;
		}
	}
	
	public void AddRoom(int id, IntTriple pos) {
		rooms[pos.x, pos.y, pos.z] = new Room(id, Game.DIMENSION_ROOM, pos, this);
		roomList.Add(rooms[pos.x, pos.y, pos.z]);
	}
	
	public List<Room> GetEmptyNeighboursOfRoom(IntTriple pos) {
		List<Room> neighbours = new List<Room>();
		foreach (IntTriple step in Cave.ROOM_DIRECTIONS) {
			IntTriple newPos = pos + step;
			try {
				if (rooms[newPos.x, newPos.y, newPos.z] != null) {
					neighbours.Add(rooms[newPos.x, newPos.y, newPos.z]);
				}
			} catch (IndexOutOfRangeException e) {
				Game.DefNull(e);
			}
		}
		return neighbours;
	}

	public List<IntTriple> GetFilledNeighboursOfRoom(IntTriple pos) {
		List<IntTriple> neighbours = new List<IntTriple>();
		foreach (IntTriple step in Cave.ROOM_DIRECTIONS) {
			IntTriple newPos = pos + step;
			try {
				if (rooms[newPos.x, newPos.y, newPos.z] == null) {
					neighbours.Add(newPos);
				}
			} catch (IndexOutOfRangeException e) {
				Game.DefNull(e);
			}
		}
		return neighbours;
	}
	
}
