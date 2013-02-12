using System;
using UnityEngine;
using System.Collections.Generic;
	
public class Zone {
	public int dimension;
	public Room[,,] rooms;
	public List<Room> roomList;
	public IntTriple position;
	public IntTriple entryRoom;
	public IntTriple exitRoom;
	public IntTriple deltaToLastZone;
	
	private Cave cave;
	
	private static IntDouble[] ENTRYEXIT_POSITIONS = new IntDouble[] { new IntDouble(0,1), new IntDouble(1,1), new IntDouble(2,1), new IntDouble(1,0), new IntDouble(1,2) };

	public Zone(int dim, Cave c) {
		dimension = dim;
		cave = c;
		rooms = new Room[dim,dim,dim];
		roomList = new List<Room>();
		CreateRooms();
	}
	
	private void CreateRooms() {
		entryRoom = new IntTriple(ENTRYEXIT_POSITIONS[UnityEngine.Random.Range(0,5)],0);
		AddRoom(0, entryRoom);
		exitRoom = new IntTriple(ENTRYEXIT_POSITIONS[UnityEngine.Random.Range(0,5)],2);
		AddRoom(1, exitRoom);
		// dig from entry to exit
		IntTriple pos = entryRoom;
		while (pos != exitRoom) {
			IntTriple delta = exitRoom - pos;
			int random = UnityEngine.Random.Range(0,3);
			for (int i=0; i<3; i++) {
				if (delta.GetFactor(random) != 0) {
					pos.SetFactor(random, pos.GetFactor(random) + Math.Sign(delta.GetFactor(random)));
					if (pos != exitRoom) {
						AddRoom(2+i, pos);
					}
					i=3;
				} else {
					random++;
					if (random == 3) random = 0;
				}
			}
		}
	}
	
	public int GetRoomDensity(IntTriple pos) {
		if (rooms[pos.x, pos.y, pos.z] == null) {
			return Cave.DENSITY_FILLED;
		} else {
			return Cave.DENSITY_EMPTY;
		}
	}
	
	public Room GetRoom(GridPosition gP) {
		return rooms[gP.roomPosition.x, gP.roomPosition.y, gP.roomPosition.z];
	}
	
	public int GetCellDensity(GridPosition gP) {
		if (GetRoom(gP).cells[gP.cellPosition.x, gP.cellPosition.y, gP.cellPosition.z] == null) {
			return Cave.DENSITY_FILLED;
		} else {
			return Cave.DENSITY_EMPTY;
		}
	}
	
	public void AddRoom(int id, IntTriple pos) {
		rooms[pos.x, pos.y, pos.z] = new Room(id, Game.DIMENSION_ROOM, pos, cave);
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
}
