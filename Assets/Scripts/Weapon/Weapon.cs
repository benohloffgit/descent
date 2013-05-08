using System;
using UnityEngine;
using System.Collections;

public class Weapon {
	
	public static int PRIMARY = 0;
	public static int SECONDARY = 1;
	
	public const int TYPE_EMPTY = 0;
	
	public const int TYPE_GUN = 1;
	public const int TYPE_LASER = 2;
	public const int TYPE_TWIN_GUN = 3;
	public const int TYPE_PHASER = 4;
	public const int TYPE_TWIN_LASER = 5;
	public const int TYPE_GAUSS = 6;
	public const int TYPE_TWIN_PHASER = 7;
	public const int TYPE_TWIN_GAUSS = 8;
	
	public const int TYPE_MISSILE = 1;
	public const int TYPE_GUIDED_MISSILE = 2;
	public const int TYPE_CHARGED_MISSILE = 3;
	public const int TYPE_DETONATOR_MISSILE = 4;	
	public const int TYPE_MINE_TOUCH = 5;
	public const int TYPE_MINE_SUICIDAL = 6;
	public const int TYPE_MINE_INFRARED = 7;
	public const int TYPE_MINE_TIMED = 8;
	public const int TYPE_LASER_BEAM = 9;

	public static int MISSILE_START = 2;
	public static int MISSILE_GUIDED_START = 4;
	public static int MISSILE_CHARGED_START = 8; // charged from shield
	public static int MISSILE_DETONATOR_START = 16; // right click to exploded while flying
	
	public static string[] PRIMARY_TYPES = new string[] {"", "Gun", "Laser", "Twin Gun", "Phaser", "Twin Laser", "Gauss", "Twin Phaser", "Twin Gauss"};

	public static string[] SECONDARY_TYPES = new string[] {"", "Missile", "Guided Missile", "Charged Missile", "Detonator Missile", "Mine", "", "", "", "Laser Beam"};
	
	public float lastShotTime;
	public Transform weaponTransform;
	
	public int type;
	public int model;
	public int mountAs;
	protected float accuracy;
	public float frequency;
	public int damage;
	protected float speed;
	public int ammunition;
	
	protected Vector3 position;
	private Vector3 tangent = Vector3.zero;
	private Vector3 binormal = Vector3.zero;
	private bool isReloaded;
		
	private Play play;
	private Game game;
	private Ship ship;
	private Enemy enemy;
	private Transform parent;
	private int mountedTo;
	private int layerMask;
	private RaycastHit hit;
	public Shot loadedShot;

	public static int[] SHIP_PRIMARY_WEAPON_TYPES = new int[] { // per 5 zones, 64 = 320 zones
		1,1,2,1,1,2,3, 1,1,2,1,1,2,3,4, 2,2,3,2,2,3,4,5, 3,3,4,3,3,4,5,6, 4,4,5,4,4,5,6,7, 5,5,6,5,5,6,7,8, 6,6,7,6,6,7,8, 7,7,8,7,7,8, 8,8,8,8
	};
	public static int[] SHIP_PRIMARY_WEAPON_MODELS = new int[] { // per 5 zones, 64 = 320 zones
		1,2,1,3,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2, 5,6,3,7,8,4, 5,6,7,8,
	};
	public static int[] SHIP_SECONDARY_WEAPON_TYPES = new int[] {
		0,0,1,1,2,1,1, 2,3,1,1,2,1,1,2, 3,4,2,2,3,2,2,3, 4,1,3,3,4,3,3,4, 1,2,4,4,1,4,4,1, 2,3,1,1,2,1,1,2, 3,4,2,2,3,2,2, 3,4,3,3,4,3, 4,4,4,4
	};
	public static int[] SHIP_SECONDARY_WEAPON_MODELS = new int[] {
		0,0,1,2,1,3,4, 2,1,5,6,3,7,8,4, 2,1,5,6,3,7,8,4, 2,9,5,6,3,7,8,4, 10,9,5,6,11,7,8,12, 10,9,13,14,11,15,16,12, 10,9,13,14,11,15,16, 12,10,13,14,11,15, 12,13,14,15
	};

	private static float MAX_RAYCAST_DISTANCE = Game.MAX_VISIBILITY_DISTANCE * 1.5f;
	
