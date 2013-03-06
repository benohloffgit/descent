using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {
	public static string TAG = "Enemy";
	
	public static string CLAZZ_A = "a";
	public static string CLAZZ_B = "b";
	public static string CLAZZ_C = "c";
	
	public Game game;
	public Play play;
	
	public string clazz;
	public int number;
	public int health;
	public int shield;
	public float size;
	public float aggressiveness; // between 0 (no at all) and 1.0 (attacks 100% of time)
	public float movementForce;
	public float turningForce;
	public int lookAtRange;
	public float lookAtToleranceAiming;
	public float lookAtToleranceRoaming;
	public float chaseRange;
	public float shootingRange;
		
	public void Initialize(Play play_, string clazz_, int number_, int health_, int shield_, float size_, float aggressiveness_, float movementForce_,
			float turningForce_, int lookAtRange_, float lookAtToleranceAiming_, float lookAtToleranceRoaming_, float chaseRange_) {
		play = play_;
		game = play.game;
		clazz = clazz_;
		number = number_;
		health = health_;
		shield = shield_;
		size = size_;
		aggressiveness = aggressiveness_;
		movementForce = movementForce_;
		turningForce = turningForce_;
		lookAtRange = lookAtRange_;
		lookAtToleranceAiming = lookAtToleranceAiming_;
		lookAtToleranceRoaming = lookAtToleranceRoaming_;
		chaseRange = chaseRange_;
		
		// derived values
		shootingRange = RoomMesh.MESH_SCALE * lookAtRange;
		
		transform.localScale *= size;
	}
	
	public void Damage(int damage, Vector3 contactPos) {
		health -= damage;
		play.DisplayHit(contactPos, play.ship.transform.rotation);
		if (health <= 0) {
			Destroy(gameObject);
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
		}
	}
	

}
