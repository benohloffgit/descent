using System;
using UnityEngine;
using System.Collections;

public class PrefabFactory {
	private Game game;
	private Play play;

	private GameObject gunBulletTemplate;
	private GameObject laserShotTemplate;
	private GameObject missileShotTemplate;
	private GameObject breadcrumbTemplate;
	private GameObject explosionTemplate;
	private GameObject hitTemplate;
	private GameObject healthTemplate;
	private GameObject shieldTemplate;
	
	public PrefabFactory(Game g) {
		game = g;
	}
	
	public void Initialize(Play p) {
		play = p;
		gunBulletTemplate = GameObject.Instantiate(game.gunBulletPrefab) as GameObject;
		gunBulletTemplate.GetComponent<Shot>().enabled = false;
		laserShotTemplate = GameObject.Instantiate(game.laserShotPrefab) as GameObject;
		laserShotTemplate.GetComponent<Shot>().enabled = false;
		missileShotTemplate = GameObject.Instantiate(game.missileShotPrefab) as GameObject;
		missileShotTemplate.GetComponent<Shot>().enabled = false;
		breadcrumbTemplate = GameObject.Instantiate(game.breadcrumbPrefab) as GameObject;
		explosionTemplate = GameObject.Instantiate(game.explosionPrefab) as GameObject;
		explosionTemplate.GetComponent<Explosion>().enabled = false;
		hitTemplate = GameObject.Instantiate(game.hitPrefab) as GameObject;
		hitTemplate.GetComponent<Hit>().enabled = false;
		healthTemplate = GameObject.Instantiate(game.healthPrefab) as GameObject;
		shieldTemplate = GameObject.Instantiate(game.shieldPrefab) as GameObject;
	}
	
	public Shot CreateGunShot(Vector3 pos, Quaternion rot, int damage, int source) {
		GameObject newBullet = GameObject.Instantiate(gunBulletTemplate, pos, rot) as GameObject;
		Shot shot = newBullet.GetComponent<Shot>();
		shot.Initialize(play, damage, source, Shot.BULLET);
		shot.enabled = false;
		return shot;
	}
	
	public Shot CreateLaserShot(Vector3 pos, Quaternion rot, int damage, int source) {
		GameObject newLaser = GameObject.Instantiate(laserShotTemplate, pos, rot) as GameObject;
		Shot shot = newLaser.GetComponent<Shot>();
		shot.Initialize(play, damage, source, Shot.BULLET);
		shot.enabled = false;
		return shot;
	}

	public Shot CreateMissileShot(Vector3 pos, Quaternion rot, int damage, int source) {
		GameObject newMissile = GameObject.Instantiate(missileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, source, Shot.MISSILE);
		shot.enabled = false;
		return shot;
	}
	
	public GameObject CreateBreadcrumb(Vector3 pos, Quaternion rot) {
		GameObject newBreadcrumb = GameObject.Instantiate(breadcrumbTemplate, pos, rot) as GameObject;
		return newBreadcrumb;
	}

	public GameObject CreateHealth(Vector3 pos, Quaternion rot, int amount) {
		GameObject newHealth = GameObject.Instantiate(healthTemplate, pos, rot) as GameObject;
		CollecteableHealth health = newHealth.GetComponent<CollecteableHealth>();
		health.Initialize(play, amount);
		return newHealth;
	}

	public GameObject CreateShield(Vector3 pos, Quaternion rot, int amount) {
		GameObject newShield = GameObject.Instantiate(shieldTemplate, pos, rot) as GameObject;
		CollecteableShield shield = newShield.GetComponent<CollecteableShield>();
		shield.Initialize(play, amount);
		return newShield;
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

