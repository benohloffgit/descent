using UnityEngine;
using System.Collections;

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
			targetCubePosition = play.room.GetRandomEmptyCubePositionFrom(cubePosition, Random.Range(minDistance,maxDistance+1));
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
				targetCubePosition = play.room.GetRandomEmptyCubePositionFrom(cubePosition, Random.Range(minDistance,maxDistance+1));
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
}









