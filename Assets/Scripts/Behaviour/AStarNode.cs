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

	public AStarNode(GridPosition p, float g, float h) {
		gridPos = p;
		position = p.cellPosition.GetVector3();
		goal = g;
		heuristic = h;
		fitness = g + h;
		hashCode = (int) (position.x * Game.DIMENSION_ROOM + position.y * Game.DIMENSION_ROOM_SQUARED + position.z * Game.DIMENSION_ROOM_CUBED);
	}
	
	public override int GetHashCode() {
		return hashCode;
	}
	
}

