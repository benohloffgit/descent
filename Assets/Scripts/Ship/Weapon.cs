using System;
using UnityEngine;
using System.Collections;

public class Weapon {
	
	public static int TYPE_GUN = 1;
	public static int TYPE_LASER = 2;
		
	public float lastShotTime;
	
	protected int type;
	protected int model;
	protected float accuracy;
	public float frequency;
	protected int damage;
	protected float speed;
	
	protected Vector3 position;
	
	private Play play;
	private Enemy enemy;
	private Transform weaponTransform;
		
	public Weapon(Enemy enemy_, int type_, int model_, Vector3 position_) {
		enemy = enemy_;
		play = enemy.play;
//		enemyTransform = enemy.transform;
		type = type_;
		model = model_;
		position = position_;
		lastShotTime = Time.time;
				
		Initialize();
	}
	
	public void Shoot() {
		play.Shoot(type, weaponTransform.position, weaponTransform.rotation, weaponTransform.forward, accuracy, speed, enemy.collider);
	}
	
	private void Initialize() {
		GameObject weaponGameObject;
		switch (type) {
			case 1:
				switch (model) {
					case 1:	accuracy = 4.0f; frequency = 2.0f; damage = 5; speed = 100f; break;
					case 2:	accuracy = 4.0f; frequency = 1.0f; damage = 5; speed = 100f; break;
				}
				weaponGameObject = enemy.enemyDistributor.CreateGun(); break;
			case 50:
				switch (model) {
					case 1: accuracy = 4.0f; frequency = 1.0f; damage = 10; speed = 400f; break;
				}
				weaponGameObject = enemy.enemyDistributor.CreateGun(); break;
			default:
				weaponGameObject = enemy.enemyDistributor.CreateGun(); break;
		}
		weaponTransform = weaponGameObject.transform;
//		weaponTransform.position = enemy.transform.position;
		weaponTransform.parent = enemy.transform;
		weaponTransform.localPosition = enemy.transform.InverseTransformPoint(position);
	}
	
}

