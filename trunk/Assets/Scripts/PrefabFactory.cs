using System;
using UnityEngine;
using System.Collections;

public class PrefabFactory {
	private Game game;

	private GameObject gunBulletTemplate;
	private GameObject laserShotTemplate;
	
	public PrefabFactory(Game g) {
		game = g;
	}
	
	public void Initialize() {
		gunBulletTemplate = GameObject.Instantiate(game.gunBulletPrefab) as GameObject;
		gunBulletTemplate.GetComponent<Shot>().enabled = false;
		laserShotTemplate = GameObject.Instantiate(game.laserShotPrefab) as GameObject;
		laserShotTemplate.GetComponent<Shot>().enabled = false;
	}
	
	public GameObject CreateGunBullet(Vector3 pos, Quaternion rot) {
		GameObject newBullet = GameObject.Instantiate(gunBulletTemplate, pos, rot) as GameObject;
		Shot shot = newBullet.GetComponent<Shot>();
		shot.enabled = true;
		shot.shotType = Game.Shot.Bullet;
		return newBullet;
	}
	
	public GameObject CreateLaserShot(Vector3 pos, Quaternion rot) {
		GameObject newLaser = GameObject.Instantiate(laserShotTemplate, pos, rot) as GameObject;
		Shot shot = newLaser.GetComponent<Shot>();
		shot.enabled = true;
		shot.shotType = Game.Shot.Laser;
		return newLaser;
	}
}
