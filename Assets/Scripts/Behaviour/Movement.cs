using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Movement {
	private Play play;
	
	private int layerMaskAll;
	private int layerMaskMoveables;
	
	private static float RAYCAST_DISTANCE = 2.0f;
	
	public Movement(Play p) {
		play = p;

		layerMaskAll = ( (1 << Game.LAYER_SHIP) | (1 << Game.LAYER_ENEMIES) | (1 << Game.LAYER_CAVE) );
		layerMaskMoveables = ( (1 << Game.LAYER_SHIP) | (1 << Game.LAYER_ENEMIES) );
	}
	
	public void Roam(Rigidbody rigidbody, ref Vector3 targetCubePosition, int minDistance, int maxDistance, float force) {
		Vector3 position = rigidbody.transform.position;
		Vector3 cubePosition = Room.GetCubePosition(position);
		if (cubePosition == targetCubePosition) {
			targetCubePosition = play.room.GetRandomEmptyCubePositionFrom(cubePosition, UnityEngine.Random.Range(minDistance,maxDistance+1));
//			Debug.Log ("current " + cubePosition + ", setting new target " + targetCubePosition + " in frame " + Time.frameCount);
		}
		if (cubePosition != targetCubePosition) {
			Vector3 avoidance = Vector3.zero;	
			RaycastHit hit;
			for (int i=0; i<Room.DIRECTIONS.Length; i++) {
				if (Physics.Raycast(position, Room.DIRECTIONS[i], out hit, RAYCAST_DISTANCE, layerMaskAll)) {
					avoidance += hit.normal * (RAYCAST_DISTANCE/hit.distance);
				}
			}
			Vector3 target = (Room.GetPositionFromCube(targetCubePosition) - position).normalized;
			// if obstacle in target direction, get new target
			if (Physics.Raycast(position, target, out hit, RAYCAST_DISTANCE, layerMaskMoveables)) {
				targetCubePosition = play.room.GetRandomEmptyCubePositionFrom(cubePosition, UnityEngine.Random.Range(minDistance,maxDistance+1));
			}
			rigidbody.AddForce((avoidance.normalized + target) * force);			
//			Debug.Log (avoidance + " " + target);
		}
	}
	
	public void LookAt(Rigidbody rigidbody, Transform target, int minDistance) {
		Vector3 position = rigidbody.transform.position;
		if ( Vector3.Distance(Room.GetCubePosition(position), Room.GetCubePosition(target.position)) <= minDistance ) {
//			Vector3 toTarget = (target.position - position).normalized;
			float angleForward = Vector3.Angle(rigidbody.transform.forward, -target.forward);
			if (angleForward > 30.0f) {
				rigidbody.AddTorque(Vector3.Cross(rigidbody.transform.forward, -target.forward) * 10.0f);
			}
			float angleUp = Vector3.Angle(rigidbody.transform.up, target.up);
			if (angleUp > 30.0f) {
				rigidbody.AddTorque(Vector3.Cross(rigidbody.transform.up, -target.up) * 10.0f);
			}
		}
	}
	
	private void AStarInitialization() {
	}

	// http://en.wikipedia.org/wiki/A*
	public LinkedList<AStarNode> AStarPath(EnemyDistributor.IntTriple s, EnemyDistributor.IntTriple g) {
		Dictionary<int, AStarNode> closedSet = new Dictionary<int, AStarNode>();
		Dictionary<int, AStarNode> openSet = new Dictionary<int, AStarNode>();
		Dictionary<int, int> cameFrom = new Dictionary<int, int>();
		LinkedList<AStarNode> path = new LinkedList<AStarNode>();
		
		AStarNode goal = new AStarNode(g, 0, 0);
		
		AStarNode startNode = new AStarNode(s, 0, AStarHeuristic(s, g));
		openSet.Add(startNode.GetHashCode(), startNode);
		
		while (openSet.Count > 0) {
			AStarNode current = AStarGetWithLowestFitness(openSet);
//			Debug.Log ("testing current position " + current.position);
			if (current.position == goal.position) {
				Debug.Log ("path found");
				closedSet.Add(current.GetHashCode(), current);
				AStarReconstructPath(ref cameFrom, ref path, ref closedSet, goal.GetHashCode());
/*				foreach (KeyValuePair<int, int> n in path) {
					Debug.Log (n.Key + " came from " + n.Value);
				}*/
				return path;
			}
			
			openSet.Remove(current.GetHashCode());
			closedSet.Add(current.GetHashCode(), current);
			
			foreach (AStarNode n in AStarGetNeighbours(current)) {
				AStarNode neighbour = n;
				if (closedSet.ContainsKey(neighbour.GetHashCode())) {
					continue;
				}
				float tentativeGoal = current.goal + AStarHeuristic(current, neighbour);
				if (!openSet.ContainsKey(neighbour.GetHashCode()) || tentativeGoal <= neighbour.goal) {
					neighbour.goal = tentativeGoal;
					neighbour.fitness = neighbour.goal + AStarHeuristic(neighbour, goal);
					cameFrom.Add(neighbour.GetHashCode(), current.GetHashCode());
					if (!openSet.ContainsKey(neighbour.GetHashCode())) {
						openSet.Add(neighbour.GetHashCode(), neighbour);
					}						
				}
			}
//			Debug.Log ("openSet count " + openSet.Count);
		}
		Debug.Log ("no path exists");
		return path;
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
		for (int x=-1; x<2; x++) {
			for (int y=-1; y<2; y++) {
				for (int z=-1; z<2; z++) {
					try {
						if (play.room.cubeDensity[n.intTriple.x+x,n.intTriple.y+y,n.intTriple.z+z] == CaveDigger.DENSITY_EMPTY) {
							if (!(x == 0 && y == 0 && z == 0)) {
								neighbours.Add(new AStarNode(new EnemyDistributor.IntTriple(n.intTriple.x+x,n.intTriple.y+y,n.intTriple.z+z), 0, 0));
							}
						}
					} catch (IndexOutOfRangeException e) {
					}
				}
			}
		}
//		Debug.Log ("getting neighbour list of " + neighbours.Count);
		return neighbours;
	}
	
	private void AStarReconstructPath(ref Dictionary<int, int> cameFrom, ref LinkedList<AStarNode> path, ref Dictionary<int, AStarNode> closedSet, int current) {
		if (cameFrom.ContainsKey(current)) {
//			Debug.Log ("path consists of " + current);
			path.AddFirst(closedSet[current]);
			AStarReconstructPath(ref cameFrom, ref path, ref closedSet, cameFrom[current]);
		} else {
//			Debug.Log ("last node of path " + current);
			path.AddFirst(closedSet[current]);
		}
	}
	
	private float AStarHeuristic(EnemyDistributor.IntTriple start, EnemyDistributor.IntTriple goal) {
		return Vector3.Distance(start.GetVector3(), goal.GetVector3());
	}

	private float AStarHeuristic(AStarNode a, AStarNode b) {
		return Vector3.Distance(a.position, b.position);
	}
	
	
															
}









