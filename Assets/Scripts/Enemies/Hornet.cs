using UnityEngine;
using System.Collections;

public class Hornet : Enemy {	
	private GridPosition targetPosition;
	private Mode mode;
	private float currentAngleUp;
	private int damage;

	public enum Mode { ROAMING=0, ATTACKING=1 }
	
	void Start() {
		mode = Mode.ROAMING;
	}
	
	public override void InitializeWeapon(int mount, int type) {
		damage = Weapon.SECONDARY_DAMAGE[type];
	}
		
	public override void DispatchFixedUpdate(Vector3 isShipVisible) {
		if (isShipVisible != Vector3.zero) {
			aggressiveness = Enemy.AGGRESSIVENESS_ON;
		}
		
		if (aggressiveness > Enemy.AGGRESSIVENESS_OFF) {
			mode = Mode.ATTACKING;
		} else {
			mode = Mode.ROAMING;
		}
		
		if (mode == Mode.ATTACKING) {
			Vector3 direction = (play.GetShipPosition() - transform.position).normalized;
			play.movement.MoveTo(myRigidbody, direction, movementForce);
		} else {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
		}
		
		play.movement.LookAt(myRigidbody, play.ship.transform, 0, 0, ref currentAngleUp, Movement.LookAtMode.IntoMovingDirection);
	}

	void OnCollisionEnter(Collision c) {
		if (c.collider.tag == Ship.TAG) {
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
			//c.rigidbody.AddExplosionForce(MINE_EXPLOSION_POWER, transform.position, MINE_EXPLOSION_RADIUS);
			play.ship.Damage(damage);
			play.DamageShip(Game.ENEMY);
			Destroy(gameObject);
		}
	}
}

