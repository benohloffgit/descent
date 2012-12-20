using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public struct AStarNode {
	public EnemyDistributor.IntTriple intTriple;
	public Vector3 position;
	public float goal;
	public float heuristic;
	public float fitness;
	private int hashCode;

	public AStarNode(EnemyDistributor.IntTriple p, float g, float h) {
		intTriple = p;
		position = p.GetVector3();
		goal = g;
		heuristic = h;
		fitness = g + h;
		hashCode = (int) (position.x * Room.MESH_SCALE + position.y * (Room.MESH_SCALE*Room.MESH_SCALE) + position.z * (Room.MESH_SCALE*Room.MESH_SCALE*Room.MESH_SCALE));
	}
	
	public int GetHashCode() {
		return hashCode;
	}
	
}

