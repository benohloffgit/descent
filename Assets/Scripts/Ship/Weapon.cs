using System;
using UnityEngine;
using System.Collections;

public class Weapon {
	
	public static int TYPE_GUN = 1;
	public static int TYPE_LASER = 2;
	public static int TYPE_PHASER = 2;
	public static int TYPE_GAUSS = 3;
		
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
		play.Shoot(type, weaponTransform.position, weaponTransform.rotation, bulletPath, accuracy, speed, damage, parent.collider);
	}
	
	private void Initialize() {
		GameObject weaponGameObject;
		switch (type) {
			case 1:
				switch (model) {
					case 1:	accuracy = 4.0f; frequency = 2.0f; damage = 5; speed = 100f; break;
					case 2:	accuracy = 4.0f; frequency = 1.0f; damage = 5; speed = 100f; break;
					case 9:	accuracy = 4.0f; frequency = 0.2f; damage = 5; speed = 100f; break;
				}
				weaponGameObject = GameObject.Instantiate(game.gunPrefab) as GameObject; break;
			case 2:
				switch (model) {
					case 1:	accuracy = 3.0f; frequency = 2.0f; damage = 10; speed = 200f; break;
					case 2:	accuracy = 3.0f; frequency = 1.0f; damage = 10; speed = 200f; break;
					case 9:	accuracy = 3.0f; frequency = 0.2f; damage = 10; speed = 200f; break;
				}
				weaponGameObject = GameObject.Instantiate(game.laserGunPrefab) as GameObject; break;
			default:
				weaponGameObject = GameObject.Instantiate(game.gunPrefab) as GameObject; break;
		}
		weaponTransform = weaponGameObject.transform;
		weaponTransform.parent = parent.transform;
		weaponTransform.localPosition = parent.InverseTransformPoint(position);
	}
	
}

