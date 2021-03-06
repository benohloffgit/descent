using UnityEngine;
using System.Collections;

public class Hornet : Enemy {	
	private GridPosition targetPosition;
	private Mode mode;
	private int damage;
	private float soundPlayedTime;

	public enum Mode { ROAMING=0, ATTACKING=1 }

	private static float SOUND_PLAY_DELTA = 2.5f;

	private int myAudioSourceID = AudioSourcePool.NO_AUDIO_SOURCE;
	
	void Start() {
		mode = Mode.ROAMING;
		soundPlayedTime = Time.fixedTime;
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
			if (Time.fixedTime > soundPlayedTime + SOUND_PLAY_DELTA) {
				soundPlayedTime = Time.fixedTime;
				myAudioSourceID = play.game.PlaySound(myAudioSourceID, transform, Game.SOUND_TYPE_VARIOUS, 64);
			}
		} else {
			play.movement.Roam(myRigidbody, currentGridPosition, ref targetPosition, roamMinRange, roamMaxRange, movementForce);
		}
		
		play.movement.LookAt(myRigidbody, play.ship.transform, 0, 0, ref currentAngleUp,
			ref dotProductLookAt, Movement.LookAtMode.IntoMovingDirection);
	}

	void OnCollisionEnter(Collision c) {
		if (c.collider.tag == Ship.TAG) {
			play.DisplayExplosion(transform.position, play.ship.transform.rotation);
			//c.rigidbody.AddExplosionForce(MINE_EXPLOSION_POWER, transform.position, MINE_EXPLOSION_RADIUS);
			play.ship.Damage(damage, c.contacts[0].point, Shot.MINE_TOUCH);
			play.DamageShip(Game.ENEMY);
			play.RemoveEnemyOnDeath(this);
			Destroy(gameObject);
		}
	}
}

