using System;
using UnityEngine;
using System.Collections;

public class Weapon {
	
	public static int PRIMARY = 0;
	public static int SECONDARY = 1;
	
	public const int TYPE_GUN = 0;
	public const int TYPE_LASER = 1;
	public const int TYPE_TWIN_GUN = 2;
	public const int TYPE_PHASER = 3;
	public const int TYPE_TWIN_LASER = 4;
	public const int TYPE_GAUSS = 5;
	public const int TYPE_TWIN_PHASER = 6;
	public const int TYPE_TWIN_GAUSS = 7;
	
	public const int TYPE_MISSILE = 0;
	public const int TYPE_GUIDED_MISSILE = 1;
	public const int TYPE_CHARGED_MISSILE = 2;
	public const int TYPE_DETONATOR_MISSILE = 3;
	
	public const int TYPE_MINE_TOUCH = 4;
	public const int TYPE_MINE_SUICIDAL = 5;
//	public const int TYPE_MINE_INFRARED = 6;
//	public const int TYPE_MINE_TIMED = 7;
	public const int TYPE_LASER_BEAM = 8;
	
	private static float TWIN_WEAPON_DAMAGE_MODIFIER = 0.6f;
	
	public static int[] SHIP_PRIMARY_WEAPON_AVAILABILITY_MIN = new int[] {1,2,4,7,10,14,19,25}; //{1,4,8,13,20,30,42,58};
	public static int[] SHIP_PRIMARY_WEAPON_AVAILABILITY_MAX = new int[] {1,3,6,9,13,18,24,31}; //{1,7,12,19,29,41,57,63}
	//missile cap: 3, light 6, twin primary 7, speed 19, cloak 41
	public static int[] SHIP_SECONDARY_WEAPON_AVAILABILITY_MIN = new int[] { 3, 6, 13, 21 }; //{ 4, 9, 19, 31 }
	public static int[] SHIP_SECONDARY_WEAPON_AVAILABILITY_MAX = new int[] { 5, 12, 20, 28 }; //{ 8, 18, 30, 42 };
	public static int[] PRIMARY_DAMAGE = new int[] { 22, 25, 28, 30, 33, 36, 40, 43, 0, 0, 0, 0, 0, 0, 0, 0 };
	public static int[] SECONDARY_DAMAGE = new int[] { 65, 85, 100, 120, 90, 50, 0, 0, 30 };	
	public static float[] PRIMARY_SPEED = new float[] { 200f, 300f, 200f, 400f, 300f, 350f, 400f, 350f};
	public static float[] PRIMARY_ACCURACY = new float[] { 5.0f, 4.5f, 4.0f, 3.75f, 3.5f, 3.25f, 3.0f, 2.75f };
	public static float[] PRIMARY_FREQUENCY = new float[] { 4.0f, 3.75f, 3.5f, 3.25f, 3.0f, 2.75f, 2.5f, 2.25f };
	public static float[] SECONDARY_SPEED = new float[] { 50f, 50f, 50f, 50f, 0f, 0f, 0f, 0f, 0f };
	public static float[] SECONDARY_ACCURACY = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
	public static float[] SECONDARY_FREQUENCY = new float[] { 6.0f, 5.5f, 5f, 5f, 8f, 0f, 0f, 0f, 0f};
	public static string[] PRIMARY_TYPES = new string[] {"Gun", "Laser", "Twin Gun", "Phaser", "Twin Laser", "Gauss", "Twin Phaser", "Twin Gauss"};
	public static string[] SECONDARY_TYPES = new string[] {"Missile", "Guided Missile", "Charged Missile", "Detonator Missile", "Mine", "", "", "", "Laser Beam"};
	public static Vector3[] DETONATOR_BOMB_DIRECTIONS = new Vector3[] {new Vector3(0f,0.114f,-0.1f), new Vector3(0f,-0.114f,-0.1f),
		new Vector3(0.114f,0f,-0.1f), new Vector3(-0.114f,0f,-0.1f)};
	public static Vector3[] DETONATOR_EXPL_DIRECTIONS = new Vector3[] {new Vector3(0f,-0.114f,-0.1f), new Vector3(0f,0.114f,-0.1f),
		new Vector3(-0.114f,0f,-0.1f), new Vector3(0.114f,0f,-0.1f)};
	private static Vector3[] PRIMARY_WEAPON_CENTER_OFFSET = new Vector3[] { new Vector3(0f,0.026f,0f),new Vector3(0f,0.085f,0f),new Vector3(0f,0.057f,0f), new Vector3(0f,0.031f,0f) };
	