	public Weapon(Enemy parentEnemy_, int mountAs_, Transform parent_,
			Play play_, int type_, int model_, Vector3 position_, int mountedTo_,
			float damageBase, bool isBoss = false, int ammunition_ = -1) {
		mountAs = mountAs_;
		parent = parent_;
		play = play_;
		ship = play.ship;
		game = play.game;
		type = type_;
		model = model_;
		ammunition = ammunition_;
		position = position_;
		lastShotTime = Time.time;
		mountedTo = mountedTo_;
		if (mountedTo == Game.ENEMY) {
			enemy = parentEnemy_;
		}
		isReloaded = false;
				
		Initialize();
		
		if (mountedTo == Game.SHIP) {
			layerMask = Game.LAYER_MASK_ENEMIES_CAVE;
			weaponTransform.gameObject.layer = Game.LAYER_GUN_SHIP;
			accuracy = 0f;
			damage =  Mathf.RoundToInt(damageBase * 1.5f);
			if (mountAs == Weapon.PRIMARY) {
				frequency = 0.2f;
			} else {
				damage *= 3;
			}
		} else {
			layerMask = Game.LAYER_MASK_SHIP_CAVE;
			weaponTransform.gameObject.layer = Game.LAYER_GUN_ENEMY;
			if (isBoss) {
				damageBase *= Enemy.BOSS_DAMAGE_MODIFIER;
				accuracy -= Enemy.BOSS_ACCURACY_MODIFIER;
				frequency -= Enemy.BOSS_FREQUENCY_MODIFIER;
				if (type == TYPE_MISSILE || type == TYPE_GUIDED_MISSILE) {
					ammunition *= 2;
				}
			}
			if (type == TYPE_MINE_TOUCH) {
				damage =  Mathf.RoundToInt(damageBase * 3.0f);
			} else if (type == TYPE_MISSILE || type == TYPE_GUIDED_MISSILE) {
				damage =  Mathf.RoundToInt(damageBase * 2.0f);
			} else {
				damage =  Mathf.RoundToInt(damageBase * 2.0f);
			}
		}
		
	}
	
	public void Shoot() {
		Vector3 bulletPath;
		if (Physics.Raycast(parent.position, parent.forward, out hit, MAX_RAYCAST_DISTANCE, layerMask)) {
			bulletPath = (hit.point - weaponTransform.position).normalized;
		} else {
			bulletPath = parent.forward;
		}
		
		loadedShot.enabled = true;
		loadedShot.rigidbody.isKinematic = false;
		loadedShot.rigidbody.AddForce(bulletPath * speed);
		loadedShot.transform.parent = null;
		
		if (accuracy != 0) {
			// improve accurcy the longer the ship stands still - 4seconds
			accuracy = Mathf.Max(1.0f, accuracy - (accuracy/240.0f) * (Time.time-ship.lastMoveTime) * 60.0f);
			
			Vector3.OrthoNormalize(ref bulletPath, ref tangent, ref binormal);
			Quaternion deviation1 = Quaternion.AngleAxis(UnityEngine.Random.Range(0, accuracy) * Mathf.Sign(UnityEngine.Random.value-0.5f), tangent);
			Quaternion deviation2 = Quaternion.AngleAxis(UnityEngine.Random.Range(0, accuracy) * Mathf.Sign(UnityEngine.Random.value-0.5f), binormal);
			bulletPath = deviation1 * deviation2 * bulletPath;
		}
		if (type == TYPE_MINE_TOUCH) {
			loadedShot.SetParentEnemy(enemy);
		} else {
			loadedShot.gameObject.layer = Game.LAYER_BULLETS;
			Physics.IgnoreCollision(parent.collider, loadedShot.collider);
		}
		
		lastShotTime = Time.time;
		isReloaded = false;
		if (ammunition > 0) {
			ammunition--;
		}

		if (type == TYPE_GUIDED_MISSILE && ship.missileLockMode == Ship.MissileLockMode.Locked) {
			loadedShot.LockOn(ship.lockedEnemy.transform);
		}
	}
		
	public void Mount() {
		if (mountAs == Weapon.PRIMARY) {
			weaponTransform.renderer.enabled = true;
		}
		if (mountAs == Weapon.SECONDARY && isReloaded) {
			loadedShot.transform.renderer.enabled = true;
		}
	}
	
