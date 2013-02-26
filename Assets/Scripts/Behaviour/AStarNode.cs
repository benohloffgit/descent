using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public struct AStarNode {
	public GridPosition gridPos;
	public Vector3 position;
	public float goal;
	public float heuristic;
	public float fitness;
	private int hashCode;
//	private int cellHash;
//	private int roomHash;

	public AStarNode(AStarThreadState.Mode mode, GridPosition p, float g, float h) {
		gridPos = p;
		position = gridPos.GetVector3();
		goal = g;
		heuristic = h;
		fitness = g + h;
		if (mode == AStarThreadState.Mode.ROOM) {
			hashCode = (int) (p.cellPosition.x * Game.DIMENSION_ROOM + p.cellPosition.y * Game.DIMENSION_ROOM_SQUARED + p.cellPosition.z * Game.DIMENSION_ROOM_CUBED);
		} else {
//			p.cellPosition = IntTriple.ZERO;
			hashCode = (int) (p.roomPosition.x * Game.DIMENSION_ZONE + p.roomPosition.y * Game.DIMENSION_ZONE_SQUARED + p.roomPosition.z * Game.DIMENSION_ZONE_CUBED);
		}
	}
	
	public override int GetHashCode() {
		return hashCode;
	}
	
/*	public int GetCellHash() {
		return cellHash;
	}

	public int GetRoomHash() {
		return roomHash;
	}*/
	
}

