using System;
using UnityEngine;
using System.Collections;

public class Weapon {
	
	public const int TYPE_GUN = 1;
	public const int TYPE_LASER = 2;
	public const int TYPE_TWIN_GUN = 3;
	public const int TYPE_PHASER = 4;
	public const int TYPE_TWIN_LASER = 5;
	public const int TYPE_GAUSS = 6;
	public const int TYPE_TWIN_PHASER = 7;
	public const int TYPE_TWIN_GAUSS = 8;
	
	public const int TYPE_MISSILE = 9;
	public const int TYPE_GUIDED_MISSILE = 10;
	public const int TYPE_CHARGED_MISSILE = 11;
	
	public const int TYPE_MINE_SUICIDAL = 12;
	public const int TYPE_MINE_TOUCH = 13;
	public const int TYPE_MINE_INFRARED = 14;
	public const int TYPE_MINE_TIMED = 15;
	public const int TYPE_LASER_BEAM = 16;
	
		
	public float lastShotTime;
	public Transform weaponTransform;
	
	protected int type;
	protected int model;
	protected float accuracy;
	public float frequency;
	public int damage;
	protected float speed;
	
	protected Vector3 position;
	
	private Play play;
	private Game game;
	private Transform parent;
	private int mountedTo;
	private int layerMask;
	private RaycastHit hit;

	public static int[] SHIP_PRIMARY_WEAPON_TYPES = new int[] { // per 5 zones, 64 = 320 zones
		1,1,2,1,1,2,3, 1,1,2,1,1,2,3,4, 2,2,3,2,2,3,4,5, 3,3,4,3,3,4,5,6, 4,4,5,4,4,5,6,7, 5,5,6,5,5,6,7,8, 6,6,7,6,6,7,8, 7,7,8,7,7,8, 8,8,8,8
	};
	public static int[] SHIP_PRIMARY_WEAPON_MODELS = new int[] { // per 5 zones, 64 = 320 zones
		1,2,1,3,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2,1, 5,6,3,7,8,4,2, 5,6,3,7,8,4, 5,6,7,8,
	};

	private static float MAX_RAYCAST_DISTANCE = Game.MAX_VISIBILITY_DISTANCE * 1.5f;
	
	public Weapon(Transform parent_, Play play_, int type_, int model_, Vector3 position_, int mountedTo_) {
		parent = parent_;
		play = play_;
		game = play.game;
		type = type_;
		model = model_;
		position = position_;
		lastShotTime = Time.time;
		mountedTo = mountedTo_;		
				
		Initialize();
		
		if (mountedTo == Game.SHIP) {
			layerMask = Game.LAYER_MASK_ENEMIES_CAVE;
			weaponTransform.gameObject.layer = Game.LAYER_GUN_SHIP;
			accuracy = 0f;
			frequency = 0.2f;
		} else {
			layerMask = Game.LAYER_MASK_SHIP_CAVE;
			weaponTransform.gameObject.layer = Game.LAYER_GUN_ENEMY;
		}
	}
	
	public void Shoot() {
		Vector3 bulletPath;
		if (Physics.Raycast(parent.position, parent.forward, out hit, MAX_RAYCAST_DISTANCE, layerMask)) {
			bulletPath = (hit.point - weaponTransform.position).normalized;
		} else {
			bulletPath = parent.forward;
		}
		play.Shoot(type, weaponTransform.position, weaponTransform.rotation, bulletPath, accuracy, speed, damage, parent.collider, mountedTo);
	}
	
	private void Initialize() {
		GameObject weaponGameObject;
		switch (type) {
			case TYPE_GUN:
				speed = 100f;
				accuracy = 4.0f - model * 0.015f;
				frequency = 3.0f - model * 0.02f;			
				weaponGameObject = GameObject.Instantiate(game.gunPrefab) as GameObject; break;
			case TYPE_LASER:
				speed = 200f;
				accuracy = 3.0f - model * 0.01f;
				frequency = 3.0f - model * 0.02f;
				weaponGameObject = GameObject.Instantiate(game.laserGunPrefab) as GameObject; break;
			default:
				speed = 100f;
				accuracy = 4.0f - model * 0.015f;
				frequency = 3.0f - model * 0.02f;
				weaponGameObject = GameObject.Instantiate(game.gunPrefab) as GameObject; break;
		}
		damage =  Mathf.RoundToInt(Zone.GetZone5StepID(play.zoneID)  * 12.5f + 2.5f);
				
		weaponTransform = weaponGameObject.transform;
		weaponTransform.parent = parent.transform;
		weaponTransform.localPosition = parent.InverseTransformPoint(position);
	}

}