	public float lastShotTime;
	public Transform weaponTransform;
	public Transform[] subWeaponTransforms;
	
	public int type;
	public int mountAs;
	private int mountedTo;
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
	private bool isBoss;
	private bool isTwin;
		
	private Play play;
	private Game game;
	private Ship ship;
	private Enemy enemy;
	private Transform parent;
	private Transform aimingAnchor; // needed for wallgun
	private int layerMask;
	private RaycastHit hit;
	public Shot[] loadedShots;
	private Renderer[] myRenderers;
	private int myAudioSourceID = AudioSourcePool.NO_AUDIO_SOURCE;

	private static float MAX_RAYCAST_DISTANCE = Game.MAX_VISIBILITY_DISTANCE * 1.5f;
	
	public Weapon(Enemy parentEnemy_, int mountAs_, Transform parent_,
			Play play_, int type_, Vector3[] positions_, Vector3[] rotations_, int mountedTo_,
			bool isBoss_ = false, int ammunition_ = -1) {
		loadedShots = new Shot[2];
		mountAs = mountAs_;
		parent = parent_;
		play = play_;
		ship = play.ship;
		game = play.game;
		type = type_;
		isBoss = isBoss_;
		ammunition = ammunition_;
		positions = positions_;
		rotations = rotations_;
		lastShotTime = Time.fixedTime;
		mountedTo = mountedTo_;
		if (mountedTo == Game.ENEMY) {
			enemy = parentEnemy_;
		}
		isReloaded = false;
				
		Initialize();	
	}

	// Special constructor needed for WallGun
	public Weapon(Enemy parentEnemy_, int mountAs_, Transform parent_, Transform aimingAnchor_,
			Play play_, int type_, Vector3[] positions_, Vector3[] rotations_, int mountedTo_,
			bool isBoss_ = false, int ammunition_ = -1) :
				this(parentEnemy_, mountAs_, parent_, play_, type_, positions_, rotations_, mountedTo_, isBoss_, ammunition_)
	{
		aimingAnchor = aimingAnchor_;
	}
	
