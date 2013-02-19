using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Movement {
	private Play play;
	private Cave cave;
	
	private static float RAYCAST_DISTANCE = 2.0f;
	
	public Movement(Play p) {
		play = p;
		cave = play.cave;
	}

	public void Chase(Rigidbody rigidbody, GridPosition targetPosition, float force, ref bool isOnPath) {
		Vector3 position = rigidbody.transform.position;
		GridPosition currentPosition = cave.GetGridFromPosition(position);
		if (currentPosition == targetPosition) {
			isOnPath = false;
		} else {
			Vector3 avoidance = Vector3.zero;	
			RaycastHit hit;
			for (int i=0; i<RoomMesh.DIRECTIONS.Length; i++) {
				if (Physics.Raycast(position, RoomMesh.DIRECTIONS[i], out hit, RAYCAST_DISTANCE, Game.LAYER_MASK_ALL)) {
					avoidance += hit.normal * (RAYCAST_DISTANCE/hit.distance);
				}
			}
			Vector3 target = (cave.GetPositionFromGrid(targetPosition) - position).normalized;
			rigidbody.AddForce((avoidance.normalized + target) * force);			
		}
	}
	
	public void Roam(Rigidbody rigidbody, ref GridPosition targetPosition, int minDistance, int maxDistance, float force) {
		Vector3 position = rigidbody.transform.position;
		GridPosition currentPosition = cave.GetGridFromPosition(position);
		if (currentPosition == targetPosition) {
			targetPosition = play.cave.GetRandomEmptyGridPositionFrom(currentPosition, UnityEngine.Random.Range(minDistance,maxDistance+1));
//			Debug.Log ("current " + cubePosition + ", setting new target " + targetCubePosition + " in frame " + Time.frameCount);
		}
		if (currentPosition != targetPosition) {
			Vector3 avoidance = Vector3.zero;	
			RaycastHit hit;
			for (int i=0; i<RoomMesh.DIRECTIONS.Length; i++) {
				if (Physics.Raycast(position, RoomMesh.DIRECTIONS[i], out hit, RAYCAST_DISTANCE, Game.LAYER_MASK_ALL)) {
					avoidance += hit.normal * (RAYCAST_DISTANCE/hit.distance);
				}
			}
			Vector3 target = (cave.GetPositionFromGrid(targetPosition) - position).normalized;
			// if obstacle in target direction, get new target
			if (Physics.Raycast(position, target, out hit, RAYCAST_DISTANCE, Game.LAYER_MASK_MOVEABLES)) {
				targetPosition = play.cave.GetRandomEmptyGridPositionFrom(currentPosition, UnityEngine.Random.Range(minDistance,maxDistance+1));
			} else {
				rigidbody.AddForce((avoidance.normalized + target) * force);			
			}
//			Debug.Log (avoidance + " " + target);
		}
	}

	public void LookAt(Rigidbody rigidbody, Transform target, int minDistance, float angleForwardMax, ref float currentAngleUp) {
		Vector3 position = rigidbody.transform.position;
		if ( Vector3.Distance(cave.GetGridFromPosition(position).GetVector3(), cave.GetGridFromPosition(target.position).GetVector3()) <= minDistance ) {
			Vector3 toTarget = target.position - position;
			float angleUp = Vector3.Angle(rigidbody.transform.up, target.up);
			if (angleUp > 35.0f) {
				currentAngleUp = angleUp;
			}
			if (currentAngleUp > 5.0f) {
				rigidbody.AddTorque(Vector3.Cross(rigidbody.transform.up, target.up) * 10.0f);
				currentAngleUp = angleUp;
			} else {
				currentAngleUp = 0f;
			}
			float angleForward = Vector3.Angle(rigidbody.transform.forward, toTarget);
			if (angleForward > angleForwardMax) {
				rigidbody.AddTorque(Vector3.Cross(rigidbody.transform.forward, toTarget) * 10.0f);
			}
		}
	}
	
/*	public void LookAt(Rigidbody rigidbody, Transform target, int minDistance, float angleTolerance) {
		Vector3 position = rigidbody.transform.position;
		if ( Vector3.Distance(Cave.GetGridFromPosition(position).GetVector3(), Cave.GetGridFromPosition(target.position).GetVector3()) <= minDistance ) {
			Vector3 toTarget = target.position - position;
//			Vector3 tangent = Vector3.zero;
//			Vector3 binormal = Vector3.zero;
//			Vector3.OrthoNormalize(ref toTarget, ref tangent, ref binormal);
			float angleUp = Vector3.Angle(rigidbody.transform.up, target.up);
			if (angleUp > 30.0f) {
				rigidbody.AddTorque(Vector3.Cross(rigidbody.transform.up, target.up) * 10.0f);
			}
			float angleForward = Vector3.Angle(rigidbody.transform.forward, toTarget);
			if (angleForward > angleTolerance) {
				rigidbody.AddTorque(Vector3.Cross(rigidbody.transform.forward, toTarget) * 10.0f);
			}
		}*/
/*		Vector3 position = rigidbody.transform.position;
		if ( Vector3.Distance(Cave.GetGridFromPosition(position).GetVector3(), Cave.GetGridFromPosition(target.position).GetVector3()) <= minDistance ) {
			float angleUp = Vector3.Angle(rigidbody.transform.up, target.up);
			if (angleUp > angleTolerance) {
				rigidbody.AddTorque(Vector3.Cross(rigidbody.transform.up, -target.up) * 10.0f);
			}
			Vector3 toTarget = target.position-position;
			float angleForward = Vector3.Angle(rigidbody.transform.forward, toTarget);
			if (angleForward > angleTolerance) {
				rigidbody.AddTorque(Vector3.Cross(rigidbody.transform.forward, toTarget) * 1.0f);
			}
		}
	}*/
	
	// http://en.wikipedia.org/wiki/A*
	public void AStarPath(AStarThreadState aStarThreadState, GridPosition s, GridPosition g) {
		aStarThreadState.Start();
	UnityThreadHelper.TaskDistributor.Dispatch( () => {
			
		Dictionary<int, AStarNode> closedSet = new Dictionary<int, AStarNode>();
		Dictionary<int, AStarNode> openSet = new Dictionary<int, AStarNode>();
		Dictionary<int, int> cameFrom = new Dictionary<int, int>();
		
		AStarNode goal = new AStarNode(g, 0, 0);
		
		AStarNode startNode = new AStarNode(s, 0, AStarHeuristic(s, g));
		openSet.Add(startNode.GetHashCode(), startNode);
		
		while (openSet.Count > 0) {
//			Debug.Log ("here1");
			AStarNode current = AStarGetWithLowestFitness(openSet);
			if (current.position == goal.position) {
//				Debug.Log ("path FOUND!");// + (Time.realtimeSinceStartup-startTime));
				closedSet.Add(current.GetHashCode(), current);
				AStarReconstructPath(ref cameFrom, ref aStarThreadState.path, ref closedSet, goal.GetHashCode());
				aStarThreadState.Finish();
				return;
			}
			
//			Debug.Log ("testing current position and moving from open to closed / hash " + current.position + " / " + current.GetHashCode());
			openSet.Remove(current.GetHashCode());
			closedSet.Add(current.GetHashCode(), current);
			
			foreach (AStarNode n in AStarGetNeighbours(current)) {
				AStarNode neighbour = n;
				if (closedSet.ContainsKey(neighbour.GetHashCode())) {
//					Debug.Log ("neighbour in closed / hash " + neighbour.position +  " / " + neighbour.GetHashCode());
					continue;
				}
				float tentativeGoal = current.goal + AStarHeuristic(current, neighbour);
				if (!openSet.ContainsKey(neighbour.GetHashCode()) || tentativeGoal <= neighbour.goal) {
//					Debug.Log ("here2 " + closedSet.Count);
					neighbour.goal = tentativeGoal;
					neighbour.heuristic = AStarHeuristic(neighbour, goal);
					neighbour.fitness = neighbour.goal + neighbour.heuristic; //AStarHeuristic(neighbour, goal);
					cameFrom.Add(neighbour.GetHashCode(), current.GetHashCode());
					if (!openSet.ContainsKey(neighbour.GetHashCode())) {
//						Debug.Log ("neighbour added to open " + neighbour.position);
						openSet.Add(neighbour.GetHashCode(), neighbour);
					} else {
//						Debug.Log ("neighbour already in open " + neighbour.position);
					}
				} else {
//					Debug.Log ("neighbour not valid " + neighbour.position + " " + openSet.ContainsKey(neighbour.GetHashCode()) + " " + closedSet.ContainsKey(neighbour.GetHashCode()));
				}
			}
//			Debug.Log ("openSet count " + openSet.Count);
		}
//		Debug.Log ("Find nearest PATH location based on heuristic!");//+ (Time.realtimeSinceStartup-startTime));
		AStarChoosePathFromNearestNode(ref cameFrom, ref aStarThreadState.path, ref closedSet);
		aStarThreadState.Finish();
		return;
//		aStarThreadState.path.Clear();
//		aStarThreadState.Finish();
	});
	}
	
	private AStarNode AStarGetWithLowestFitness(Dictionary<int, AStarNode> nodeSet) {
		int key = 0;
		float fitness = 1000000f;
		foreach (KeyValuePair<int, AStarNode> n in nodeSet) {
			if (n.Value.fitness < fitness) {
				key = n.Value.GetHashCode();
				fitness = n.Value.fitness;
			}
		}
		return nodeSet[key];
	}
	
	private ArrayList AStarGetNeighbours(AStarNode n) {
		ArrayList neighbours = new ArrayList();
		for (int i=0; i<RoomMesh.DIRECTIONS.Length; i++) {
			try {
				GridPosition gridPosition = new GridPosition(new IntTriple(n.position + RoomMesh.DIRECTIONS[i]), n.gridPos.roomPosition);
				if (play.cave.GetCellDensity(gridPosition) == Cave.DENSITY_EMPTY) {
					neighbours.Add(new AStarNode(gridPosition, 0, 0));
				}
			} catch (IndexOutOfRangeException e) {
				Game.DefNull(e);
			}
		}		
//		Debug.Log ("getting neighbour list of " + neighbours.Count);
		return neighbours;
	}

	private void AStarChoosePathFromNearestNode(ref Dictionary<int, int> cameFrom, ref LinkedList<AStarNode> path, ref Dictionary<int, AStarNode> closedSet) {
		AStarNode nearestNode;
		int key = 0;
		float heuristic = 1000000f;
		foreach (KeyValuePair<int, AStarNode> n in closedSet) {
//			Debug.Log ("heuristic is " + n.Value.heuristic +  " " + closedSet[n.Value.GetHashCode()].gridPos);
			if (n.Value.heuristic < heuristic) {
				key = n.Value.GetHashCode();
				heuristic = n.Value.heuristic;
//				Debug.Log ("Lower heuristic " + closedSet[key].heuristic + " " + closedSet[key].gridPos);
			}
		}
		nearestNode = closedSet[key];
//		Debug.Log ("NearestNode " + nearestNode.gridPos);
		AStarReconstructPath(ref cameFrom, ref path, ref closedSet, nearestNode.GetHashCode());
	}
		
	private void AStarReconstructPath(ref Dictionary<int, int> cameFrom, ref LinkedList<AStarNode> path, ref Dictionary<int, AStarNode> closedSet, int current) {
		path.AddFirst(closedSet[current]);
		if (cameFrom.ContainsKey(current)) {
//			Debug.Log ("path consists of " + current);
			AStarReconstructPath(ref cameFrom, ref path, ref closedSet, cameFrom[current]);
		}
	}
	
	private float AStarHeuristic(GridPosition start, GridPosition goal) {
		return Vector3.Distance(start.GetVector3(), goal.GetVector3());
	}

	private float AStarHeuristic(AStarNode a, AStarNode b) {
		return Vector3.Distance(a.position, b.position);
	}
															
}









