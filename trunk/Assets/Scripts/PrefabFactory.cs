using System;
using UnityEngine;
using System.Collections;

public class PrefabFactory {
	private Game game;
	private Play play;

	private GameObject gunBulletTemplate;
	private GameObject laserShotTemplate;
	private GameObject phaserShotTemplate;
	private GameObject missileShotTemplate;
	private GameObject breadcrumbTemplate;
	private GameObject explosionTemplate;
	private GameObject hitTemplate;
	private GameObject healthDropTemplate;
	private GameObject shieldDropTemplate;
	private GameObject missileDropTemplate;
	private GameObject mineTouchTemplate;
	
	public PrefabFactory(Game g) {
		game = g;
	}
	
	public void Initialize(Play p) {
		play = p;
		gunBulletTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.BULLET]) as GameObject;
		gunBulletTemplate.GetComponent<Shot>().enabled = false;
		laserShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.LASER]) as GameObject;
		laserShotTemplate.GetComponent<Shot>().enabled = false;
		phaserShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.PHASER]) as GameObject;
		phaserShotTemplate.GetComponent<Shot>().enabled = false;
		missileShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.MISSILE]) as GameObject;
		missileShotTemplate.GetComponent<Shot>().enabled = false;
		breadcrumbTemplate = GameObject.Instantiate(game.breadcrumbPrefab) as GameObject;
		breadcrumbTemplate.GetComponent<Breadcrumb>().enabled = false;
		explosionTemplate = GameObject.Instantiate(game.explosionPrefab) as GameObject;
		explosionTemplate.GetComponent<Explosion>().enabled = false;
		hitTemplate = GameObject.Instantiate(game.hitPrefab) as GameObject;
		hitTemplate.GetComponent<Hit>().enabled = false;
		healthDropTemplate = GameObject.Instantiate(game.healthDropPrefab) as GameObject;
		healthDropTemplate.GetComponent<CollecteableHealth>().enabled = false;
		healthDropTemplate.tag = ""; // so it is not removed at the end of zone
		shieldDropTemplate = GameObject.Instantiate(game.shieldDropPrefab) as GameObject;
		shieldDropTemplate.GetComponent<CollecteableShield>().enabled = false;
		shieldDropTemplate.tag = "";
		missileDropTemplate = GameObject.Instantiate(game.missileDropPrefab) as GameObject;
		missileDropTemplate.GetComponent<CollecteableMissile>().enabled = false;
		missileDropTemplate.tag = "";
		mineTouchTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.MINE_TOUCH]) as GameObject;
		mineTouchTemplate.GetComponent<Shot>().enabled = false;
	}
	
	public Shot CreateGunShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newBullet = GameObject.Instantiate(gunBulletTemplate, pos, rot) as GameObject;
		Shot shot = newBullet.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.BULLET);
		shot.enabled = true;
		return shot;
	}
	
	public Shot CreateLaserShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newLaser = GameObject.Instantiate(laserShotTemplate, pos, rot) as GameObject;
		Shot shot = newLaser.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.BULLET);
		shot.enabled = true;
		return shot;
	}

	public Shot CreatePhaserShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newPhaser = GameObject.Instantiate(phaserShotTemplate, pos, rot) as GameObject;
		Shot shot = newPhaser.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.PHASER);
		shot.enabled = true;
		return shot;
	}
	
	public Shot CreateMissileShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newMissile = GameObject.Instantiate(missileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.MISSILE);
		shot.enabled = false;
		if (mountedTo == Game.ENEMY) {
			shot.gameObject.layer = Game.LAYER_GUN_ENEMY;
		}
		return shot;
	}

	public Shot CreateMineTouchShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newMineTouch = GameObject.Instantiate(mineTouchTemplate, pos, rot) as GameObject;
		Shot shot = newMineTouch.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.MINE_TOUCH);
		shot.enabled = false;
		return shot;
	}
	
	public Shot CreateLaserBeamShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject laserBeam = GameObject.Instantiate(game.shotPrefabs[Shot.LASER_BEAM], pos, rot) as GameObject;
		Shot shot = laserBeam.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.LASER_BEAM);
		shot.enabled = false;
		return shot;
	}
	
	public Shot CreateGuidedMissileShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newMissile = GameObject.Instantiate(missileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.GUIDED);
		shot.enabled = false;
		if (mountedTo == Game.ENEMY) {
			shot.gameObject.layer = Game.LAYER_GUN_ENEMY;
		}
		return shot;
	}
	
	public GameObject CreateBreadcrumb(Vector3 pos, Quaternion rot) {
		GameObject newBreadcrumb = GameObject.Instantiate(breadcrumbTemplate, pos, rot) as GameObject;
		Breadcrumb breadcrumb = newBreadcrumb.GetComponent<Breadcrumb>();
		breadcrumb.Initialize(play);
		breadcrumb.enabled = true;
		return newBreadcrumb;
	}

	public GameObject CreateHealthDrop(Vector3 pos, Quaternion rot, int amount) {
		GameObject newHealthDrop = GameObject.Instantiate(healthDropTemplate, pos, rot) as GameObject;
		newHealthDrop.tag = CollecteablePowerUp.TAG;
		CollecteableHealth healthDrop = newHealthDrop.GetComponent<CollecteableHealth>();
		healthDrop.Initialize(play, amount);
		healthDrop.enabled = true;
		return newHealthDrop;
	}

	public GameObject CreateShieldDrop(Vector3 pos, Quaternion rot, int amount) {
		GameObject newShieldDrop = GameObject.Instantiate(shieldDropTemplate, pos, rot) as GameObject;
		newShieldDrop.tag = CollecteablePowerUp.TAG;
		CollecteableShield shieldDrop = newShieldDrop.GetComponent<CollecteableShield>();
		shieldDrop.Initialize(play, amount);
		shieldDrop.enabled = true;
		return newShieldDrop;
	}

	public GameObject CreateMissileDrop(Vector3 pos, Quaternion rot, int type, int amount) {
		GameObject newMissileDrop = GameObject.Instantiate(missileDropTemplate, pos, rot) as GameObject;
		newMissileDrop.tag = CollecteablePowerUp.TAG;
		CollecteableMissile missileDrop = newMissileDrop.GetComponent<CollecteableMissile>();
		missileDrop.Initialize(play, type, amount);
		missileDrop.enabled = true;
		return newMissileDrop;
	}

	public GameObject CreateKeyDrop(Vector3 pos, Quaternion rot, int keyType) {
		GameObject newKeyDrop = GameObject.Instantiate(game.keyPrefab, pos, rot) as GameObject;
		CollecteableKey keyDrop = newKeyDrop.GetComponent<CollecteableKey>();
		keyDrop.Initialize(play, keyType);
		keyDrop.renderer.material.mainTexture = play.game.keyTextures[keyType];
		keyDrop.enabled = true;
		return newKeyDrop;
	}

	public GameObject CreatePowerUpDrop(Vector3 pos, Quaternion rot, int type, int id) {
		GameObject newPowerUpDrop;
		if (type == Game.POWERUP_PRIMARY_WEAPON) {
			newPowerUpDrop = GameObject.Instantiate(game.primaryWeaponPrefabs[id], pos, rot) as GameObject;
		} else {
//			newPowerUpDrop = GameObject.Instantiate(game.primaryWeaponPrefabs[id], pos, rot) as GameObject;
			newPowerUpDrop = GameObject.Instantiate(game.powerUpPrefabs[id], pos, rot) as GameObject;
		}
		CollecteablePowerUp powerUpDrop = newPowerUpDrop.AddComponent<CollecteablePowerUp>();
		SphereCollider col = newPowerUpDrop.AddComponent<SphereCollider>();
		col.isTrigger = true;
		powerUpDrop.Initialize(play, type, id);
		powerUpDrop.enabled = true;
		return newPowerUpDrop;
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