	public void Unmount() {
		if (mountAs == Weapon.PRIMARY) {
			weaponTransform.renderer.enabled = false;
		}
		if (mountAs == Weapon.SECONDARY && isReloaded) {
			loadedShot.transform.renderer.enabled = false;
		}
	}
	
	public bool IsReloaded() {
		if (isReloaded) {
			return true;
		} else {
			if (Time.time > lastShotTime + frequency && (ammunition == -1 || ammunition > 0)) {
				Reload();
				return true;
			}
		}
		return false;
	}
		
	public void Reload() {
		if (mountAs == Weapon.PRIMARY) {
			if (type == Weapon.TYPE_GUN) {
				loadedShot = game.CreateFromPrefab().CreateGunShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_LASER) {
				loadedShot = game.CreateFromPrefab().CreateLaserShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else {
				loadedShot = game.CreateFromPrefab().CreateLaserShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			}			
		} else {
			if (type == Weapon.TYPE_MISSILE) {
				loadedShot = game.CreateFromPrefab().CreateMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_GUIDED_MISSILE) {
				loadedShot = game.CreateFromPrefab().CreateGuidedMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_MINE_TOUCH) {
				loadedShot = game.CreateFromPrefab().CreateMineTouchShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_LASER_BEAM) {
				loadedShot = game.CreateFromPrefab().CreateLaserBeamShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else {
				loadedShot = game.CreateFromPrefab().CreateMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			}
			if (loadedShot.rigidbody != null) {
				loadedShot.rigidbody.isKinematic = true;
			}
		}
		loadedShot.transform.parent = weaponTransform;
		isReloaded = true;
	}
	
	private void Initialize() {
		GameObject weaponGameObject;
		if (mountAs == PRIMARY) {
			switch (type) {
				case TYPE_GUN:
					speed = 100f;
					accuracy = 4.0f - model * 0.25f;
					frequency = 3.0f - model * 0.125f; break;		
				case TYPE_LASER:
					speed = 200f;
					accuracy = 3.0f - model * 0.1875f;
					frequency = 3.0f - model * 0.125f; break;
//					weaponGameObject = GameObject.Instantiate(game.laserGunPrefab) as GameObject; break;
				default:
					speed = 100f;
					accuracy = 4.0f - model * 0.1875f;
					frequency = 3.0f - model * 0.125f; break;
//					weaponGameObject = GameObject.Instantiate(game.gunPrefab) as GameObject; break;
			}
			weaponGameObject = GameObject.Instantiate(game.primaryWeaponPrefabs[type-1]) as GameObject; 
		} else {
			switch (type) {
				case TYPE_MISSILE:
					speed = 50f;
					accuracy = 0;//4.0f - model * 0.015f;
					frequency = 4.0f - model * 0.0625f; break;			
//					weaponGameObject = GameObject.Instantiate(game.missileLauncherPrefab) as GameObject; break;
				case TYPE_GUIDED_MISSILE:
					speed = 50f;
					accuracy = 0;//3.0f - model * 0.01f;
					frequency = 6.0f - model * 0.0625f; break;
//					weaponGameObject = GameObject.Instantiate(game.missileLauncherPrefab) as GameObject; break;
				case TYPE_MINE_TOUCH:
					speed = 0f;
					accuracy = 0;//3.0f - model * 0.01f;
					frequency = 10.0f - model * 0.0625f; break;
//					weaponGameObject = GameObject.Instantiate(game.missileLauncherPrefab) as GameObject; break;
				case TYPE_LASER_BEAM:
					speed = 0f;
					accuracy = 0;//3.0f - model * 0.01f;
					frequency = 0; break;
//					weaponGameObject = GameObject.Instantiate(game.missileLauncherPrefab) as GameObject; break;
				default:
					speed = 50f;
					accuracy = 0;//4.0f - model * 0.015f;
					frequency = 6.0f - model * 0.0625f; break;
//					weaponGameObject = GameObject.Instantiate(game.missileLauncherPrefab) as GameObject; break;
			}
			weaponGameObject = GameObject.Instantiate(game.emptyPrefab) as GameObject;
		}
				
		weaponTransform = weaponGameObject.transform;
		weaponTransform.parent = parent.transform;
		weaponTransform.localPosition = position;//parent.InverseTransformPoint(position);
		Unmount();
	}
	
}