	private void Initialize() {
		GameObject weaponGameObject;
		if (mountAs == PRIMARY) {
			weaponGameObject = GameObject.Instantiate(game.primaryWeaponPrefabs[type]) as GameObject; 
			damage =  PRIMARY_DAMAGE[type];
		} else {
			weaponGameObject = GameObject.Instantiate(game.emptyPrefab) as GameObject;
			damage =  SECONDARY_DAMAGE[type];
		}
		weaponTransform = weaponGameObject.transform;
		weaponTransform.parent = parent.transform;

		isTwin = (mountAs == PRIMARY && (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS)) ? true : false;
		
		if (mountAs == PRIMARY) {
			if (type == TYPE_TWIN_GUN || type == TYPE_TWIN_LASER || type == TYPE_TWIN_PHASER || type == TYPE_TWIN_GAUSS) {
				subWeaponTransforms = new Transform[2];
				weaponTransform.localPosition = Vector3.zero;
				weaponTransform.localRotation = Quaternion.identity;
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
			speed = PRIMARY_SPEED[type];
			if (mountedTo == Game.ENEMY && enemy.clazzNum == Enemy.CLAZZ_WALLGUN14) {
				accuracy = 0f;
			} else {
				accuracy = PRIMARY_ACCURACY[type];
			}
			frequency = PRIMARY_FREQUENCY[type];
			myRenderers = weaponTransform.GetComponentsInChildren<Renderer>();
		} else {
			weaponTransform.localPosition = positions[Game.WEAPON_POSITION_CENTER];
			weaponTransform.localEulerAngles = rotations[Game.WEAPON_POSITION_CENTER];
			speed = SECONDARY_SPEED[type];
			accuracy = SECONDARY_ACCURACY[type];
			frequency = SECONDARY_FREQUENCY[type];
		}
		
		Unmount();

		if (mountedTo == Game.SHIP) {
			layerMask = Game.LAYER_MASK_ENEMIES_CAVE;
			if (isTwin) {
				subWeaponTransforms[0].gameObject.layer = Game.LAYER_GUN_SHIP;
				subWeaponTransforms[1].gameObject.layer = Game.LAYER_GUN_SHIP;
			} else {
				weaponTransform.gameObject.layer = Game.LAYER_GUN_SHIP;
			}
			accuracy = 0f;
			if (mountAs == Weapon.PRIMARY) {
				frequency = 0.2f;
			}
		} else {
			layerMask = Game.LAYER_MASK_SHIP_CAVE;
			if (isTwin) {
				subWeaponTransforms[0].gameObject.layer = Game.LAYER_GUN_ENEMY;
				subWeaponTransforms[1].gameObject.layer = Game.LAYER_GUN_ENEMY;
			} else {
				weaponTransform.gameObject.layer = Game.LAYER_GUN_ENEMY;
			}
			if (isBoss) {
				accuracy -= Enemy.BOSS_ACCURACY_MODIFIER;
				frequency -= Enemy.BOSS_FREQUENCY_MODIFIER;
				if (mountAs == Weapon.SECONDARY && (type == TYPE_MISSILE || type == TYPE_GUIDED_MISSILE)) {
					ammunition *= 2;
				}
			}
		}
	}
	
	public void Shoot() {
		//play.game.state.PlaySound(type);
		
		Vector3[] bulletPaths = new Vector3[2];
		
		if (mountedTo == Game.ENEMY) {
			myAudioSourceID = play.game.PlaySound(myAudioSourceID, enemy.transform, mountAs, type);
		} else {
			ship.PlaySound(mountAs, type);
		}
		
		if (mountedTo == Game.ENEMY && enemy.clazzNum == Enemy.CLAZZ_WALLGUN14) {
			bulletPaths[0] = aimingAnchor.forward;
			loadedShots[0].transform.localScale *= RoomMesh.MESH_SCALE/5f;
		} else {
			if (Physics.Raycast(parent.position, parent.forward, out hit, MAX_RAYCAST_DISTANCE, layerMask)) {
				if (isTwin) {
					bulletPaths[0] = (hit.point - subWeaponTransforms[0].position).normalized;
					bulletPaths[1] = (hit.point - subWeaponTransforms[1].position).normalized;
				} else {
					bulletPaths[0] = (hit.point - weaponTransform.position).normalized;
				}
			} else {
				if (isTwin) {
					bulletPaths[0] = subWeaponTransforms[0].forward;
					bulletPaths[1] = subWeaponTransforms[1].forward;
				} else {
					bulletPaths[0] = parent.forward;
				}
			}
		}

		if (accuracy != 0) {
			// improve accurcy the longer the ship stands still - 4seconds
			accuracy = Mathf.Max(1.0f, accuracy - (accuracy/240.0f) * (Time.fixedTime-ship.lastMoveTime) * 60.0f);
			
			Vector3.OrthoNormalize(ref bulletPaths[0], ref tangent, ref binormal);
			Quaternion deviation1 = Quaternion.AngleAxis(UnityEngine.Random.Range(0, accuracy) * Mathf.Sign(UnityEngine.Random.value-0.5f), tangent);
			Quaternion deviation2 = Quaternion.AngleAxis(UnityEngine.Random.Range(0, accuracy) * Mathf.Sign(UnityEngine.Random.value-0.5f), binormal);
			bulletPaths[0] = deviation1 * deviation2 * bulletPaths[0];
			if (isTwin) {
				bulletPaths[1] = deviation1 * deviation2 * bulletPaths[1];
			}
		}
		
		ConfigureLoadedShot(0);
		loadedShots[0].rigidbody.AddForce(bulletPaths[0] * speed);
		if (isTwin) {
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
		
		lastShotTime = Time.fixedTime;
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
			if (Time.fixedTime > lastShotTime + frequency && (ammunition == -1 || ammunition > 0)) {
				Reload();
				return true;
			}
		}
		return false;
	}
		
	public void Reload() {
		if (mountAs == PRIMARY) {
			if (type == TYPE_GUN) {
				loadedShots[0] = game.CreateFromPrefab().CreateGunShot(weaponTransform.position+weaponTransform.TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[TYPE_GUN]), weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_TWIN_GUN) {
				loadedShots[0] = game.CreateFromPrefab().CreateGunShot(subWeaponTransforms[0].position+subWeaponTransforms[0].TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[TYPE_GUN]), subWeaponTransforms[0].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
				loadedShots[1] = game.CreateFromPrefab().CreateGunShot(subWeaponTransforms[1].position+subWeaponTransforms[1].TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[TYPE_GUN]), subWeaponTransforms[1].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
			} else if (type == Weapon.TYPE_LASER) {
				loadedShots[0] = game.CreateFromPrefab().CreateLaserShot(weaponTransform.position+weaponTransform.TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[TYPE_LASER]), weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_TWIN_LASER) {
				loadedShots[0] = game.CreateFromPrefab().CreateLaserShot(subWeaponTransforms[0].position+subWeaponTransforms[0].TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[TYPE_LASER]), subWeaponTransforms[0].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
				loadedShots[1] = game.CreateFromPrefab().CreateLaserShot(subWeaponTransforms[1].position+subWeaponTransforms[1].TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[TYPE_LASER]), subWeaponTransforms[1].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
			} else if (type == Weapon.TYPE_PHASER) {
				loadedShots[0] = game.CreateFromPrefab().CreatePhaserShot(weaponTransform.position+weaponTransform.TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[2]), weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_TWIN_PHASER) {
				loadedShots[0] = game.CreateFromPrefab().CreatePhaserShot(subWeaponTransforms[0].position+subWeaponTransforms[0].TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[2]), subWeaponTransforms[0].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
				loadedShots[1] = game.CreateFromPrefab().CreatePhaserShot(subWeaponTransforms[1].position+subWeaponTransforms[1].TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[2]), subWeaponTransforms[1].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
			} else if (type == Weapon.TYPE_GAUSS) {
				loadedShots[0] = game.CreateFromPrefab().CreateGaussShot(weaponTransform.position+weaponTransform.TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[3]), weaponTransform.rotation, damage, mountedTo);
			} else { //type == Weapon.TYPE_TWIN_GAUSS)
				loadedShots[0] = game.CreateFromPrefab().CreateGaussShot(subWeaponTransforms[0].position+subWeaponTransforms[0].TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[3]), subWeaponTransforms[0].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
				loadedShots[1] = game.CreateFromPrefab().CreateGaussShot(subWeaponTransforms[1].position+subWeaponTransforms[1].TransformDirection(PRIMARY_WEAPON_CENTER_OFFSET[3]), subWeaponTransforms[1].rotation, (int)(damage*TWIN_WEAPON_DAMAGE_MODIFIER), mountedTo);
			}			
		} else {
			if (type == TYPE_MISSILE) {
				loadedShots[0] = game.CreateFromPrefab().CreateMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_GUIDED_MISSILE) {
				loadedShots[0] = game.CreateFromPrefab().CreateGuidedMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_CHARGED_MISSILE) {
				loadedShots[0] = game.CreateFromPrefab().CreateChargedMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
			} else if (type == Weapon.TYPE_DETONATOR_MISSILE) {
				loadedShots[0] = game.CreateFromPrefab().CreateDetonatorMissileShot(weaponTransform.position, weaponTransform.rotation, damage, mountedTo);
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
		if (isTwin) {
			loadedShots[0].transform.parent = subWeaponTransforms[0];
			loadedShots[1].transform.parent = subWeaponTransforms[1];
		} else {
			loadedShots[0].transform.parent = weaponTransform;
		}
		isReloaded = true;
	}
		
}

