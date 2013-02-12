using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomMiner {
	public bool isActive = true;
	public IntTriple pos;
	public int id;
	public bool isConnectedToOtherMiner = false;
	
	private Cave cave;
	private Room room;
	private int mineCount;
	private Type type;
	
//	private List<IntTriple> weightedCells;
	
	public enum Type { QuitOnConnection=0, QuitOn40Percent=1, WeightBased=2 }

	private static int MINE_COUNT_MAX_WHEN_QUIT_ON_CONNECTION = 250;
	
	public RoomMiner(Cave c, IntTriple p, IntTriple alignment, Room r, int i, RoomMiner.Type t) {
		cave = c;
		pos = p;
		room = r;
		id = i;
		type = t;
		room.AddExitCell(pos, alignment, id);
		mineCount = 1;
		
/*		if (type == Type.WeightBased) {
			weightedCells = new List<IntTriple>();
			weightedCells.Add(new IntTriple(4,4,4));
			weightedCells.Add(new IntTriple(12,12,4));
			weightedCells.Add(new IntTriple(4,12,12));
			weightedCells.Add(new IntTriple(4,4,12));
		}*/
			
//		Debug.Log ("miner created with id " + id);
	}
	
	public int Mine() {
		int digged = 0;
		if (type == Type.QuitOnConnection) {
			IntTriple newPos = GetMineableNeighbour();
			if (newPos != pos) {
				pos = newPos;
				room.AddCell(pos, id);
				digged++;
			}
		} else if (type == Type.QuitOn40Percent) {
			pos = GetRandomNeighbour();
			if (room.GetCellDensity(pos) == Cave.DENSITY_FILLED) {
				room.AddCell(pos, id);
				digged++;
			}
		} else if (type == Type.WeightBased) {
			pos = GetWeightedNeighbour();
			if (room.GetCellDensity(pos) == Cave.DENSITY_FILLED) {
				room.AddCell(pos, id);
				digged++;
			}
		}
		return digged;
	}
	
	private IntTriple GetWeightedNeighbour() {
		List<WeightedProbability> possibilities = new List<WeightedProbability>();
		IntTriple center = new IntTriple(8,8,8);
		int lastProbabilityIndex = 0;
		foreach (IntTriple step in Cave.ROOM_DIRECTIONS) {
			IntTriple newPos = pos + step;
			if (		newPos.x > 0 && newPos.x < room.dimension-1 
					&& 	newPos.y > 0 && newPos.y < room.dimension-1 
					&& 	newPos.z > 0 && newPos.z < room.dimension-1 ) {
				float distanceToCenter = (center - newPos).Magnitude();
				int newProbabilityIndex = 100 - Mathf.RoundToInt(distanceToCenter * 10.0f);
				possibilities.Add(new WeightedProbability(newPos, lastProbabilityIndex, lastProbabilityIndex + newProbabilityIndex - 1));
				lastProbabilityIndex += newProbabilityIndex;
				if (room.GetCellDensity(newPos) == Cave.DENSITY_EMPTY && room.IsCellNotEmptiedByMiner(newPos, id)) {
					// we have a connection
					isConnectedToOtherMiner = true;
				}				
			}
		}
		int random = UnityEngine.Random.Range(0, lastProbabilityIndex);
		int seek = 0;
		for (int i=0; i<possibilities.Count; i++) {
			if (possibilities[i].maxProbability > random && possibilities[i].minProbability <= random) {
				seek = i;
				i = possibilities.Count;
			}
		}
		return possibilities[seek].baseValue;
	}

	private IntTriple GetRandomNeighbour() {
		List<IntTriple> possibilities = new List<IntTriple>();
		
		foreach (IntTriple step in Cave.ROOM_DIRECTIONS) {
			IntTriple newPos = pos + step;
			if (		newPos.x > 0 && newPos.x < room.dimension-1 
					&& 	newPos.y > 0 && newPos.y < room.dimension-1 
					&& 	newPos.z > 0 && newPos.z < room.dimension-1 ) {
				possibilities.Add(newPos);
				if (room.GetCellDensity(newPos) == Cave.DENSITY_EMPTY && room.IsCellNotEmptiedByMiner(newPos, id)) {
					// we have a connection
					isConnectedToOtherMiner = true;
				}				
			}
		}
		return possibilities[UnityEngine.Random.Range(0, possibilities.Count)];
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
					isConnectedToOtherMiner = true;
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
			if (mineCount > MINE_COUNT_MAX_WHEN_QUIT_ON_CONNECTION) { // move towards
//				Debug.Log ("moving towards other miner...");
				IntTriple delta = cave.GetPosOfActiveMinerOtherThan(id) - pos;
				if (delta == IntTriple.ZERO) {
					isConnectedToOtherMiner = true;
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
