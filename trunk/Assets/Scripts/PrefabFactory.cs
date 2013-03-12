using System;
using UnityEngine;
using System.Collections;

public class PrefabFactory {
	private Game game;
	private Play play;

	private GameObject gunBulletTemplate;
	private GameObject laserShotTemplate;
	private GameObject breadcrumbTemplate;
	private GameObject explosionTemplate;
	private GameObject hitTemplate;
	
	public PrefabFactory(Game g) {
		game = g;
	}
	
	public void Initialize(Play p) {
		play = p;
		gunBulletTemplate = GameObject.Instantiate(game.gunBulletPrefab) as GameObject;
		gunBulletTemplate.GetComponent<Shot>().enabled = false;
		laserShotTemplate = GameObject.Instantiate(game.laserShotPrefab) as GameObject;
		laserShotTemplate.GetComponent<Shot>().enabled = false;
		breadcrumbTemplate = GameObject.Instantiate(game.breadcrumbPrefab) as GameObject;
		explosionTemplate = GameObject.Instantiate(game.explosionPrefab) as GameObject;
		explosionTemplate.GetComponent<Explosion>().enabled = false;
		hitTemplate = GameObject.Instantiate(game.hitPrefab) as GameObject;
		hitTemplate.GetComponent<Hit>().enabled = false;
	}
	
	public GameObject CreateGunBullet(Vector3 pos, Quaternion rot, int damage) {
		GameObject newBullet = GameObject.Instantiate(gunBulletTemplate, pos, rot) as GameObject;
		Shot shot = newBullet.GetComponent<Shot>();
		shot.Initialize(play, damage);
		shot.enabled = true;
		shot.shotType = Game.Shot.Bullet;
		return newBullet;
	}
	
	public GameObject CreateLaserShot(Vector3 pos, Quaternion rot, int damage) {
		GameObject newLaser = GameObject.Instantiate(laserShotTemplate, pos, rot) as GameObject;
		Shot shot = newLaser.GetComponent<Shot>();
		shot.enabled = true;
		shot.Initialize(play, damage);
		shot.shotType = Game.Shot.Laser;
		return newLaser;
	}
	
	public GameObject CreateBreadcrumb(Vector3 pos, Quaternion rot) {
		GameObject newBreadcrumb = GameObject.Instantiate(breadcrumbTemplate, pos, rot) as GameObject;
		return newBreadcrumb;
	}

	public GameObject CreateExplosion(Vector3 pos, Quaternion rot) {
		GameObject newExplosion = GameObject.Instantiate(explosionTemplate, pos, rot) as GameObject;
		Explosion explosion = newExplosion.GetComponent<Explosion>();
		explosion.enabled = true;
		explosion.Initialize(play);
		return newExplosion;
	}

	public GameObject CreateHit(Vector3 pos, Quaternion rot) {
		GameObject newHit = GameObject.Instantiate(hitTemplate, pos, rot) as GameObject;
		Hit hit = newHit.GetComponent<Hit>();
		hit.enabled = true;
		hit.Initialize(play);
		return newHit;
	}
	
}

