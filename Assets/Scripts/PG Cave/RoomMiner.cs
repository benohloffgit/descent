using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomMiner {
	public bool isActive = true;
	public IntTriple pos;
	public ZoneMiner otherMiner;
	
	private Cave cave;
	private Room2 room;
	
	public RoomMiner(Cave c, IntTriple p, IntTriple alignment, Room2 r) {
		cave = c;
		pos = p;
		room = r;
		room.AddExitCell(pos, alignment);
	}
	
	public int Mine() {
		int digged = 0;
		IntTriple newPos = GetMineableNeighbour();
		if (newPos == pos) {
			Debug.Log ("Miner deactivated on pos " + pos);
			isActive = false;
		} else {
			pos = newPos;
			room.AddCell(pos);
			digged++;
		}
		return digged;
	}
	
	private IntTriple GetMineableNeighbour() {
		List<IntTriple> possibilities = new List<IntTriple>();
		
		foreach (IntTriple step in Cave.ROOM_DIRECTIONS) {
			IntTriple newPos = pos + step;
			if (		newPos.x > 0 && newPos.x < room.dimension-1 
					&& 	newPos.y > 0 && newPos.y < room.dimension-1 
					&& 	newPos.z > 0 && newPos.z < room.dimension-1
					&& 	room.GetCellDensity(newPos) == Cave.DENSITY_FILLED	) {
				possibilities.Add(newPos);
			}
		}
		
		if (possibilities.Count > 0) {
			return possibilities[UnityEngine.Random.Range(0, possibilities.Count)];
		} else {
			return pos;
		}
	}
}
