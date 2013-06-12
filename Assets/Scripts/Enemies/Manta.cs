using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Normal Roaming/Aiming behaviour according to generated model values
 * Chases ship
 * Dodges bullets
 */
public class Manta : Enemy {
	private RaycastHit hit;
	private GridPosition targetPosition;
	private Mode mode;
	private AStarThreadState aStarThreadState = new AStarThreadState();
	private bool isOnPath;
	private List<Shot> detectedShots;
	private Shot[,] evasionGrid;
	private IntDouble evasionGridPos;
	
	private static float DODGE_SPHERE_RADIUS = 2.5f;
	
	private static Vector3[] WEAPON_POSITIONS = new Vector3[] {new Vector3(0.53f, 0.63f, 0.750f), new Vector3(-0.53f, 0.63f, 0.750f), new Vector3(0, 0, 0)};
	private static Vector3[] WEAPON_ROTATIONS = new Vector3[] {new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0,0,0)};
	private static IntDouble[] EVASION_DIRECTIONS = new IntDouble[] {IntDouble.LEFT, IntDouble.UP, IntDouble.RIGHT, IntDouble.DOWN};
	
	public enum Mode { ROAMING=0, DODGEFINDING=1 }
	
	public override void InitializeWeapon(int mount, int type) {
		if (mount == Weapon.PRIMARY) {
			primaryWeapons.Add(new Weapon(this, mount, transform, play, type, WEAPON_POSITIONS,
					WEAPON_ROTATIONS, Game.ENEMY, spawn.isBoss));
		}
	}
	
	void Start() {
		detectedShots = new List<Shot>();
		InitializeEvasionGrid();
		targetPosition = cave.GetGridFromPosition(transform.position);
		mode = Mode.ROAMING;
		isOnPath = false;
		canBeDeactivated = false;
	}
	
	// scan for ship bullets with sphere cast between enemy and ship
	// if bullets found and source is ship make sphere cast for each of them between bullet and enemy in bullets direction
	// if hit store impact point
	// evade left right up down
	// check next closest bullet if it would have impact there, if so evade again
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (isShipVisible != Vector3.zero) {
			mode = Mode.DODGEFINDING;
			if (isShipVisible.magnitude <= shootingRange) {
				aggressiveness = Enemy.AGGRESSIVENESS_ON;
			}
		}

		if (mode == Mode.ROAMING) {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
		} else if (mode == Mode.DODGEFINDING) {
			RaycastHit[] hits = Physics.SphereCastAll(transform.position, DODGE_SPHERE_RADIUS, isShipVisible.normalized, isShipVisible.magnitude, 1 << Game.LAYER_BULLETS);
			foreach (RaycastHit h in hits) {
				Shot s = h.collider.GetComponent<Shot>();
				if (!detectedShots.Contains(s) && s.source == Game.SHIP) {
					if (Physics.SphereCast(s.transform.position, DODGE_SPHERE_RADIUS, s.transform.forward, out hit, isShipVisible.magnitude, 1 << Game.LAYER_ENEMIES)) {
						if (hit.collider.gameObject.GetInstanceID() == gameObject.GetInstanceID()) {
							//Debug.Log("Bullet detected on path towards me");
							detectedShots.Add(s);
							evasionGrid[evasionGridPos.x, evasionGridPos.y] = s;
							break;
						}
					}
				}
			}
		}
		if (evasionGrid[evasionGridPos.x, evasionGridPos.y] != null) { // evade this
			EvadeToNewGridPos();
			mode = Mode.ROAMING;
		}
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude),
				lookAtToleranceAiming, ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.None);
		} else {
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceAiming, ref currentAngleUp,
				ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
		}
		clazz = "Manta " + mode;
		
	}
	
	private void EvadeToNewGridPos() {
		List<IntDouble> directionPool = new List<IntDouble>();
		foreach (IntDouble dir in EVASION_DIRECTIONS) {
			if (evasionGrid[evasionGridPos.x+dir.x, evasionGridPos.y+dir.y] == null) {
				directionPool.Add(dir);
			}
		}
		if (directionPool.Count > 0) {
			IntDouble dir = directionPool[UnityEngine.Random.Range(0, directionPool.Count)];
			ShiftEvasionGrid(-1*dir);
			play.movement.MoveTo(myRigidbody, transform.TransformDirection(dir.GetVector3(0)), movementForce*50f);
		}
	}
	
	private void ShiftEvasionGrid(IntDouble offset) {
		Shot[,] newGrid = new Shot[5,5];
		int newX, newY;
		for (int x=0; x<5; x++) {
			for (int y=0; y<5; y++) {
				newX = x+offset.x;
				newY = y+offset.y;
				if (newX>=0 && newX<5 && newY>=0 && newY<5 && evasionGrid[x,y] != null) {
					newGrid[newX, newY] = evasionGrid[x,y];
				}
			}
		}
		evasionGrid = newGrid;
	}
	
	private void InitializeEvasionGrid() {
		evasionGrid = new Shot[5,5];
		evasionGridPos = new IntDouble(2,2); // center
	}
/*
  	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (isShipVisible != Vector3.zero && isShipVisible.magnitude <= shootingRange) {
			aggressiveness = Enemy.AGGRESSIVENESS_ON;
		}
		float distanceToShip = Vector3.Distance(transform.position, play.GetShipPosition());
			
		if (mode == Mode.PATHFINDING) {
			if (aStarThreadState.IsFinishedNow()) {
				aStarThreadState.Complete();
				mode = Mode.CHASING;
				isOnPath = false;
			}
		}
		if (mode == Mode.CHASING) {
			if (isOnPath) {
				play.movement.Chase(myRigidbody, currentGridPosition, targetPosition, movementForce, ref isOnPath);
			} else {
				if (distanceToShip > chasingRange) {
					if (aStarThreadState.roomPath.Count > 0) {
						LinkedListNode<AStarNode> n = aStarThreadState.roomPath.First;
						targetPosition = n.Value.gridPos;
						aStarThreadState.roomPath.RemoveFirst();
						isOnPath = true;
					} else {
						mode = Mode.ROAMING;
					}
				} else {
					mode = Mode.ROAMING;
				}
			}					
		}
		if (mode == Mode.ROAMING) {
			if (distanceToShip > shootingRange) {
				mode = Mode.PATHFINDING;
				play.movement.AStarPath(aStarThreadState, currentGridPosition, play.GetShipGridPosition());
			} else {
				play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
			}
		}
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			play.movement.LookAt(myRigidbody, play.ship.transform, Mathf.CeilToInt(isShipVisible.magnitude),
				lookAtToleranceAiming, ref currentAngleUp, ref dotProductLookAt, Movement.LookAtMode.None);
		} else {
			play.movement.LookAt(myRigidbody, play.ship.transform, lookAtRange, lookAtToleranceAiming, ref currentAngleUp,
				ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
		}
		clazz = "Manta " + mode;
		
	}*/

}

