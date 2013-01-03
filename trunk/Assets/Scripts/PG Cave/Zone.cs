using System;
using UnityEngine;
using System.Collections.Generic;
	
public class Zone {
	public int dimension;
	public Room2[,,] rooms;
	public List<Room2> roomList;
	public IntTriple position;
	public IntTriple entryRoom;
	public IntTriple exitRoom;
	public IntTriple deltaToLastZone;

	public Zone(int dim, IntTriple pos, IntTriple eR, IntTriple d) {
		dimension = dim;
		rooms = new Room2[dim,dim,dim];
		roomList = new List<Room2>();
		position = pos;
		entryRoom = eR;
		deltaToLastZone = d;
	}
	
	public int GetRoomDensity(IntTriple pos) {
		if (rooms[pos.x, pos.y, pos.z] == null) {
			return Cave.DENSITY_FILLED;
		} else {
			return Cave.DENSITY_EMPTY;
		}
	}
	
/*	public bool IsRoomEmptiedByMiner(IntTriple pos, int minerId) {
		if (rooms[pos.x, pos.y, pos.z] != null && rooms[pos.x, pos.y, pos.z].minerId == minerId) {
			return true;
		} else {
			return false;
		}
	}*/

	public bool IsRoomNotEmptiedByMiner(IntTriple pos, int minerId) {
		if (rooms[pos.x, pos.y, pos.z] != null && rooms[pos.x, pos.y, pos.z].minerId != minerId) {
			return true;
		} else {
			return false;
		}
	}
	
	public void AddRoom(IntTriple pos, int minerId) {
		rooms[pos.x, pos.y, pos.z] = new Room2(Game.DIMENSION_ROOM, minerId, pos);
		roomList.Add(rooms[pos.x, pos.y, pos.z]);
	}
	
	public List<Room2> GetEmptyNeighboursOfRoom(IntTriple pos) {
		List<Room2> neighbours = new List<Room2>();
		foreach (IntTriple step in Cave.ZONE_DIRECTIONS) {
			IntTriple newPos = pos + step;
			try {
				if (rooms[newPos.x, newPos.y, newPos.z] != null) {
					neighbours.Add(rooms[newPos.x, newPos.y, newPos.z]);
				}
			} catch (IndexOutOfRangeException e) {
			}
		}
		return neighbours;
	}
}
