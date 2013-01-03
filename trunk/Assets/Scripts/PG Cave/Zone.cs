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
	
	private static IntDouble[] ENTRYEXIT_POSITIONS = new IntDouble[] { new IntDouble(0,1), new IntDouble(1,1), new IntDouble(2,1), new IntDouble(1,0), new IntDouble(1,2) };

	public Zone(int dim) {
		dimension = dim;
		rooms = new Room2[dim,dim,dim];
		roomList = new List<Room2>();
		CreateRooms();
	}
	
	public Zone(int dim, IntTriple pos, IntTriple eR, IntTriple d) {
		dimension = dim;
		rooms = new Room2[dim,dim,dim];
		roomList = new List<Room2>();
		position = pos;
		entryRoom = eR;
		deltaToLastZone = d;
	}
	
	private void CreateRooms() {
		entryRoom = new IntTriple(ENTRYEXIT_POSITIONS[UnityEngine.Random.Range(0,5)],0);
		AddRoom(entryRoom);
		exitRoom = new IntTriple(ENTRYEXIT_POSITIONS[UnityEngine.Random.Range(0,5)],2);
		AddRoom(exitRoom);
		// dig from entry to exit
		IntTriple pos = entryRoom;
		while (pos != exitRoom) {
			IntTriple delta = exitRoom - pos;
			int random = UnityEngine.Random.Range(0,3);
			for (int i=0; i<3; i++) {
				if (delta.GetFactor(random) != 0) {
					pos.SetFactor(random, pos.GetFactor(random) + Math.Sign(delta.GetFactor(random)));
					if (pos != exitRoom) {
						Debug.Log (pos);
						AddRoom(pos);
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
	
	private void AddRoom(IntTriple pos) {
		rooms[pos.x, pos.y, pos.z] = new Room2(Game.DIMENSION_ROOM, pos);
		roomList.Add(rooms[pos.x, pos.y, pos.z]);
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
