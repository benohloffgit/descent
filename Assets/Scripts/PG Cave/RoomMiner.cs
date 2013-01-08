using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomMiner {
	public bool isActive = true;
	public IntTriple pos;
	public int id;
	
	private Cave cave;
	private Room room;
	private int mineCount;
	
	private static int MINE_COUNT_MAX = 250;
	
	public RoomMiner(Cave c, IntTriple p, IntTriple alignment, Room r, int i) {
		cave = c;
		pos = p;
		room = r;
		id = i;
		room.AddExitCell(pos, alignment, id);
		mineCount = 1;
//		Debug.Log ("miner created with id " + id);
	}
	
	public int Mine() {
		int digged = 0;
		IntTriple newPos = GetMineableNeighbour();
		if (newPos != pos) {
			pos = newPos;
			room.AddCell(pos, id);
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
					&& 	newPos.z > 0 && newPos.z < room.dimension-1 ) {
				if (room.GetCellDensity(newPos) == Cave.DENSITY_FILLED) {
					possibilities.Add(newPos);
				} else if (room.GetCellDensity(newPos) == Cave.DENSITY_EMPTY && room.IsCellNotEmptiedByMiner(newPos, id)) {
					// we have a connection
					cave.SetAllMinersInactive();
//					Debug.Log ("Miner deactivated on pos " + pos);
					return pos;
				}				
			}
		}
		
		if (possibilities.Count > 0) {
			return possibilities[UnityEngine.Random.Range(0, possibilities.Count)];
		} else {
			mineCount++;
			if (mineCount > MINE_COUNT_MAX) { // move towards
//				Debug.Log ("moving towards other miner...");
				IntTriple delta = cave.GetPosOfActiveMinerOtherThan(id) - pos;
				if (delta == IntTriple.ZERO) {
					cave.SetAllMinersInactive();
				} else {
					int biggestFactor = delta.GetBiggestFactor();
					if (biggestFactor == 0) {
						pos.x += delta.x/Mathf.Abs(delta.x);
					} else if (biggestFactor == 1) {
						pos.y += delta.y/Mathf.Abs(delta.y);
					} else {
						pos.z += delta.z/Mathf.Abs(delta.z);
					}
				}
			}
			return pos;
		}
	}
}
