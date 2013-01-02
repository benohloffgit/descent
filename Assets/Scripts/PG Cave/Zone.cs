using System;
using UnityEngine;
	
public struct Zone {
	public int dimension;
	public Room2[,,] rooms; // nullable
	public IntTriple position;

	public Zone(int dim, IntTriple pos) {
		dimension = dim;
		rooms = new Room2[dim,dim,dim]; // nullable
		position = pos;
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
		rooms[pos.x, pos.y, pos.z] = new Room2(Game.DIMENSION_ROOM, minerId);
	}
	
}
