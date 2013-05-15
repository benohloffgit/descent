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
	
	private static float TWIN_WEAPON_DAMAGE_MODIFIER = 0.6f;
	
	public static string[] PRIMARY_TYPES = new string[] {"", "Gun", "Laser", "Twin Gun", "Phaser", "Twin Laser", "Gauss", "Twin Phaser", "Twin Gauss"};

	public static string[] SECONDARY_TYPES = new string[] {"", "Missile", "Guided Missile", "Charged Missile", "Detonator Missile", "Mine", "", "", "", "Laser Beam"};
	
	public float lastShotTime;
	public Transform weaponTransform;
	public Transform[] subWeaponTransforms;
	
	public int type;
	public int model;
	public int mountAs;
	protected float accuracy;
	public float frequency;
	public int damage;
	protected float speed;
	public int ammunition;
	
	protected Vector3[] positions;
	protected Vector3[] rotations;
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
	public Shot[] loadedShots;
	private Renderer[] myRenderers;

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
			Play play_, int type_, int model_, Vector3[] positions_, Vector3[] rotations_, int mountedTo_,
			float damageBase, bool isBoss = false, int ammunition_ = -1) {
		loadedShots = new Shot[2];
		mountAs = mountAs_;
		parent = parent_;
		play = play_;
		ship = play.ship;
		game = play.game;
		type = type_;
		model = model_;
		ammunition = ammunition_;
		positions = positions_;
		rotations = rotations_;
		lastShotTime = Time.time;
		mountedTo = mountedTo_;
		if (mountedTo == Game.ENEMY) {
			enemy = parentEnemy_;
		}
		isReloaded = false;
				
		Initialize();
		
		if (mountedTo == Game.SHIP) {
			layerMask = Game.LAYER_MASK_ENEMIES_CAVE;
			if (mountAs == Weapon.PRIMARY && (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS)) {
				subWeaponTransforms[0].gameObject.layer = Game.LAYER_GUN_SHIP;
				subWeaponTransforms[1].gameObject.layer = Game.LAYER_GUN_SHIP;
			} else {
				weaponTransform.gameObject.layer = Game.LAYER_GUN_SHIP;
			}
			accuracy = 0f;
			damage =  Mathf.RoundToInt(damageBase * 1.5f);
			if (mountAs == Weapon.PRIMARY) {
				frequency = 0.2f;
			} else {
				damage *= 3;
			}
		} else {
			layerMask = Game.LAYER_MASK_SHIP_CAVE;
			if (mountAs == Weapon.PRIMARY && (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS)) {
				subWeaponTransforms[0].gameObject.layer = Game.LAYER_GUN_ENEMY;
				subWeaponTransforms[1].gameObject.layer = Game.LAYER_GUN_ENEMY;
			} else {
				weaponTransform.gameObject.layer = Game.LAYER_GUN_ENEMY;
			}
			if (isBoss) {
				damageBase *= Enemy.BOSS_DAMAGE_MODIFIER;
				accuracy -= Enemy.BOSS_ACCURACY_MODIFIER;
				frequency -= Enemy.BOSS_FREQUENCY_MODIFIER;
				if (mountAs == Weapon.SECONDARY && (type == TYPE_MISSILE || type == TYPE_GUIDED_MISSILE)) {
					ammunition *= 2;
				}
			}
			if (mountAs == Weapon.SECONDARY && type == TYPE_MINE_TOUCH) {
				damage =  Mathf.RoundToInt(damageBase * 3.0f);
			} else if (mountAs == Weapon.SECONDARY && (type == TYPE_MISSILE || type == TYPE_GUIDED_MISSILE)) {
				damage =  Mathf.RoundToInt(damageBase * 2.0f);
			} else if (mountAs == Weapon.SECONDARY) {
				damage =  Mathf.RoundToInt(damageBase * 2.0f);
			}
		}
		
	}
	
	public void Shoot() {
		Vector3[] bulletPaths = new Vector3[2];
				
		if (Physics.Raycast(parent.position, parent.forward, out hit, MAX_RAYCAST_DISTANCE, layerMask)) {
			if (mountAs == PRIMARY && (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS)) {
				bulletPaths[0] = (hit.point - subWeaponTransforms[0].position).normalized;
				bulletPaths[1] = (hit.point - subWeaponTransforms[1].position).normalized;
			} else {
				bulletPaths[0] = (hit.point - weaponTransform.position).normalized;
			}
		} else {
			if (mountAs == PRIMARY && (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS)) {
				bulletPaths[0] = subWeaponTransforms[0].forward;
				bulletPaths[1] = subWeaponTransforms[1].forward;
			} else {
				bulletPaths[0] = parent.forward;
			}
		}

		if (accuracy != 0) {
			// improve accurcy the longer the ship stands still - 4seconds
			accuracy = Mathf.Max(1.0f, accuracy - (accuracy/240.0f) * (Time.time-ship.lastMoveTime) * 60.0f);
			
			Vector3.OrthoNormalize(ref bulletPaths[0], ref tangent, ref binormal);
			Quaternion deviation1 = Quaternion.AngleAxis(UnityEngine.Random.Range(0, accuracy) * Mathf.Sign(UnityEngine.Random.value-0.5f), tangent);
			Quaternion deviation2 = Quaternion.AngleAxis(UnityEngine.Random.Range(0, accuracy) * Mathf.Sign(UnityEngine.Random.value-0.5f), binormal);
			bulletPaths[0] = deviation1 * deviation2 * bulletPaths[0];
			if (mountAs == PRIMARY && (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS)) {
				bulletPaths[1] = deviation1 * deviation2 * bulletPaths[1];
			}
		}
		
		ConfigureLoadedShot(0);
		loadedShots[0].rigidbody.AddForce(bulletPaths[0] * speed);
		if (mountAs == PRIMARY && (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS)) {
			ConfigureLoadedShot(1);
			loadedShots[1].rigidbody.AddForce(bulletPaths[1] * speed);
			loadedShots[1].gameObject.layer = Game.LAYER_BULLETS;
			Physics.IgnoreCollision(parent.collider, loadedShots[1].collider);
		}
		
		if (mountAs == SECONDARY && type == TYPE_MINE_TOUCH) {
			loadedShots[0].SetParentEnemy(enemy);
		} else {
			loadedShots[0].gameObject.layer = Game.LAYER_BULLETS;
			Physics.IgnoreCollision(parent.collider, loadedShots[0].collider);
		}
		
		lastShotTime = Time.time;
		isReloaded = false;
		if (ammunition > 0) {
			ammunition--;
		}

		if (mountAs == SECONDARY && type == TYPE_GUIDED_MISSILE && ship.missileLockMode == Ship.MissileLockMode.Locked) {
			loadedShots[0].LockOn(ship.lockedEnemy.transform);
		}
	}
	
	private void ConfigureLoadedShot(int id) {
		loadedShots[id].enabled = true;
		loadedShots[id].rigidbody.isKinematic = false;
		loadedShots[id].transform.parent = null;
	}
		
	public void Mount() {
		if (mountAs == Weapon.PRIMARY) {
			foreach (Renderer r in myRenderers) {
				r.enabled = true;
			}
		}
		if (mountAs == Weapon.SECONDARY && isReloaded) {
			loadedShots[0].transform.renderer.enabled = true;
		}
	}
	
	public void Unmount() {
		if (mountAs == Weapon.PRIMARY) {
			foreach (Renderer r in myRenderers) {
				r.enabled = false;
			}
		}
		if (mountAs == Weapon.SECONDARY && isReloaded) {
			loadedShots[0].transform.renderer.enabled = false;
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
		if (mountAs == PRIMARY) {
			if (type == TYPE_GUN) {
				loadedShots[0] = game.CreateFromPrefab().CreateGunShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_LASER) {
				loadedShots[0] = game.CreateFromPrefab().CreateLaserShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_TWIN_GUN) {
				loadedShots[0] = game.CreateFromPrefab().CreateGunShot(subWeaponTransforms[0].position, subWeaponTransforms[0].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
				loadedShots[1] = game.CreateFromPrefab().CreateGunShot(subWeaponTransforms[1].position, subWeaponTransforms[1].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
			} else if (type == Weapon.TYPE_PHASER) {
				loadedShots[0] = game.CreateFromPrefab().CreatePhaserShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_TWIN_LASER) {
				loadedShots[0] = game.CreateFromPrefab().CreateLaserShot(subWeaponTransforms[0].position, subWeaponTransforms[0].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
				loadedShots[1] = game.CreateFromPrefab().CreateLaserShot(subWeaponTransforms[1].position, subWeaponTransforms[1].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
			} else {
				loadedShots[0] = game.CreateFromPrefab().CreateLaserShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			}			
		} else {
			if (type == TYPE_MISSILE) {
				loadedShots[0] = game.CreateFromPrefab().CreateMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_GUIDED_MISSILE) {
				loadedShots[0] = game.CreateFromPrefab().CreateGuidedMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_MINE_TOUCH) {
				loadedShots[0] = game.CreateFromPrefab().CreateMineTouchShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_LASER_BEAM) {
				loadedShots[0] = game.CreateFromPrefab().CreateLaserBeamShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else {
				loadedShots[0] = game.CreateFromPrefab().CreateMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			}
			if (loadedShots[0].rigidbody != null) {
				loadedShots[0].rigidbody.isKinematic = true;
			}
		}
		if (mountAs == PRIMARY && (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS)) {
			loadedShots[0].transform.parent = subWeaponTransforms[0];
			loadedShots[1].transform.parent = subWeaponTransforms[1];
		} else {
			loadedShots[0].transform.parent = weaponTransform;
		}
		isReloaded = true;
	}
	
	private void Initialize() {
		GameObject weaponGameObject;
		if (mountAs == PRIMARY) {
			weaponGameObject = GameObject.Instantiate(game.primaryWeaponPrefabs[type-1]) as GameObject; 
		} else {
			weaponGameObject = GameObject.Instantiate(game.emptyPrefab) as GameObject;
		}
		weaponTransform = weaponGameObject.transform;
		weaponTransform.parent = parent.transform;

		if (mountAs == PRIMARY) {
			if (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS) {
				subWeaponTransforms = new Transform[2];
				subWeaponTransforms[0] = weaponTransform.Find("Gun1");
				subWeaponTransforms[1] = weaponTransform.Find("Gun2");
				subWeaponTransforms[0].localPosition = positions[Game.WEAPON_POSITION_WING_LEFT];
				subWeaponTransforms[1].localPosition = positions[Game.WEAPON_POSITION_WING_RIGHT];
				subWeaponTransforms[0].localEulerAngles = rotations[Game.WEAPON_POSITION_WING_LEFT];
				subWeaponTransforms[1].localEulerAngles = rotations[Game.WEAPON_POSITION_WING_RIGHT];
			} else {
				weaponTransform.localPosition = positions[Game.WEAPON_POSITION_WING_LEFT];
				weaponTransform.localEulerAngles = rotations[Game.WEAPON_POSITION_WING_LEFT];
			}
			switch (type) {
				case TYPE_GUN:
					speed = 100f;
					accuracy = 4.0f - model * 0.25f;
					frequency = 3.0f - model * 0.125f; break;		
				case TYPE_LASER:
					speed = 200f;
					accuracy = 3.0f - model * 0.1875f;
					frequency = 3.0f - model * 0.125f; break;
				case TYPE_TWIN_GUN:
					speed = 100f;
					accuracy = 3.7f - model * 0.25f;
					frequency = 3.0f - model * 0.125f; break;		
				case TYPE_PHASER:
					speed = 250f;
					accuracy = 3.0f - model * 0.1875f;
					frequency = 2.7f - model * 0.125f; break;
				default:
					speed = 100f;
					accuracy = 4.0f - model * 0.1875f;
					frequency = 3.0f - model * 0.125f; break;
			}
			myRenderers = weaponTransform.GetComponentsInChildren<Renderer>();
		} else {
			weaponTransform.localPosition = positions[Game.WEAPON_POSITION_CENTER];
			weaponTransform.localEulerAngles = rotations[Game.WEAPON_POSITION_CENTER];
			switch (type) {
				case TYPE_MISSILE:
					speed = 50f;
					accuracy = 0;//4.0f - model * 0.015f;
					frequency = 4.0f - model * 0.0625f; break;			
				case TYPE_GUIDED_MISSILE:
					speed = 50f;
					accuracy = 0;//3.0f - model * 0.01f;
					frequency = 6.0f - model * 0.0625f; break;
				case TYPE_MINE_TOUCH:
					speed = 0f;
					accuracy = 0;//3.0f - model * 0.01f;
					frequency = 10.0f - model * 0.0625f; break;
				case TYPE_LASER_BEAM:
					speed = 0f;
					accuracy = 0;//3.0f - model * 0.01f;
					frequency = 0; break;
				default:
					speed = 50f;
					accuracy = 0;//4.0f - model * 0.015f;
					frequency = 6.0f - model * 0.0625f; break;
				
			}
		}
		Unmount();
	}
	
}

