using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneMiner {
	public bool isActive = true;
	public int id;	
	public IntTriple pos;
	public ZoneMiner otherMiner;
	
	private Cave cave;
	private Zone zone;
	
	public ZoneMiner(Cave c, IntTriple p, int i, Zone z) {
		cave = c;
		pos = p;
		id = i;
		zone = z;
		zone.AddRoom(pos, id);
	}
	
	public int Mine() {
		int digged = 0;
		if (isActive) {
			IntTriple newPos = GetMineableNeighbour();
			if (newPos != pos) {
				pos = newPos;
				zone.AddRoom(pos, id);
//				Debug.Log ("Mined: " + pos);
				digged++;
			}
		}
		return digged;
	}
	
	private IntTriple GetMineableNeighbour() {
//		IntTriple[] cubes = new IntTriple[] { IntTriple.ZERO,IntTriple.ZERO,IntTriple.ZERO,IntTriple.ZERO,IntTriple.ZERO,IntTriple.ZERO };
		List<IntTriple> possibilities = new List<IntTriple>();
		
		foreach (IntTriple step in Cave.ZONE_DIRECTIONS) {
			IntTriple newPos = pos + step;
			if (		newPos.x >= 0 && newPos.x < zone.dimension 
					&& 	newPos.y >= 0 && newPos.y < zone.dimension 
					&& 	newPos.z >= 0 && newPos.z < zone.dimension ) {
				if (zone.GetRoomDensity(newPos) == CaveDigger.DENSITY_FILLED) {
					possibilities.Add(newPos);
					if ((otherMiner.pos - newPos).Magnitude() < (otherMiner.pos - pos).Magnitude()) {
						// if closer to other miner, add newPos again to increase chances moving towards other miner
						possibilities.Add(newPos);
						possibilities.Add(newPos);
					}
				} else if (zone.GetRoomDensity(newPos) == CaveDigger.DENSITY_EMPTY && zone.IsRoomNotEmptiedByMiner(newPos, id)) {
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
//			Debug.Log ("all cubes around me mined by myself, moving towards other miner");
			IntTriple delta = otherMiner.pos - pos;
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
			return pos;
		}
	}
}
