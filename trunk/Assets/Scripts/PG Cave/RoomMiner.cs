using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomMiner {
	public bool isActive = true;
	public IntTriple pos;
	public int id;
	/*
	 * if 3 Miners: all need to be connected to 1 other miner
	 * if 4 Miners: 1 needs to be connected to 2 other mines and all to 1
	 * if 5 Miners: (2 need to be connected to 2 other miners OR 1 needs to be connected to 3) and all to 1 
	 */ 				
//	public bool isConnectedToOneOtherMiner = false;
//	public bool isConnectedToTwoOtherMiners = false;
	private List<int> connectedMinerIDs;
	public int connectedToNumberOfOtherMiners = 0;
	public Type type;
	private int quitOnMaxMined;
	
	public static int NO_MINER = -1;
	
	private Cave cave;
	private Room room;
	public int mineCount;
	
//	private List<IntTriple> weightedCells;
	
	public enum Type { QuitOnConnection=0, QuitOnPercent=1, WeightBased=2 }

//	private static int MINE_COUNT_MAX_WHEN_QUIT_ON_CONNECTION = 50;
	
	// dead end miners
	public RoomMiner(Cave c, IntTriple p, Room r, int i, RoomMiner.Type t, int quitOnMaxMined_ = -1) {
		cave = c;
		pos = p;
		room = r;
		id = i;
		type = t;
		mineCount = 1;
		quitOnMaxMined = quitOnMaxMined_;
		room.AddDeadEndCell(pos, id);
		connectedMinerIDs = new List<int>();
		DigStarChamber();
		
/*		if (type == Type.WeightBased) {
			weightedCells = new List<IntTriple>();
			weightedCells.Add(new IntTriple(4,4,4));
			weightedCells.Add(new IntTriple(12,12,4));
			weightedCells.Add(new IntTriple(4,12,12));
			weightedCells.Add(new IntTriple(4,4,12));
		}*/
			
//		Debug.Log ("miner created with id " + id);
	}
	
	// exit miners
	public RoomMiner(Cave c, IntTriple p, IntTriple alignment, Room r, int i, RoomMiner.Type t, int quitOnMaxMined_ = -1) {
		cave = c;
		pos = p;
		room = r;
		id = i;
		type = t;
		room.AddExitCell(pos, alignment, id);
		mineCount = 1;
		connectedMinerIDs = new List<int>();
		quitOnMaxMined = quitOnMaxMined_;
		
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
			IntTriple newPos = pos;
			if (mineCount < quitOnMaxMined)  {
				newPos = GetMineableNeighbour();
			}
			if (newPos != pos) {
				Dig(newPos);
				digged++;
			} else {
				newPos = MoveTowardsOtherMiner();
				if (room.GetCellDensity(newPos) == Cave.DENSITY_FILLED) {
					Dig(newPos);
					digged++;
				}
			}
			pos = newPos;
		} else if (type == Type.QuitOnPercent) {
			IntTriple newPos = pos;
			if (mineCount < quitOnMaxMined)  {
				newPos = GetRandomNeighbour();
			}
			if (newPos != pos) {
				Dig(newPos);
				digged++;
			} else {
				newPos = MoveTowardsOtherMiner();
				if (room.GetCellDensity(newPos) == Cave.DENSITY_FILLED) {
					Dig(newPos);
					digged++;
				}
			}
			pos = newPos;
//			pos = GetRandomNeighbour();
//			if (room.GetCellDensity(pos) == Cave.DENSITY_FILLED) {
//				digged++;
//				room.AddCell(pos, id);
//			}
/*		} else if (type == Type.WeightBased) {
			pos = GetWeightedNeighbour();
			if (room.GetCellDensity(pos) == Cave.DENSITY_FILLED) {
				room.AddCell(pos, id);
				digged++;
			}*/
		}			 
		return digged;
	}
	
/*	private IntTriple GetWeightedNeighbour() {
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
	}		 */

	private IntTriple GetRandomNeighbour() {
		List<IntTriple> possibilities = new List<IntTriple>();
		
		foreach (IntTriple step in Cave.ROOM_DIRECTIONS) {
			IntTriple newPos = pos + step;
			if (		newPos.x > 0 && newPos.x < room.dimension-1 
					&& 	newPos.y > 0 && newPos.y < room.dimension-1 
					&& 	newPos.z > 0 && newPos.z < room.dimension-1 ) {
				possibilities.Add(newPos);
//				if (room.GetCellDensity(newPos) == Cave.DENSITY_EMPTY && room.IsCellNotEmptiedByMiner(newPos, id)) {
				if (room.GetCellDensity(newPos) == Cave.DENSITY_EMPTY && room.GetMinerIDOfCell(newPos) != id) {
					// we have a connection
					if (!connectedMinerIDs.Contains(room.GetMinerIDOfCell(newPos))) {
						connectedToNumberOfOtherMiners++;
						connectedMinerIDs.Add(room.GetMinerIDOfCell(newPos));
					}
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
				} else if (room.GetCellDensity(newPos) == Cave.DENSITY_EMPTY && room.GetMinerIDOfCell(newPos) != id) {
					// we have a connection
					//isConnectedToOtherMiner = true;
					if (!connectedMinerIDs.Contains(room.GetMinerIDOfCell(newPos))) {
						connectedToNumberOfOtherMiners++;
						connectedMinerIDs.Add(room.GetMinerIDOfCell(newPos));
						return pos;
					}
//					Debug.Log ("minecount A " + mineCount + " " + id);
				}				
			}
		}
		
		if (possibilities.Count > 0) {
			mineCount++;
			return possibilities[UnityEngine.Random.Range(0, possibilities.Count)];
		} else {
			return pos;
		}
	}
	
	private IntTriple MoveTowardsOtherMiner() {
		IntTriple newPos = pos;
		IntTriple otherMinerPos = cave.GetPosOfMinerOtherThan(id, connectedMinerIDs);
		IntTriple delta = otherMinerPos - pos;
		if (delta == IntTriple.ZERO) {
			connectedToNumberOfOtherMiners++;
			connectedMinerIDs.Add(room.GetMinerIDOfCell(otherMinerPos));
//			isConnectedToOtherMiner = true;
//			Debug.Log ("minecount B " + mineCount);
		} else {
			int biggestFactor = delta.GetBiggestAbsFactor();
			try {
				if (biggestFactor == 0) {
//					newPos.x += delta.x/Mathf.Abs(delta.x);
					newPos.x += (int)Mathf.Sign(delta.x);
				} else if (biggestFactor == 1) {
					newPos.y += (int)Mathf.Sign(delta.y);
				} else {
					newPos.z += (int)Mathf.Sign(delta.z);
				}
			} catch (DivideByZeroException e) {
				Game.DefNull(e);
			}
		}
//		Debug.Log (delta + " " + otherMinerPos + " " + pos + " " + newPos);
		return newPos;
	}
	
	private void DigStarChamber() {
		IntTriple newPos = pos + IntTriple.FORWARD;
		Dig(newPos);
		newPos = pos + IntTriple.BACKWARD;
		Dig(newPos);
		newPos = pos + IntTriple.UP;
		Dig(newPos);
		newPos = pos + IntTriple.DOWN;
		Dig(newPos);
	}
	
	private void Dig(IntTriple newPos) {
		room.AddCell(newPos, id);
		mineCount++;
	}
		
}
